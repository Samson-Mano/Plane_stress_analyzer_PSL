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



#include "../system_store/stopwatch_events.h"
#include "../system_store/stress_system_store.h"
#include "../solver/polynomial_2dmesh_store.h"



class stress2d_solver
{
public:
	stress2d_solver();
	~stress2d_solver() = default;

	bool initialize_solver(stress_system_store* stress_system, int polynomial_order, 
		int solver_type,  stopwatch_events* stopwatch,
		void(*callback)(const char*));

	void perform_solve();


private:
	int polynomial_order = 0;
	int solver_type = 0;

	polynomial_2dmesh_store polynomial_2dmesh;
	stopwatch_events* m_stopwatch;

	void(*m_callback)(const char*) = nullptr;

	int numDOF = 0;

	Eigen::SparseMatrix<double> global_stiffness_matrix; // Global Stiffness Matrix [K]
	
	void create_global_stiffness_matrix();

	void report(const char* msg);


};