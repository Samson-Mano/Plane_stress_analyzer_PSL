#pragma once
#include <Eigen/Dense>

#include "../polynomial_2dmesh_store.h"

#include "../utility/integration_rules.h"
#include "../utility/shape_functions.h"

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

private:
	int polynomial_order = 0;
	int element_dof = 0;
	std::vector<integration_point> integ_points;


	Eigen::Matrix2d compute_jacobian(const std::vector<Eigen::Vector2d>& node_coords, double xi, double eta);

	Eigen::MatrixXd compute_B_matrix(const Eigen::Matrix2d& J_inv, double xi, double eta);

	Eigen::MatrixXd compute_N_matrix(double xi, double eta);

};




