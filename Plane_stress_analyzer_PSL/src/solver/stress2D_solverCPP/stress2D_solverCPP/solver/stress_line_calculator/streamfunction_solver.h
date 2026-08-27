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


#include <iomanip> // to get std::setprecision()


#include "../polynomial_2dmesh_store.h"



class streamfunction_solver
{
public:
	streamfunction_solver();
	~streamfunction_solver() = default;


	void compute_streamfunction(std::unordered_map<int, renderer_node>& renderer_node_points,
		const std::vector<renderer_triangle>& renderer_triangles,
		bool _isTensionLine,
		void(*callback)(const char*));



private:



	void(*m_callback)(const char*) = nullptr;



	bool isTensionLine = true;

	//Eigen::SparseMatrix<double> globalKMatrix; // Global Stiffness Matrix [K]

	//Eigen::VectorXd globalFVector; // Global Force Vector [F]



	Eigen::Matrix3d getElementKMatrix(const std::unordered_map<int, renderer_node>& renderer_node_points,
		const renderer_triangle& renderer_tri);

	Eigen::Vector3d getElementFVector(const std::unordered_map<int, renderer_node>& renderer_node_points,
		const renderer_triangle& renderer_tri);

	Eigen::Vector3d getElementFVector_S1(const std::unordered_map<int, renderer_node>& renderer_node_points,
		const renderer_triangle& renderer_tri);


	void report(const char* msg);


};




