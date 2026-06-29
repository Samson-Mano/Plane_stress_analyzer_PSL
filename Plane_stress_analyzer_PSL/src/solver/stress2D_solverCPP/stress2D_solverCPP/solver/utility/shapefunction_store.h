#pragma once
#include <vector>
#include <Eigen/Dense>

#include "integration_rules.h"
#include "shape_functions.h"

enum element_type
{
	TRI = 0,
	QUAD = 1
};


struct shapefunction_data
{
	integration_point ip;                       // Integration point coordinates and weight
    std::vector<std::pair<double, double>> dN;  // Shape function derivatives
    Eigen::MatrixXd N_mat;                      // Shape function matrix
	std::vector<double> N;                       // Shape function values (for reference)

};

class shapefunction_store
{
public:
	explicit shapefunction_store(element_type e, int order);
	~shapefunction_store() = default;

	// Access pre-computed data
	const std::vector<shapefunction_data>& get_data() const { return NdN_data; }

	// Get number of nodes per element
	int get_nodes_per_element() const { return nodes_per_element; }


private:
	std::vector<shapefunction_data> NdN_data;
	int nodes_per_element = 0;


	void build_tri_data(int order);
	void build_quad_data(int order);
	Eigen::MatrixXd compute_N_matrix(const std::vector<double>& N, int num_nodes) const;


};

