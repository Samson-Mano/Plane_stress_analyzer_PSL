#include "quadelement_formulation.h"

quadelement_formulation::quadelement_formulation(int order) : polynomial_order(order)
{
	// Number of integration points per direction — one more than the polynomial
	// order ensures exact integration of the (2p-1)-degree integrand in the
	// stiffness matrix (B^T C B contains derivatives of order-p shape functions).
 
	// Get the integration points
	this->integ_points = integration_rules::get_quad_2d_gauss_points(order + 1);

	// Set the degree of freedom based on polynomial order Q4, Q9, Q16, Q25
	// DOF = 2 * (order + 1) ^ 2  (2 DOF per node, (order + 1) ^ 2 nodes per element)
	int nodes_per_elem = (order + 1) * (order + 1);
	this->element_dof = 2 * nodes_per_elem;
	
}


Eigen::MatrixXd quadelement_formulation::compute_quadelement_stiffness_matrix(
	const std::vector<Eigen::Vector2d>& node_coords,
	const material_store& element_material)
{
	// Returns the (2n x 2n) 
	// stiffness matrix K = ∫ B^T C B t dΩ

	Eigen::MatrixXd K = Eigen::MatrixXd::Zero(this->element_dof, this->element_dof);
	Eigen::Matrix3d C = element_material.get_elasticity_matrix();

	// Numerical integration
	for (const integration_point& ip : integ_points)
	{
		double xi = ip.xi;
		double eta = ip.eta;
		double weight = ip.weight;

		// Compute Jacobian
		Eigen::Matrix2d J = compute_jacobian(node_coords, xi, eta);
		double detJ = J.determinant();
		Eigen::Matrix2d J_inv = J.inverse();


		// Compute B matrix
		Eigen::MatrixXd B = compute_B_matrix(J_inv, xi, eta);

		// Compute B^T * C * B
		Eigen::MatrixXd BT_C_B = B.transpose() * C * B;

		// Accumulate stiffness
		K += BT_C_B * detJ * weight * element_material.thickness;
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

	for (const integration_point& ip : integ_points)
	{
		Eigen::Matrix2d J = compute_jacobian(node_coords, ip.xi, ip.eta);
		double          detJ = J.determinant();

		Eigen::MatrixXd N_mat = compute_N_matrix(ip.xi, ip.eta);

		// M += ρ t (N^T N) detJ w
		M += N_mat.transpose() * N_mat
			* (element_material.matdensity * element_material.thickness * detJ * ip.weight);
	}

	return M;

}


Eigen::Matrix2d quadelement_formulation::compute_jacobian(const std::vector<Eigen::Vector2d>& node_coords,
	double xi, double eta)
{
	// Jacobian of the isoparametric mapping at (xi, eta)

	// Jacobian  J = Σ_i [ dNi/dξ · xi,  dNi/dξ · yi ]
	//               Σ_i [ dNi/dη · xi,  dNi/dη · yi ]


	std::vector<std::pair<double, double>> dN = shape_functions::get_quad_shape_derivatives(this->polynomial_order, xi, eta);

	Eigen::Matrix2d J = Eigen::Matrix2d::Zero();

	for (int i = 0; i < static_cast<int>(node_coords.size()); i++)
	{
		J(0, 0) += dN[i].first * node_coords[i].x(); // ∂x/∂ξ
		J(0, 1) += dN[i].first * node_coords[i].y(); // ∂y/∂ξ
		J(1, 0) += dN[i].second * node_coords[i].x(); // ∂x/∂η
		J(1, 1) += dN[i].second * node_coords[i].y(); // ∂y/∂η
	}

	return J;


}

Eigen::MatrixXd quadelement_formulation::compute_B_matrix(const Eigen::Matrix2d& J_inv,
	double xi, double eta)
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

	std::vector<std::pair<double, double>> dN = shape_functions::get_quad_shape_derivatives(this->polynomial_order, xi, eta);

	int num_nodes = static_cast<int>(dN.size());
	Eigen::MatrixXd B = Eigen::MatrixXd::Zero(3, 2 * num_nodes);

	for (int i = 0; i < num_nodes; ++i)
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



Eigen::MatrixXd quadelement_formulation::compute_N_matrix(double xi, double eta)
{
	// Shape function matrix (2 x 2n) for the mass matrix integrand

	// Shape function matrix  N_mat (2 x 2n)
	//
	//          | N1  0   N2  0  ... Nn  0  |
	// N_mat =  |  0  N1   0  N2 ...  0  Nn |
	//
	// Maps the nodal DOF vector  d = [u1 v1 u2 v2 ... un vn]^T
	// to displacements  {u, v} = N_mat · d


	std::vector<double> N = shape_functions::get_quad_shape_functions(this->polynomial_order, xi, eta);

	int num_nodes = static_cast<int>(N.size());
	Eigen::MatrixXd N_mat = Eigen::MatrixXd::Zero(2, 2 * num_nodes);

	for (int i = 0; i < num_nodes; ++i)
	{
		N_mat(0, 2 * i) = N[i];   // u-component
		N_mat(1, 2 * i + 1) = N[i];   // v-component
	}
	return N_mat;

}