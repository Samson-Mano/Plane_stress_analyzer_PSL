#include "trielement_formulation.h"

trielement_formulation::trielement_formulation(int order) : polynomial_order(order)
{
	// Number of integration points per direction — one more than the polynomial
	// order ensures exact integration of the (2p-1)-degree integrand in the
	// stiffness matrix (B^T C B contains derivatives of order-p shape functions).

	// Get the integration points
	this->integ_points = integration_rules::get_tri_gauss_points(order);

	// Set the degree of freedom based on polynomial order T3, T6, T10, T15
	// DOF = 3 * (order + 1) ^ 2  (2 DOF per node, (order + 1) ^ 2 nodes per element)
	int nodes_per_elem = (3 * order);
	this->element_dof = 2 * nodes_per_elem;

}




