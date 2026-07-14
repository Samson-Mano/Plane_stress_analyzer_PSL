#pragma once
#include "../polynomial_2dmesh_store.h"
#include "shape_functions.h"
#include "integration_rules.h"

class ipresult_extrapolator
{

public:
	ipresult_extrapolator(const std::unordered_map<int, polynomial_trielement_store>& polynomial_trielement_list,
		const std::unordered_map<int, polynomial_quadelement_store>& polynomial_quadelement_list, const int polynomial_order);

	~ipresult_extrapolator() = default;


	void extrapolate_results_to_nodes(std::unordered_map<int, polynomial_node_store>& polynomial_node_list);

private:
	const std::unordered_map<int, polynomial_trielement_store>& polynomial_trielement_list;
	const std::unordered_map<int, polynomial_quadelement_store>& polynomial_quadelement_list;
	const int polynomial_order;

	std::unordered_map<int, int> node_elementcount; // First int is node_id, second int is the count of elements sharing that node

	// Store the natural coordinates of each node in each element for extrapolation
	std::vector<std::pair<double, double>> tri_element_natural_coordinates; // First double is xi, second double is eta
	std::vector<std::pair<double, double>> quad_element_natural_coordinates; // First double is xi, second double is eta

	// Store the interpolation weights for each node in each element for extrapolation
	std::vector<std::vector<double>> tri_element_interpolation_weights; // First vector is the node index (natural coordinate index), second vector is the interpolation weight value	
	std::vector<std::vector<double>> quad_element_interpolation_weights; // First vector is the node index (natural coordinate index), second vector is the interpolation weight value	


	void create_element_natural_coordinates();

	void create_tri_element_natural_coordinates(const std::vector<double>& node_spacing);

	void create_quad_element_natural_coordinates(const std::vector<double>& node_spacing);

	void create_interpolation_weights_at_nodes();

	void create_tri_least_squares_extrapolation_weights();

	void create_quad_least_squares_interpolation_weights();

};




