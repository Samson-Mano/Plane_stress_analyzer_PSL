#pragma once
#include <Eigen/Dense>

#include "../polynomial_2dmesh_store.h"

#include "../utility/shapefunction_store.h"

#include "../../system_store/stress_system_store.h"

class quadelement_formulation
{

public:
	explicit quadelement_formulation(int order);
	~quadelement_formulation() = default;


	Eigen::MatrixXd compute_quadelement_stiffness_matrix(const std::vector<Eigen::Vector2d>& node_coords,
		const material_store& element_material);


	Eigen::MatrixXd compute_quadelement_mass_matrix(const std::vector<Eigen::Vector2d>& node_coords,
		const material_store& element_material);


	std::vector<Eigen::Vector3d> compute_quadelement_strain(const std::vector<Eigen::Vector2d>& node_coords,
		const std::vector<Eigen::Vector2d>& node_displacements);


	const int& get_element_dof() const { return element_dof; }

private:
	int polynomial_order = 0;
	int nodes_per_element = 0;
	int element_dof = 0;

	shapefunction_store quad_sf_store;


	Eigen::Matrix2d compute_jacobian(const std::vector<Eigen::Vector2d>& node_coords,
		const std::vector<std::pair<double, double>>& dN); 

	Eigen::MatrixXd compute_B_matrix(const Eigen::Matrix2d& J_inv,
		const std::vector<std::pair<double, double>>& dN);


};




