#include "ipresult_extrapolator.h"

ipresult_extrapolator::ipresult_extrapolator(const std::unordered_map<int, polynomial_trielement_store>& polynomial_trielement_list,
	const std::unordered_map<int, polynomial_quadelement_store>& polynomial_quadelement_list, const int polynomial_order)
	: polynomial_trielement_list(polynomial_trielement_list), 
	polynomial_quadelement_list(polynomial_quadelement_list), 
	polynomial_order(polynomial_order)
{
	// Create element natural coordinates for extrapolation
	create_element_natural_coordinates();

	// Create shape function weights for triangle and quadrilateral element at natural coordinates
	create_shape_function_weights_at_nodes();

}


void ipresult_extrapolator::create_element_natural_coordinates()
{
	// Node spacing based on the polynomial order
	std::vector<double> node_spacing;

	if (this->polynomial_order == 1)
	{
		// p = 1 (Linear / Bilinear)
		// No change - T3 + Q4

	}
	else if (this->polynomial_order == 2)
	{
		// p = 2 (Quadratic)
		// T6: Adds 1 node per edge
		// Q9: Adds 1 node per edge + 1 center node
		node_spacing.push_back(0.5);

	}
	else if (this->polynomial_order == 3)
	{
		// p = 3 (Cubic)
		// T10: Adds 2 nodes per edge + 1 internal node
		// Q16: Adds 2 nodes per edge + 4 internal nodes
		node_spacing.push_back(1.0 / 3.0);
		node_spacing.push_back(2.0 / 3.0);

	}
	else if (this->polynomial_order == 4)
	{
		// p = 4 (Quartic)
		// T15: Adds 3 nodes per edge + 3 internal nodes
		// Q25: Adds 3 nodes per edge + 9 internal nodes
		node_spacing.push_back(0.25);
		node_spacing.push_back(0.5);
		node_spacing.push_back(0.75);

	}

	// Create natural coordinates for triangle elements
	create_tri_element_natural_coordinates(node_spacing);

	// Create natural coordinates for quadrilateral elements
	create_quad_element_natural_coordinates(node_spacing);	

}


void ipresult_extrapolator::create_tri_element_natural_coordinates(const std::vector<double>& node_spacing)
{

	// Create natural coordinates for triangle elements
	tri_element_natural_coordinates.clear();

	// Corner nodes
	tri_element_natural_coordinates.push_back({ 0.0, 0.0 }); // Corner 1
	tri_element_natural_coordinates.push_back({ 1.0, 0.0 }); // Corner 2
	tri_element_natural_coordinates.push_back({ 0.0, 1.0 }); // Corner 3

	// Edge nodes
	for (int i = 0; i < 3; i++)
	{
		std::pair<double, double> start_node = tri_element_natural_coordinates[i];
		std::pair<double, double> end_node = tri_element_natural_coordinates[(i + 1) % 3];
		for (double spacing : node_spacing)
		{
			double x = (start_node.first * (1.0 - spacing)) + (end_node.first * spacing);
			double y = (start_node.second * (1.0 - spacing)) + (end_node.second * spacing);
			tri_element_natural_coordinates.push_back({ x, y });
		}
	}


	// Internal nodes
	if (this->polynomial_order == 3)
	{
		// T10: 1 internal node at the centroid
		tri_element_natural_coordinates.push_back({ 1.0 / 3.0, 1.0 / 3.0 });

	}
	else if (this->polynomial_order == 4)
	{
		// T15: 3 internal nodes
		// Using area coordinates: 
			std::vector<std::pair<double, double>> internal_points =
		{
			{1.0 / 2.0, 1.0 / 4.0},
			{1.0 / 4.0, 1.0 / 2.0},
			{1.0 / 4.0, 1.0 / 4.0}
		};


		for (const auto& [alpha, beta] : internal_points)
		{
			double gamma = 1.0 - alpha - beta;
			double x = alpha * 0.0 + beta * 1.0 + gamma * 0.0;
			double y = alpha * 0.0 + beta * 0.0 + gamma * 1.0;
			tri_element_natural_coordinates.push_back({ x, y });
		}
	}
	// Note: The internal nodes are added after the edge nodes, so they will be at the end of the vector.
}



void ipresult_extrapolator::create_quad_element_natural_coordinates(const std::vector<double>& node_spacing)
{

	// Create natural coordinates for quadrilateral elements
	quad_element_natural_coordinates.clear();

	// Corner nodes
	quad_element_natural_coordinates.push_back({ -1.0, -1.0 }); // Corner 1
	quad_element_natural_coordinates.push_back({ 1.0, -1.0 });  // Corner 2
	quad_element_natural_coordinates.push_back({ 1.0, 1.0 });   // Corner 3
	quad_element_natural_coordinates.push_back({ -1.0, 1.0 });  // Corner 4

	// Edge nodes
	for (int i = 0; i < 4; i++)
	{
		std::pair<double, double> start_node = quad_element_natural_coordinates[i];
		std::pair<double, double> end_node = quad_element_natural_coordinates[(i + 1) % 4];


		for (double spacing : node_spacing)
		{
			double x = (start_node.first * (1.0 - spacing)) + (end_node.first * spacing);
			double y = (start_node.second * (1.0 - spacing)) + (end_node.second * spacing);
			quad_element_natural_coordinates.push_back({ x, y });
		}
	}


	// Internal nodes
	if (this->polynomial_order == 2)
	{
		// Q9: 1 center node
		quad_element_natural_coordinates.push_back({ 0.0, 0.0 });

	}
	else if (this->polynomial_order == 3)
	{
		// Q16: 4 internal nodes (2x2 grid)
		double xi[4] = { -0.5, 0.5, -0.5, 0.5 };
		double eta[4] = { -0.5, -0.5, 0.5, 0.5 };

		// Map from natural coordinates (-1,1) to physical coordinates
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				// From left to right, then bottom to top
				quad_element_natural_coordinates.push_back({ xi[j], eta[i] });
			}
		}

	}
	else if (this->polynomial_order == 4)
	{
		// Q25: 9 internal nodes (3x3 grid)
		double xi[9] = { -0.666, 0.0, 0.666, -0.666, 0.0, 0.666, -0.666, 0.0, 0.666 };
		double eta[9] = { -0.666, -0.666, -0.666, 0.0, 0.0, 0.0, 0.666, 0.666, 0.666 };

		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				// From left to right, then bottom to top
				quad_element_natural_coordinates.push_back({ xi[j], eta[i] });
			}
		}
	}
	// Note: The internal nodes are added after the edge nodes, so they will be at the end of the vector.
}



void ipresult_extrapolator::create_shape_function_weights_at_nodes()
{

	//_________________________________________________________________________________________________________________________________
	// === TRIANGLE ELEMENTS ===

	// Get the integration points for triangle elements based on the polynomial order
	std::vector<integration_point> tri_integration_points = integration_rules::get_tri_dunavant_points(this->polynomial_order);

	const int num_nodes_per_tri_element = ((this->polynomial_order + 1) * (this->polynomial_order + 2)) / 2;
	const int num_tri_integration_points = static_cast<int>(tri_integration_points.size());

	// Pre-compute shape functions at integration points
	std::vector<std::vector<double>> tri_shape_functions_at_ip;

	// For each integration point, evaluate the shape functions
	for (const auto& ip : tri_integration_points)
	{
		double xi = ip.xi;
		double eta = ip.eta;

		// Shape function at the integration point
		std::vector<double> N_ip = shape_functions::get_tri_shape_functions(this->polynomial_order, xi, eta);

		tri_shape_functions_at_ip.push_back(N_ip);
		
	}


	// Initialize a 2D vector to store shape function weights at nodes for each integration point
	std::vector<std::vector<double>> tri_shape_function_weights_at_nodes(num_nodes_per_tri_element,
		std::vector<double>(num_tri_integration_points, 0.0));

	for (int nd_id = 0; nd_id < num_nodes_per_tri_element; ++nd_id)
	{
		const std::pair<double, double>& node_nat_coords = tri_element_natural_coordinates[nd_id];

		double xi = node_nat_coords.first;
		double eta = node_nat_coords.second;

		std::vector<double> N_at_node = shape_functions::get_tri_shape_functions(this->polynomial_order, xi, eta);

		// Use shape function values directly
		double total_weight = 0.0;
		std::vector<double> shape_function_weights_at_ip(num_tri_integration_points, 0.0);

		for (int ip_idx = 0; ip_idx < num_tri_integration_points; ++ip_idx)
		{
			// Get the shape function value at the integration point for the current node
			const std::vector<double>& N_ip = tri_shape_functions_at_ip[ip_idx];

			// Calculate weight using dot product
			// This preserves the shape function properties
			double weight = 0.0;
			for (int i = 0; i < num_nodes_per_tri_element; ++i)
			{
				if (std::abs(N_ip[i]) > 1e-10)
				{
					// Simplified: N_at_node[i] * N_ip[i] / (N_ip[i] * N_ip[i]) = N_at_node[i] / N_ip[i]

					weight += (N_at_node[i]  /  N_ip[i]);
				}
			}

			shape_function_weights_at_ip[ip_idx] = weight;

			// Sum the weights for normalization
			total_weight += weight;	
		}

		// Normalize the shape function weights
		if (std::abs(total_weight) > 1e-12) 
		{
			for (auto& w : shape_function_weights_at_ip) 
			{
				w /= total_weight;
			}
		}

		
		tri_shape_function_weights_at_nodes[nd_id] = shape_function_weights_at_ip;
	}

	// shape functions weights for triangle at natural coordinates
	tri_element_shape_function_weights.clear();
	tri_element_shape_function_weights = std::move(tri_shape_function_weights_at_nodes);


	//_________________________________________________________________________________________________________________________________
	// === QUADRILATERAL ELEMENTS ===

	// Get the integration points for quadrilateral elements based on the polynomial order
	std::vector<integration_point> quad_integration_points = integration_rules::get_quad_gauss_points(this->polynomial_order);


	const int num_nodes_per_quad_element = (this->polynomial_order + 1) * (this->polynomial_order + 1);
	const int num_quad_integration_points = static_cast<int>(quad_integration_points.size());


	// Pre-compute shape functions at integration points
	std::vector<std::vector<double>> quad_shape_functions_at_ip;

	// For each integration point, evaluate the shape functions
	for (const auto& ip : quad_integration_points)
	{
		double xi = ip.xi;
		double eta = ip.eta;

		// Shape function at the integration point
		std::vector<double> N_ip = shape_functions::get_quad_shape_functions(this->polynomial_order, xi, eta);

		quad_shape_functions_at_ip.push_back(N_ip);

	}

	// Initialize a 2D vector to store shape function weights at nodes for each integration point
	std::vector<std::vector<double>> quad_shape_function_weights_at_nodes(num_nodes_per_quad_element,
		std::vector<double>(num_quad_integration_points, 0.0));


	for (int nd_id = 0; nd_id < num_nodes_per_quad_element; ++nd_id)
	{
		const std::pair<double, double>& node_nat_coords = quad_element_natural_coordinates[nd_id];

		double xi = node_nat_coords.first;
		double eta = node_nat_coords.second;

		std::vector<double> N_at_node = shape_functions::get_quad_shape_functions(this->polynomial_order, xi, eta);

		// Use shape function values directly
		double total_weight = 0.0;
		std::vector<double> shape_function_weights_at_ip(num_quad_integration_points, 0.0);

		for (int ip_idx = 0; ip_idx < num_quad_integration_points; ++ip_idx)
		{
			// Get the shape function value at the integration point for the current node
			const std::vector<double>& N_ip = quad_shape_functions_at_ip[ip_idx];

			// Calculate weight using dot product
			// This preserves the shape function properties
			double weight = 0.0;
			for (int i = 0; i < num_nodes_per_quad_element; ++i)
			{
				if (std::abs(N_ip[i]) > 1e-10)
				{
					// Simplified: N_at_node[i] * N_ip[i] / (N_ip[i] * N_ip[i]) = N_at_node[i] / N_ip[i]

					weight += (N_at_node[i]  / N_ip[i]);
				}
			}

			shape_function_weights_at_ip[ip_idx] = weight;

			// Sum the weights for normalization
			total_weight += weight;
		}

		// Normalize the shape function weights
		if (std::abs(total_weight) > 1e-12)
		{
			for (auto& w : shape_function_weights_at_ip)
			{
				w /= total_weight;
			}
		}


		quad_shape_function_weights_at_nodes[nd_id] = shape_function_weights_at_ip;
	}

	// shape functions weights for quadrilateral at natural coordinates
	quad_element_shape_function_weights.clear();
	quad_element_shape_function_weights = std::move(quad_shape_function_weights_at_nodes);

}




void ipresult_extrapolator::extrapolate_results_to_nodes(std::unordered_map<int, polynomial_node_store>& polynomial_node_list)
{

	// Initialize nodal results and element count
	for (auto& node_pair : polynomial_node_list)
	{
		polynomial_node_store& node = node_pair.second;
		node.sigma_x = 0.0;
		node.sigma_y = 0.0;
		node.tau_xy = 0.0;
		node.sigma_1 = 0.0;
		node.sigma_2 = 0.0;
		node.von_mises = 0.0;
		node.max_shear = 0.0;
		node.theta_p = 0.0;


		node_elementcount[node.node_id] = 0; // Initialize element count for each node

	}

	// Extrapolate results from triangle elements
	for (const auto& tri_elm_m : polynomial_trielement_list)
	{
		const polynomial_trielement_store& tri_elm = tri_elm_m.second;

		int local_id = 0;

		for (const int& nd_id : tri_elm.ordered_node_ids)
		{
			polynomial_node_store& node = polynomial_node_list[nd_id];

			const std::vector<double>& shape_function_weights = tri_element_shape_function_weights[local_id];

			// Extrapolate results from integration points to the node
			for (int ip_idx = 0; ip_idx < static_cast<int>(tri_elm.results_at_ip.size()); ++ip_idx)
			{
				const element_results& ip_result = tri_elm.results_at_ip[ip_idx];
				double weight = shape_function_weights[ip_idx];
				node.sigma_x += weight * ip_result.sigma_x;
				node.sigma_y += weight * ip_result.sigma_y;
				node.tau_xy += weight * ip_result.tau_xy;
				node.sigma_1 += weight * ip_result.sigma_1;
				node.sigma_2 += weight * ip_result.sigma_2;
				node.von_mises += weight * ip_result.von_mises;
				node.max_shear += weight * ip_result.max_shear;
				node.theta_p += weight * ip_result.theta_p;
			}

			// Increment the element count for this node
			node_elementcount[nd_id]++;
			local_id++;
		}

	}


	// Extrapolate results from quadrilateral elements
	for (const auto& quad_elm_m : polynomial_quadelement_list)
	{
		const polynomial_quadelement_store& quad_elm = quad_elm_m.second;

		int local_id = 0;

		for (const int& nd_id : quad_elm.ordered_node_ids)
		{
			polynomial_node_store& node = polynomial_node_list[nd_id];

			const std::vector<double>& shape_function_weights = quad_element_shape_function_weights[local_id];

			// Extrapolate results from integration points to the node
			for (int ip_idx = 0; ip_idx < static_cast<int>(quad_elm.results_at_ip.size()); ++ip_idx)
			{
				const element_results& ip_result = quad_elm.results_at_ip[ip_idx];
				double weight = shape_function_weights[ip_idx];
				node.sigma_x += weight * ip_result.sigma_x;
				node.sigma_y += weight * ip_result.sigma_y;
				node.tau_xy += weight * ip_result.tau_xy;
				node.sigma_1 += weight * ip_result.sigma_1;
				node.sigma_2 += weight * ip_result.sigma_2;
				node.von_mises += weight * ip_result.von_mises;
				node.max_shear += weight * ip_result.max_shear;
				node.theta_p += weight * ip_result.theta_p;
			}

			// Increment the element count for this node
			node_elementcount[nd_id]++;
			local_id++;
		}

	}


	// Average the results for each node based on the number of elements contributing to it
	for (auto& node_pair : polynomial_node_list)
	{
		polynomial_node_store& node = node_pair.second;
		int count = node_elementcount[node.node_id];
		if (count > 0)
		{
			double inv = 1.0 / static_cast<double>(count);

			node.sigma_x *= inv;
			node.sigma_y *= inv;
			node.tau_xy *= inv;
			node.sigma_1 *= inv;
			node.sigma_2 *= inv;
			node.von_mises *= inv;
			node.max_shear *= inv;
			node.theta_p *= inv;
		}
	}

}




