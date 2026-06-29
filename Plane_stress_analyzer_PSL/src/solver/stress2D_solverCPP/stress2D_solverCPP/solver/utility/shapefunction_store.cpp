#include "shapefunction_store.h"


shapefunction_store::shapefunction_store(element_type e, int order)
{

	if (order < 1 || order > 4) 
	{
		throw std::invalid_argument("Polynomial order must be between 1 and 4");
	}

	switch (e) 
	{
	case TRI:
		build_tri_data(order);
		break;
	case QUAD:
		build_quad_data(order);
		break;
	default:
		throw std::invalid_argument("Invalid element type");
	}

}

void shapefunction_store::build_tri_data(int order)
{
	// Triangle element: T3 (p=1), T6 (p=2), T10 (p=3), T15 (p=4)
	this->nodes_per_element = ((order + 1) * (order + 2)) / 2;

	// Get integration points for triangle
	std::vector<integration_point> integ_points =
		integration_rules::get_tri_gauss_points(order + 1);

	// Pre-compute shape functions and derivatives for all integration points
	NdN_data.reserve(integ_points.size());

	for (const integration_point& ip : integ_points)
	{
		shapefunction_data data;
		data.ip = ip;

		// Get shape functions
		data.N = shape_functions::get_tri_shape_functions(order, ip.xi, ip.eta);

		// Get shape function derivatives
		data.dN = shape_functions::get_tri_shape_derivatives(order, ip.xi, ip.eta);

		// Create N matrix
		data.N_mat = compute_N_matrix(data.N, this->nodes_per_element);

		NdN_data.push_back(std::move(data));
	}
}



void shapefunction_store::build_quad_data(int order)
{
	// Quadrilateral element: Q4 (p=1), Q9 (p=2), Q16 (p=3), Q25 (p=4)
	this->nodes_per_element = (order + 1) * (order + 1);

	// Get integration points for quadrilateral
	std::vector<integration_point> integ_points =
		integration_rules::get_quad_2d_gauss_points(order + 1);

	// Pre-compute shape functions and derivatives for all integration points
	NdN_data.reserve(integ_points.size());

	for (const integration_point& ip : integ_points)
	{
		shapefunction_data data;
		data.ip = ip;

		// Get shape functions
		data.N = shape_functions::get_quad_shape_functions(order, ip.xi, ip.eta);

		// Get shape function derivatives
		data.dN = shape_functions::get_quad_shape_derivatives(order, ip.xi, ip.eta);

		// Create N matrix
		data.N_mat = compute_N_matrix(data.N, this->nodes_per_element);

		NdN_data.push_back(std::move(data));
	}
}




Eigen::MatrixXd shapefunction_store::compute_N_matrix(const std::vector<double>& N, int num_nodes) const
{
	// Shape function matrix (2 x 2n) for the mass matrix integrand

	// Shape function matrix  N_mat (2 x 2n)
	//
	//          | N1  0   N2  0  ... Nn  0  |
	// N_mat =  |  0  N1   0  N2 ...  0  Nn |
	//
	// Maps the nodal DOF vector  d = [u1 v1 u2 v2 ... un vn]^T
	// to displacements  {u, v} = N_mat · d


	// int num_nodes = static_cast<int>(N.size());
	Eigen::MatrixXd N_mat = Eigen::MatrixXd::Zero(2, 2 * num_nodes);

	for (int i = 0; i < num_nodes; ++i)
	{
		N_mat(0, 2 * i) = N[i];   // u-component
		N_mat(1, (2 * i) + 1) = N[i];   // v-component
	}

	return N_mat;

}




