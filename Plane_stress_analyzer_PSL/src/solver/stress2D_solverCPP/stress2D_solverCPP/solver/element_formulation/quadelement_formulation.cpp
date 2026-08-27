#include "quadelement_formulation.h"

quadelement_formulation::quadelement_formulation(int order) : polynomial_order(order),
	quad_sf_store(QUAD, order)
{
	
	this->nodes_per_element = quad_sf_store.get_nodes_per_element();
	this->element_dof = 2 * this->nodes_per_element;

}


Eigen::MatrixXd quadelement_formulation::compute_quadelement_stiffness_matrix(
	const std::vector<Eigen::Vector2d>& node_coords,
	const material_store& element_material)
{
	// Returns the (2n x 2n) 
	// stiffness matrix K = ∫ B^T C B t dΩ

	Eigen::MatrixXd K = Eigen::MatrixXd::Zero(this->element_dof, this->element_dof);
	Eigen::Matrix3d C = element_material.get_elasticity_matrix();

	const auto& quad_sf_datas_all = quad_sf_store.get_data();


	for (int idx = 0; idx < static_cast<int>(quad_sf_datas_all.size()); ++idx)
	{
		const auto& quad_sf_data = quad_sf_datas_all[idx];
		const integration_point& ip = quad_sf_data.ip;

		// Compute Jacobian
		Eigen::Matrix2d J = compute_jacobian(node_coords, quad_sf_data.dN);
		double detJ = J.determinant();

		// // Validate Jacobian
		// validate_jacobian(detJ, ip.xi, ip.eta);

		// Compute inverse Jacobian
		Eigen::Matrix2d J_inv = J.inverse();

		// Compute B matrix
		Eigen::MatrixXd B = compute_B_matrix(J_inv, quad_sf_data.dN);

		// Accumulate stiffness
		Eigen::MatrixXd BT_C_B = B.transpose() * C * B;
		K += BT_C_B * detJ * ip.weight * element_material.thickness;
	}

	return K;

}



Eigen::MatrixXd quadelement_formulation::compute_quadelement_mass_matrix(
	const std::vector<Eigen::Vector2d>& node_coords,
	const material_store& element_material)
{
	// Returns the (2n x 2n) 
	// consistent mass matrix M = ∫ ρ t N^T N dΩ

	Eigen::MatrixXd M = Eigen::MatrixXd::Zero(this->element_dof, this->element_dof);
	double density_thickness = element_material.matdensity * element_material.thickness;

	const auto& quad_sf_datas_all = quad_sf_store.get_data();


	for (int idx = 0; idx < static_cast<int>(quad_sf_datas_all.size()); ++idx)
	{
		const auto& quad_sf_data = quad_sf_datas_all[idx];
		const integration_point& ip = quad_sf_data.ip;

		// Compute Jacobian
		Eigen::Matrix2d J = compute_jacobian(node_coords, quad_sf_data.dN);
		double detJ = J.determinant();

		// // Validate Jacobian
		// validate_jacobian(detJ, ip.xi, ip.eta);

		// Accumulate mass matrix
		const Eigen::MatrixXd& N_mat = quad_sf_data.N_mat;
		M += N_mat.transpose() * N_mat * (density_thickness * detJ * ip.weight);

	}

	return M;

}


Eigen::Matrix2d quadelement_formulation::compute_jacobian(const std::vector<Eigen::Vector2d>& node_coords,
	const std::vector<std::pair<double, double>>& dN)
{
	// Jacobian of the isoparametric mapping at (xi, eta)

	// Jacobian  J = Σ_i [ dNi/dξ · xi,  dNi/dξ · yi ]
	//               Σ_i [ dNi/dη · xi,  dNi/dη · yi ]


	Eigen::Matrix2d J = Eigen::Matrix2d::Zero();

	for (int i = 0; i < this->nodes_per_element; i++)
	{
		J(0, 0) += dN[i].first * node_coords[i].x(); // ∂x/∂ξ
		J(0, 1) += dN[i].first * node_coords[i].y(); // ∂y/∂ξ
		J(1, 0) += dN[i].second * node_coords[i].x(); // ∂x/∂η
		J(1, 1) += dN[i].second * node_coords[i].y(); // ∂y/∂η
	}

	return J;

}


Eigen::MatrixXd quadelement_formulation::compute_B_matrix(const Eigen::Matrix2d& J_inv,
	const std::vector<std::pair<double, double>>& dN) 
{
	// Strain-displacement matrix (3 x 2n).
	// Accepts a pre-computed J_inv to avoid recomputing the Jacobian.

	// B matrix (3 x 2n) — strain-displacement in physical coordinates
	//
	//        | dNi/dx    0      |
	// Bi  =  |   0     dNi/dy  |
	//        | dNi/dy  dNi/dx  |
	//
	// Physical derivatives from natural ones via the inverse Jacobian:
	//   { dN/dx }         { dN/dξ }
	//   { dN/dy } = J^-T  { dN/dη }
	//

	Eigen::MatrixXd B = Eigen::MatrixXd::Zero(3, 2 * this->nodes_per_element);

	for (int i = 0; i < this->nodes_per_element; ++i)
	{
		// Transform derivatives from natural to physical coordinates
		double dN_dx = J_inv(0, 0) * dN[i].first + J_inv(0, 1) * dN[i].second;
		double dN_dy = J_inv(1, 0) * dN[i].first + J_inv(1, 1) * dN[i].second;

		// B matrix for plane stress/strain
		B(0, 2 * i) = dN_dx;           // εx = ∂u/∂x
		B(1, (2 * i) + 1) = dN_dy;       // εy = ∂v/∂y
		B(2, 2 * i) = dN_dy;           // γxy = ∂u/∂y + ∂v/∂x
		B(2, (2 * i) + 1) = dN_dx;
	}

	return B;

}




std::vector<Eigen::Vector3d> quadelement_formulation::compute_quadelement_strain(
	const std::vector<Eigen::Vector2d>& node_coords,
	const std::vector<Eigen::Vector2d>& node_displacements)
{
	// Strain matrix E = B * d
	// B is the strain-displacement matrix [ 3 x 2n], d is the nodal displacement vector [ 2n x 1]

	std::vector<Eigen::Vector3d> strain_at_ips; // Strain at integration points

	// Create a vector of nodal displacements (2n x 1)
	Eigen::VectorXd displ(this->element_dof);

	for (int i = 0; i < this->nodes_per_element; ++i)
	{
		displ((2 * i) + 0) = node_displacements[i].x();
		displ((2 * i) + 1) = node_displacements[i].y();
	}

	const auto& quad_sf_datas_all = quad_sf_store.get_data();

	for (int idx = 0; idx < static_cast<int>(quad_sf_datas_all.size()); ++idx)
	{
		const auto& quad_sf_data = quad_sf_datas_all[idx];
		const integration_point& ip = quad_sf_data.ip;

		// Compute Jacobian
		Eigen::Matrix2d J = compute_jacobian(node_coords, quad_sf_data.dN);
		double detJ = J.determinant();

		// // Validate Jacobian
		// validate_jacobian(detJ, ip.xi, ip.eta);

		// Compute inverse Jacobian
		Eigen::Matrix2d J_inv = J.inverse();

		// Compute B matrix
		Eigen::MatrixXd B = compute_B_matrix(J_inv, quad_sf_data.dN);

		// Compute strain at this integration point
		Eigen::Vector3d strain = B * displ;

		// Add to the vector of strains at integration points
		strain_at_ips.push_back(strain);
	}

	return strain_at_ips;
}






