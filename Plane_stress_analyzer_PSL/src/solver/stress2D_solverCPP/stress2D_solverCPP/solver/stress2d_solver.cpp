#include "stress2d_solver.h"

stress2d_solver::stress2d_solver()
{
// Empty Constructor

}


void stress2d_solver::initialize_solver(stress_system_store* stress_system, stopwatch_events* stopwatch,
	void(*callback)(const char*))
{

	// Set the stopwatch
	this->m_stopwatch = stopwatch;

	// Store callback locally
	this->m_callback = callback;

	report("Solver initialized successfully");

	polynomial_2dmesh.generate_2dpolynomial_mesh(stress_system);

	

}


void stress2d_solver::perform_solve()
{
	// Create the polynomial mesh





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

