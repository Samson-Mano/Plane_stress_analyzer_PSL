#include "stress2d_solver.h"

stress2d_solver::stress2d_solver(int solver_type, int polynomial_order) :
	polynomial_order(polynomial_order), solver_type(solver_type), 
	quad_elem_formulation(polynomial_order), tri_elem_formulation(polynomial_order)
{
// Empty Constructor

}


bool stress2d_solver::initialize_solver(stress_system_store* stress_system, 
	bool isSelfWeight, double accl_x, double accl_y,
	stopwatch_events* stopwatch, void(*callback)(const char*))
{

	// Set the stopwatch
	this->m_stopwatch = stopwatch;

	// Store callback locally
	this->m_callback = callback;


	this->isSelfWeight = isSelfWeight;

	if (this->isSelfWeight == true)
	{
		// Gravity load
		this->accl_x = accl_x;
		this->accl_y = accl_y;
	}


	report("Solver initialized successfully");

	polynomial_2dmesh.generate_2dpolynomial_mesh(stress_system, polynomial_order);

	if (polynomial_2dmesh.isPolynomialMeshCreated == true)
	{
		std::string rprt = "Linear solver mesh created";
		if (polynomial_order > 1)
		{
			rprt = "Higher order solver mesh created (order = " + std::to_string(polynomial_order) + ")";
		}
		report(rprt.c_str());

	}
	else
	{
		report("Failed to create solver mesh");

		return false;
	}

	return true;
}


void stress2d_solver::perform_solve()
{

	this->numDOF = static_cast<int>(polynomial_2dmesh.polynomial_node_list.size()) * 2;
	
	this->trielement_dof = tri_elem_formulation.get_element_dof();
	this->quadelement_dof = quad_elem_formulation.get_element_dof();

	// Create the global stiffness matrix
	create_global_stiffness_matrix();

	report("Global stiffness matrix created");

	if (this->isSelfWeight == true)
	{
		create_global_mass_matrix();

		report("Global mass matrix created");
	}



}


void stress2d_solver::create_global_stiffness_matrix()
{


	// Global stiffness matrix
	this->global_stiffness_matrix.resize(numDOF, numDOF);
	this->global_stiffness_matrix.setZero();

	std::vector<Eigen::Triplet<double>> k_triplets;

	if (static_cast<int>(polynomial_2dmesh.polynomial_trielement_list.size()) > 0)
	{
		// Triangle elements
		for (auto& tri_elm_m : polynomial_2dmesh.polynomial_trielement_list)
		{
			// Get the element
			polynomial_trielement_store tri_elm = tri_elm_m.second;

			// Get the material
			material_store element_material = polynomial_2dmesh.get_material_data().at(tri_elm.materialid);

			std::vector<Eigen::Vector2d> node_coords;

			for (int nd_id : tri_elm.ordered_node_ids)
			{
				double x_coord = polynomial_2dmesh.polynomial_node_list[nd_id].x_coord;
				double y_coord = polynomial_2dmesh.polynomial_node_list[nd_id].y_coord;

				node_coords.push_back({x_coord, y_coord});
			}

			// Calculate the element k matrix
			Eigen::MatrixXd element_k_matrix = Eigen::MatrixXd::Zero(this->trielement_dof, this->trielement_dof);

			element_k_matrix = tri_elem_formulation.compute_trielement_stiffness_matrix(node_coords, element_material);

			// Assemble the global stiffness matrix
			for (int i = 0; i < this->trielement_dof; i++)
			{
				// get the global map id
				int i_node_map1 = (tri_elm.ordered_node_ids[i] * 2) + 0;
				int i_node_map2 = (tri_elm.ordered_node_ids[i] * 2) + 1;

				for (int j = 0; j < this->trielement_dof; j++)
				{
					// get the global map id
					int j_node_map1 = (tri_elm.ordered_node_ids[j] * 2) + 0;
					int j_node_map2 = (tri_elm.ordered_node_ids[j] * 2) + 1;

					k_triplets.emplace_back(i_node_map1, j_node_map1, element_k_matrix((i*2) + 0, j));

					// Note: Triplets don’t accumulate — Eigen accumulates when building the sparse matrix.
				}
			}

		}
	}

	if (static_cast<int>(polynomial_2dmesh.polynomial_quadelement_list.size()) > 0)
	{

	}

}


void stress2d_solver::create_global_mass_matrix()
{

	// Global mass matrix
	this->global_mass_matrix.resize(numDOF, numDOF);
	this->global_mass_matrix.setZero();

	std::vector<Eigen::Triplet<double>> m_triplets;

	if (static_cast<int>(polynomial_2dmesh.polynomial_trielement_list.size()) > 0)
	{

	}

	if (static_cast<int>(polynomial_2dmesh.polynomial_quadelement_list.size()) > 0)
	{

	}


}


void stress2d_solver::report(const char* msg)
{
	std::stringstream stopwatch_elapsed_str;

	stopwatch_elapsed_str << std::fixed << std::setprecision(6)
		<< this->m_stopwatch->elapsed();

	std::string final_msg = std::string(msg) + " " +
		stopwatch_elapsed_str.str() +
		" secs";

	if (m_callback)
		m_callback(final_msg.c_str());
	//
}

