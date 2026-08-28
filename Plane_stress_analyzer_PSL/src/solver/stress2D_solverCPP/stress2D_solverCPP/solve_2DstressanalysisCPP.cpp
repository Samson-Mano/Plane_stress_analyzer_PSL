
#include <iostream>
#include <fstream>
#include <vector>
#include <cmath>
#include <string>
#include <cstdint>
#include <iomanip>
#include <sstream>

#include "h_refinement/h_refinement_store.h"
#include "system_store/stress_system_store.h"
#include "system_store/stopwatch_events.h"
#include "solver/stress2d_solver.h"


#pragma pack(push, 1)
struct SolverSettings
{
	int solverType;
	int hRefinement;
	int pRefinement;
	int formulation;
	double extendConstraints;
	double extendLoads;
	double saveHRefinedModel;
	double selfWeight;
	double xAcceleration;
	double yAcceleration;
	double orientationAngle;
};
#pragma pack(pop)



// Function to solve the system setting from C# or Python
extern "C" __declspec(dllexport) void solve_2DstressanalysisCPP(const char* input_file,
	const char* output_file,
	SolverSettings* settings,
	bool* isAnalysisSuccess,
	void(*callback)(const char*))
{


	if (callback) callback("Initializing solver...");
	(*isAnalysisSuccess) = false;



	std::string msg = "";

	if (!settings)
	{
		msg = "Solver settings error";
		if (callback) callback(msg.c_str());

		return;
	}


	// Example placeholder
	std::ifstream infile(input_file, std::ios::binary);
	std::ofstream outfile(output_file, std::ios::binary);


	int solvertype = settings->solverType;
	int h_refinement = settings->hRefinement;
	int polynomial_order = settings->pRefinement + 1;
	int formulation = settings->formulation;
	bool isConstraintExtend = static_cast<int>(settings->extendConstraints) == 0 ? false : true;
	bool isLoadExtend = static_cast<int>(settings->extendLoads) == 0 ? false : true;
	bool isSavehRefinedModel = static_cast<int>(settings->saveHRefinedModel) == 0 ? false : true;
	bool isSelfWeight = static_cast<int>(settings->selfWeight) == 0 ? false : true;
	double accl_x = settings->xAcceleration;
	double accl_y = settings->yAcceleration;
	double orientation_angle = settings->orientationAngle;

	if (accl_x == 0 && accl_y == 0)
		isSelfWeight = false;

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

	if (formulation == 0)
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



	stopwatch_events stopwatch;
	std::stringstream stopwatch_elapsed_str;

	// Start the solver stop watch
	stopwatch.start();


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


	//_______________________________________________________________________________________
	// Read the elements for H Refinement module
	h_refinement_store h_refinement_model;

	// ---------- Nodes ----------
	int32_t nodeCount;
	infile.read(reinterpret_cast<char*>(&nodeCount), 4);

	for (int i = 0; i < nodeCount; i++)
	{
		int32_t node_id = 0; double x_coord = 0.0, y_coord = 0.0;

		infile.read(reinterpret_cast<char*>(&node_id), 4);
		infile.read(reinterpret_cast<char*>(&x_coord), 8);
		infile.read(reinterpret_cast<char*>(&y_coord), 8);

		// Add node to the H Refinement system store
		h_refinement_model.add_node(node_id, x_coord, y_coord);

	}

	stopwatch_elapsed_str.str("");       // clear the string content
	stopwatch_elapsed_str.clear();       // clear any error flags
	stopwatch_elapsed_str << std::fixed << std::setprecision(6) << stopwatch.elapsed();

	msg = "Finished reading nodes at " + stopwatch_elapsed_str.str() + " secs";
	if (callback) callback(msg.c_str());



	// ---------- Tri Elements ----------
	int32_t triCount;
	infile.read(reinterpret_cast<char*>(&triCount), 4);

	for (int i = 0; i < triCount; i++)
	{
		int32_t tri_id = 0, nodeid1 = 0, nodeid2 = 0, nodeid3 = 0, materialid = 0;

		infile.read(reinterpret_cast<char*>(&tri_id), 4);
		infile.read(reinterpret_cast<char*>(&nodeid1), 4);
		infile.read(reinterpret_cast<char*>(&nodeid2), 4);
		infile.read(reinterpret_cast<char*>(&nodeid3), 4);
		infile.read(reinterpret_cast<char*>(&materialid), 4);

		// Add tri element to the H Refinement system store
		h_refinement_model.add_trielement(tri_id, nodeid1, nodeid2, nodeid3, materialid);

	}

	stopwatch_elapsed_str.str("");       // clear the string content
	stopwatch_elapsed_str.clear();       // clear any error flags
	stopwatch_elapsed_str << std::fixed << std::setprecision(6) << stopwatch.elapsed();

	msg = "Finished reading triangular elements at " + stopwatch_elapsed_str.str() + " secs";
	if (callback) callback(msg.c_str());


	// ---------- Quad Elements ----------
	int32_t quadCount;
	infile.read(reinterpret_cast<char*>(&quadCount), 4);

	for (int i = 0; i < quadCount; i++)
	{
		int32_t quad_id = 0, nodeid1 = 0, nodeid2 = 0, nodeid3 = 0, nodeid4 = 0, materialid = 0;

		infile.read(reinterpret_cast<char*>(&quad_id), 4);
		infile.read(reinterpret_cast<char*>(&nodeid1), 4);
		infile.read(reinterpret_cast<char*>(&nodeid2), 4);
		infile.read(reinterpret_cast<char*>(&nodeid3), 4);
		infile.read(reinterpret_cast<char*>(&nodeid4), 4);
		infile.read(reinterpret_cast<char*>(&materialid), 4);

		// Add quad element to the H Refinement system store
		h_refinement_model.add_quadelement(quad_id, nodeid1, nodeid2, nodeid3, nodeid4, materialid);

	}

	stopwatch_elapsed_str.str("");       // clear the string content
	stopwatch_elapsed_str.clear();       // clear any error flags
	stopwatch_elapsed_str << std::fixed << std::setprecision(6) << stopwatch.elapsed();

	msg = "Finished reading quadrilateral elements at " + stopwatch_elapsed_str.str() + " secs";
	if (callback) callback(msg.c_str());


	// ---------- Materials ----------
	int32_t matCount;
	infile.read(reinterpret_cast<char*>(&matCount), 4);

	for (int i = 0; i < matCount; i++)
	{
		int32_t materialid = 0, numelement = 0;
		double material_density = 0.0, youngs_modulus = 0.0, poissons_ratio = 0.0;
		double yield_point = 0.0, thickness = 0.0;

		infile.read(reinterpret_cast<char*>(&materialid), 4);

		int32_t nameLen;
		infile.read(reinterpret_cast<char*>(&nameLen), 4);

		std::string matname(nameLen, '\0');
		infile.read(&matname[0], nameLen);

		infile.read(reinterpret_cast<char*>(&material_density), 8);
		infile.read(reinterpret_cast<char*>(&youngs_modulus), 8);
		infile.read(reinterpret_cast<char*>(&poissons_ratio), 8);
		infile.read(reinterpret_cast<char*>(&yield_point), 8);
		infile.read(reinterpret_cast<char*>(&thickness), 8);

		// Add material to the H Refinement system store
		h_refinement_model.add_material(materialid, youngs_modulus, material_density, poissons_ratio,
			yield_point, thickness, formulation);

	}

	stopwatch_elapsed_str.str("");       // clear the string content
	stopwatch_elapsed_str.clear();       // clear any error flags
	stopwatch_elapsed_str << std::fixed << std::setprecision(6) << stopwatch.elapsed();

	msg = "Finished reading materials at " + stopwatch_elapsed_str.str() + " secs";
	if (callback) callback(msg.c_str());


	// ---------- Node Constraints ----------
	int32_t ndCnstCount;
	infile.read(reinterpret_cast<char*>(&ndCnstCount), 4);

	for (int i = 0; i < ndCnstCount; i++)
	{
		int32_t nodeConstraintsetid = 0;
		int32_t constrainttype = 0;
		double constraint_angle = 0.0;

		infile.read(reinterpret_cast<char*>(&nodeConstraintsetid), 4);
		infile.read(reinterpret_cast<char*>(&constrainttype), 4);
		infile.read(reinterpret_cast<char*>(&constraint_angle), 8);

		int32_t nidCount;
		infile.read(reinterpret_cast<char*>(&nidCount), 4);

		std::vector<int> node_id_list;

		for (int j = 0; j < nidCount; j++)
		{
			int32_t node_id = 0;
			infile.read(reinterpret_cast<char*>(&node_id), 4);

			// Update the constraint of the node where constarints are applied
			node_id_list.push_back(node_id);
		}

		// Add node constraints to the H Refinement system store
		h_refinement_model.add_nodeconstraint(nodeConstraintsetid, constrainttype, constraint_angle, node_id_list);

	}

	stopwatch_elapsed_str.str("");       // clear the string content
	stopwatch_elapsed_str.clear();       // clear any error flags
	stopwatch_elapsed_str << std::fixed << std::setprecision(6) << stopwatch.elapsed();

	msg = "Finished reading nodal constraints at " + stopwatch_elapsed_str.str() + " secs";
	if (callback) callback(msg.c_str());



	// ---------- Node Loads ----------
	int32_t ndLoadCount;
	infile.read(reinterpret_cast<char*>(&ndLoadCount), 4);

	for (int i = 0; i < ndLoadCount; i++)
	{
		int32_t nodeLoadsetid = 0;
		double load_anplitude = 0;
		double load_angle = 0.0;

		infile.read(reinterpret_cast<char*>(&nodeLoadsetid), 4);
		infile.read(reinterpret_cast<char*>(&load_anplitude), 8);
		infile.read(reinterpret_cast<char*>(&load_angle), 8);

		int32_t nidCount;
		infile.read(reinterpret_cast<char*>(&nidCount), 4);

		std::vector<int> node_id_list;

		for (int j = 0; j < nidCount; j++)
		{
			int32_t node_id = 0;
			infile.read(reinterpret_cast<char*>(&node_id), 4);

			// Update the constraint of the node where constarints are applied
			node_id_list.push_back(node_id);
		}

		// Add node loads to the H Refinement system store
		h_refinement_model.add_nodeload(nodeLoadsetid, load_anplitude, load_angle, node_id_list);

	}

	stopwatch_elapsed_str.str("");       // clear the string content
	stopwatch_elapsed_str.clear();       // clear any error flags
	stopwatch_elapsed_str << std::fixed << std::setprecision(6) << stopwatch.elapsed();

	msg = "Finished reading nodal constraints at " + stopwatch_elapsed_str.str() + " secs";
	if (callback) callback(msg.c_str());


	// Create the edge
	h_refinement_model.create_edge_wireframe();

	// Preform refinement
	h_refinement_model.perform_refinement(h_refinement, isConstraintExtend, isLoadExtend, &stopwatch, callback);


	// Print the H Refined binary file for testing
	if (isSavehRefinedModel == true && h_refinement > 0)
	{
		h_refinement_model.save_hrefined_model();
	}

	
		// Copy H Refined Mesh to the stress analyzer
		stress_system_store stress_system;

		stress_system.polynomial_order = polynomial_order; // 0, 1, 2

		stress_system.node_list = std::move(h_refinement_model.node_list);
		stress_system.edge_list = std::move(h_refinement_model.edge_list);
		stress_system.trielement_list = std::move(h_refinement_model.trielement_list);
		stress_system.quadelement_list = std::move(h_refinement_model.quadelement_list);

		stress_system.material_list = std::move(h_refinement_model.material_list);

		stress_system.constraint_list = std::move(h_refinement_model.constraint_list);
		stress_system.load_list = std::move(h_refinement_model.load_list);

		stress_system.node_edge_map = std::move(h_refinement_model.node_edge_map);

		// Initialize the solver
		bool isSolverInitialized = false;
		(*isAnalysisSuccess) = false;

		stress2d_solver solver(solvertype, polynomial_order);

		isSolverInitialized = solver.initialize_solver(&stress_system, output_file,
			isSelfWeight, accl_x, accl_y, orientation_angle, &stopwatch, callback);


		if (isSolverInitialized == true)
		{
			bool isSolveSuccessful = solver.perform_solve();

			if (isSolveSuccessful == true)
			{
				(*isAnalysisSuccess) = true;
			}
		}


	//_________________________________________________________
	// Close the files
	infile.close();
	outfile.close();




}