#include "ipresult_extrapolator.h"

ipresult_extrapolator::ipresult_extrapolator(const std::unordered_map<int, polynomial_trielement_store>& polynomial_trielement_list,
	const std::unordered_map<int, polynomial_quadelement_store>& polynomial_quadelement_list, const int polynomial_order)
	: polynomial_trielement_list(polynomial_trielement_list),
	polynomial_quadelement_list(polynomial_quadelement_list),
	polynomial_order(polynomial_order)
{
	// Create element natural coordinates for extrapolation
	create_element_natural_coordinates();

	// Create interpolation weights for triangle and quadrilateral element at natural coordinates
	create_interpolation_weights_at_nodes();

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



void ipresult_extrapolator::create_interpolation_weights_at_nodes()
{

	//_________________________________________________________________________________________________________________________________
	// === TRIANGLE ELEMENTS ===
	create_tri_least_squares_extrapolation_weights();


	//_________________________________________________________________________________________________________________________________
	// === QUADRILATERAL ELEMENTS ===
	create_quad_least_squares_interpolation_weights();

}


void ipresult_extrapolator::create_tri_least_squares_extrapolation_weights()
{

	// Get the integration points for triangle elements based on the polynomial order
	std::vector<integration_point> tri_integration_points = integration_rules::get_tri_dunavant_points(this->polynomial_order);

	const int num_nodes_per_tri_element = ((this->polynomial_order + 1) * (this->polynomial_order + 2)) / 2;
	const int num_tri_integration_points = static_cast<int>(tri_integration_points.size());

	// For triangles, we use area coordinates (L1, L2, L3) or (xi, eta) with L3 = 1 - xi - eta
	const int poly_order = this->polynomial_order;
	const int num_terms = ((poly_order + 1) * (poly_order + 2)) / 2; // Complete 2D polynomial

	// Build polynomial basis matrix A (size: num_integration_points x num_polynomial_terms)
	Eigen::MatrixXd A(num_tri_integration_points, num_terms);
	A.setZero();


	for (int ip_idx = 0; ip_idx < num_tri_integration_points; ++ip_idx)
	{
		const auto& ip = tri_integration_points[ip_idx];
		double xi = ip.xi;      // L1
		double eta = ip.eta;    // L2
		double zeta = 1.0 - xi - eta; // L3

		// Complete polynomial basis in area coordinates
		// 1, xi, eta, xi², xi*eta, eta², xi³, xi²*eta, xi*eta², eta³, ...
		int term_idx = 0;
		for (int p = 0; p <= poly_order; ++p)
		{
			for (int q = 0; q <= p; ++q)
			{
				// Using (xi, eta) coordinates with zeta = 1 - xi - eta
				// The complete polynomial basis is: xi^(p-q) * eta^q
				A(ip_idx, term_idx++) = pow(xi, p - q) * pow(eta, q);
			}
		}
	}

	// Compute pseudo-inverse with regularization
	Eigen::MatrixXd ATA = A.transpose() * A;
	Eigen::MatrixXd I = Eigen::MatrixXd::Identity(num_terms, num_terms);
	double regularization_factor = 1e-10;

	Eigen::MatrixXd pseudo_inv = (ATA + regularization_factor * I).ldlt().solve(A.transpose());


	// Initialize interpolation weights
	std::vector<std::vector<double>> tri_interpolation_weights_at_nodes(
		num_nodes_per_tri_element,
		std::vector<double>(num_tri_integration_points, 0.0));

	for (int nd_id = 0; nd_id < num_nodes_per_tri_element; ++nd_id)
	{
		const std::pair<double, double>& node_nat_coords = tri_element_natural_coordinates[nd_id];
		double xi_n = node_nat_coords.first;
		double eta_n = node_nat_coords.second;
		double zeta_n = 1.0 - xi_n - eta_n;

		// Evaluate polynomial basis at node
		Eigen::RowVectorXd basis_at_node(num_terms);
		basis_at_node.setZero();

		int term_idx = 0;
		for (int p = 0; p <= poly_order; ++p)
		{
			for (int q = 0; q <= p; ++q)
			{
				basis_at_node[term_idx++] = pow(xi_n, p - q) * pow(eta_n, q);
			}
		}

		// Compute weights: W_row = basis * pseudo_inv
		Eigen::RowVectorXd W_row = basis_at_node * pseudo_inv;

		for (int ip = 0; ip < num_tri_integration_points; ++ip)
		{
			tri_interpolation_weights_at_nodes[nd_id][ip] = W_row(ip);
		}
	}


	tri_element_interpolation_weights.clear();
	tri_element_interpolation_weights = std::move(tri_interpolation_weights_at_nodes);

}
	

void ipresult_extrapolator::create_quad_least_squares_interpolation_weights()
{

	// Get the integration points for quadrilateral elements based on the polynomial order
	std::vector<integration_point> quad_integration_points = integration_rules::get_quad_gauss_points(this->polynomial_order);

	const int num_nodes_per_quad_element = (this->polynomial_order + 1) * (this->polynomial_order + 1);
	const int num_quad_integration_points = static_cast<int>(quad_integration_points.size());

	// Build polynomial basis matrix A (size: num_integration_points x num_polynomial_terms)
	// For 2D, use complete polynomial basis: 1, x, y, x², xy, y², ...
	const int poly_order = this->polynomial_order;
	const int num_terms = ((poly_order + 1) * (poly_order + 2)) / 2; // Complete 2D polynomial

	Eigen::MatrixXd A(num_quad_integration_points, num_terms);
	A.setZero();


	for (int ip_idx = 0; ip_idx < num_quad_integration_points; ++ip_idx)
	{
		const auto& ip = quad_integration_points[ip_idx];
		double xi = ip.xi;
		double eta = ip.eta;

		// Complete polynomial basis: 1, xi, eta, xi², xi*eta, eta², ...
		int term_idx = 0;
		for (int p = 0; p <= poly_order; ++p)
		{
			for (int q = 0; q <= p; ++q)
			{
				A(ip_idx, term_idx++) = pow(xi, p - q) * pow(eta, q);

			}
		}

	}

	// Compute pseudo-inverse: (A^T A)^-1 A^T
	// Compute pseudo-inverse with regularization
	Eigen::MatrixXd ATA = A.transpose() * A;
	Eigen::MatrixXd I = Eigen::MatrixXd::Identity(num_terms, num_terms);
	double regularization_factor = 1e-10; 

	// Use LDLT for symmetric positive definite matrix
	Eigen::MatrixXd pseudo_inv = (ATA + regularization_factor * I).ldlt().solve(A.transpose());


	// Initialize a 2D vector to store interpolation weights at nodes for each natural coordinates
	std::vector<std::vector<double>> quad_interpolation_weights_at_nodes(num_nodes_per_quad_element,
		std::vector<double>(num_quad_integration_points, 0.0));


	for (int nd_id = 0; nd_id < num_nodes_per_quad_element; ++nd_id)
	{
		const std::pair<double, double>& node_nat_coords = quad_element_natural_coordinates[nd_id];
		double xi_n = node_nat_coords.first;
		double eta_n = node_nat_coords.second;

		// Evaluate polynomial basis at node
		Eigen::RowVectorXd basis_at_node(num_terms);
		basis_at_node.setZero();

		int term_idx = 0;
		for (int p = 0; p <= poly_order; ++p) 
		{
			for (int q = 0; q <= p; ++q) 
			{
				basis_at_node[term_idx++] = pow(xi_n, p - q) * pow(eta_n, q);
			}
		}

		// Compute weights: W_row = basis * pseudo_inv
		Eigen::RowVectorXd W_row = basis_at_node * pseudo_inv;

		for (int ip = 0; ip < num_quad_integration_points; ++ip) 
		{
			quad_interpolation_weights_at_nodes[nd_id][ip] = W_row(ip);
		}
	}


	quad_element_interpolation_weights.clear();
	quad_element_interpolation_weights = std::move(quad_interpolation_weights_at_nodes);

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

			const std::vector<double>& interpolation_weights = tri_element_interpolation_weights[local_id];

			// Extrapolate results from integration points to the node
			for (int ip_idx = 0; ip_idx < static_cast<int>(tri_elm.results_at_ip.size()); ++ip_idx)
			{
				const element_results& ip_result = tri_elm.results_at_ip[ip_idx];
				double weight = interpolation_weights[ip_idx];
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

			const std::vector<double>& interpolation_weights = quad_element_interpolation_weights[local_id];

			// Extrapolate results from integration points to the node
			for (int ip_idx = 0; ip_idx < static_cast<int>(quad_elm.results_at_ip.size()); ++ip_idx)
			{
				const element_results& ip_result = quad_elm.results_at_ip[ip_idx];
				double weight = interpolation_weights[ip_idx];
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




