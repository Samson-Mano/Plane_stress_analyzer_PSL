#include "stress2d_solver.h"

stress2d_solver::stress2d_solver()
{
// Empty Constructor

}


bool stress2d_solver::initialize_solver(stress_system_store* stress_system, int polynomial_order, 
	int solver_type,  stopwatch_events* stopwatch,	
	void(*callback)(const char*))
{

	// Set the stopwatch
	this->m_stopwatch = stopwatch;

	// Store callback locally
	this->m_callback = callback;

	this->polynomial_order = polynomial_order;
	this->solver_type = solver_type;

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
	
	// Create the global stiffness matrix
	create_global_stiffness_matrix();

	report("Global stiffness matrix created");




}


void stress2d_solver::create_global_stiffness_matrix()
{

	this->numDOF = static_cast<int>(polynomial_2dmesh.polynomial_node_list.size()) * 2;

	// Global stiffness matrix
	this->global_stiffness_matrix.resize(numDOF, numDOF);
	this->global_stiffness_matrix.setZero();

	std::vector<Eigen::Triplet<double>> k_triplets;

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

