
#include <iostream>
#include <fstream>
#include <vector>
#include <cmath>
#include <string>
#include <cstdint>
#include <iomanip>
#include <sstream>

#include "system_store/stress_system_store.h"
#include "system_store/stopwatch_events.h"
// #include "solver/helmholtz2d_spectral_solver.h"



// Function to solve the system setting from C# or Python
extern "C" __declspec(dllexport) void solve_2DstressanalysisCPP(
	const char* input_file,
	const char* output_file,
	const int* solver_settings,
	int solver_settings_count,
	bool* isAnalysisSuccess,
	void(*callback)(const char*))
{
	std::string msg = "";


	// Example placeholder
	std::ifstream infile(input_file, std::ios::binary);
	std::ofstream outfile(output_file, std::ios::binary);

	if (callback) callback("Initializing solver...");
	(*isAnalysisSuccess) = false;


	int solvertype = 0;
	int h_refinement = 0;
	int polynomial_order = 0;
	int formulation = 0;

	if (solver_settings && solver_settings_count == 4)
	{
		solvertype = solver_settings[0];
		h_refinement = solver_settings[1];
		polynomial_order = solver_settings[2];
		formulation = solver_settings[3];

		std::string s_type = "";
		if (solvertype == 0)
		{
			s_type = "Elimination method";
		}
		else if (solvertype == 1)
		{
			s_type = "Lagrange method";
		}

		std::string f_type = "";

		if(formulation == 0)
		{
			f_type = "Plane stress formulation";
		}
		else if (formulation == 1)
		{
			f_type = "Plane strain formulation";
		}

		msg = "Solver type = " + s_type +
			", H_refinement order = " + std::to_string(h_refinement) + ", Polynomial order = " + 
			std::to_string(polynomial_order) + ", Formulation = " + f_type;

		if (callback) callback(msg.c_str());
	}
	else
	{
		msg = "Solver settings error";
		if (callback) callback(msg.c_str());

		return;
	}



	stopwatch_events stopwatch;
	std::stringstream stopwatch_elapsed_str;


	if (!infile.is_open())
	{
		msg = "Error: Unable to open input file: " + std::string(input_file);
		if (callback) callback(msg.c_str());

		(*isAnalysisSuccess) = false;

		// std::cerr << "Error: Unable to open input file: " << input_file << std::endl;
		return;
	}
	if (!outfile.is_open())
	{
		msg = "Error: Unable to open output file: " + std::string(output_file);
		if (callback) callback(msg.c_str());

		(*isAnalysisSuccess) = false;

		// std::cerr << "Error: Unable to open output file: " << output_file << std::endl;
		return;
	}




	// (*isAnalysisSuccess) = true;

	//_________________________________________________________
	// Close the files
	infile.close();
	outfile.close();


}