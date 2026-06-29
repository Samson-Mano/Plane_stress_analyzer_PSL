#pragma once
#include <Eigen/Dense>

#include "../polynomial_2dmesh_store.h"

#include "../utility/shapefunction_store.h"

#include "../../system_store/stress_system_store.h"



class trielement_formulation
{

public:
	explicit trielement_formulation(int order);
	~trielement_formulation() = default;


	Eigen::MatrixXd compute_trielement_stiffness_matrix(const std::vector<Eigen::Vector2d>& node_coords,
		const material_store& element_material);

	Eigen::MatrixXd compute_trielement_mass_matrix(const std::vector<Eigen::Vector2d>& node_coords,
		const material_store& element_material);

private:
	int polynomial_order = 0;
	int nodes_per_element = 0;
	int element_dof = 0;

	shapefunction_store sf_store;


	Eigen::Matrix2d compute_jacobian(const std::vector<Eigen::Vector2d>& node_coords,
		const std::vector<std::pair<double, double>>& dN);

	Eigen::MatrixXd compute_B_matrix(const Eigen::Matrix2d& J_inv,
		const std::vector<std::pair<double, double>>& dN);


};






