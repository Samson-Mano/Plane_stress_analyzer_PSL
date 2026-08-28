#pragma once


#include <cmath>

#pragma warning(push)
#pragma warning (disable : 26451)
#pragma warning (disable : 26495)
#pragma warning (disable : 6255)
#pragma warning (disable : 6294)
#pragma warning (disable : 26813)
#pragma warning (disable : 26454)

// Optimization for Eigen Library
// 1) OpenMP (Yes (/openmp)
//	 Solution Explorer->Configuration Properties -> C/C++ -> Language -> Open MP Support
// 2) For -march=native, choose "AVX2" or the latest supported instruction set.
//   Solution Explorer->Configuration Properties -> C/C++ -> Code Generation -> Enable Enhanced Instruction Set 

#include <Eigen/Dense>
#include <Eigen/Sparse>
#include <Eigen/SparseLU>
#include <Eigen/Eigenvalues>
// Define the sparse matrix type for the reduced global stiffness matrix
typedef Eigen::SparseMatrix<double> SparseMatrix;
#pragma warning(pop)

#include "stress_line_calculator/streamfunction_solver.h"

#include "../system_store/stopwatch_events.h"
#include "../system_store/stress_system_store.h"
#include "../solver/polynomial_2dmesh_store.h"

#include "../solver/element_formulation/trielement_formulation.h"
#include "../solver/element_formulation/quadelement_formulation.h"

#include "../solver/utility/ipresult_extrapolator.h"

#include <fstream>

#include <iomanip> // to get std::setprecision()


class stress2d_solver
{
public:
	stress2d_solver(int solver_type, int polynomial_order);
	~stress2d_solver() = default;

	bool initialize_solver(stress_system_store* stress_system, const char* output_file_char, 
		bool isSelfWeight, double accl_x, double accl_y, double orientation_angle,
		stopwatch_events* stopwatch, void(*callback)(const char*));

	bool perform_solve();

	void store_k_m_matrices_text_debug();

private:
	const double M_PI = 3.14159265358979323846;

	int polynomial_order = 0;
	int solver_type = 0;
	bool isSelfWeight = false;

	double accl_x = 0.0;
	double accl_y = 0.0;

	double orientation_angle = 0.0; // Orientation angle in degrees

	int trielement_dof = 0;
	int quadelement_dof = 0;

	std::string output_file;

	polynomial_2dmesh_store polynomial_2dmesh;
	stopwatch_events* m_stopwatch;

	quadelement_formulation quad_elem_formulation;
	trielement_formulation tri_elem_formulation;


	void(*m_callback)(const char*) = nullptr;

	int numDOF = 0;

	Eigen::SparseMatrix<double> global_supportInclination_matrix; // Global Support Inclination Matrix [S]

	Eigen::SparseMatrix<double> global_stiffness_matrix; // Global Stiffness Matrix [K]
	Eigen::SparseMatrix<double> global_mass_matrix; // Global Mass Matrix [M]


	Eigen::VectorXd global_load_vector; // Global Load Vector [F]
	Eigen::VectorXd global_BC_flag_vector; // Global Boundary Condition Flag Vector [BC]
	Eigen::VectorXd global_displacement_vector; // Global Displacement Vector [U]

	Eigen::VectorXd global_reaction_vector; // Global Reaction Vector [R]


	void solve_BCs_elimination_method();

	void solve_BCs_lagrange_method();



	void create_global_supportInclination_matrix();

	void create_global_stiffness_matrix();

	void create_global_mass_matrix();

	void create_global_load_vector();

	void create_global_load_vector_self_weight();

	void create_global_BC_flag_vector();


	void assemble_element_matrix(const std::vector<int>& node_ids,
		const Eigen::MatrixXd& element_matrix, std::vector<Eigen::Triplet<double>>& triplets);

	bool store_results();

	bool check_valid_results(const Eigen::VectorXd& results, const std::string& result_name);

	void set_element_results();

	void map_results_to_rendererelements();


	element_results compute_element_result_at_ip(const Eigen::Vector3d& stress_at_ip,
		const Eigen::Vector3d& strain_at_ip);

	void report(const char* msg);

	std::string matrix_to_string(const Eigen::MatrixXd& mat);


};