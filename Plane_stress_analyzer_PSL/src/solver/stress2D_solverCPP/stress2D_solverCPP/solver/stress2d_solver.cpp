#include "stress2d_solver.h"

stress2d_solver::stress2d_solver(int solver_type, int polynomial_order) :
	polynomial_order(polynomial_order), solver_type(solver_type), 
	quad_elem_formulation(polynomial_order), tri_elem_formulation(polynomial_order)
{
// Empty Constructor

}


bool stress2d_solver::initialize_solver(stress_system_store* stress_system, 
	const char* output_file_char,
	bool isSelfWeight, double accl_x, double accl_y,
	stopwatch_events* stopwatch, void(*callback)(const char*))
{

	// Set the stopwatch
	this->m_stopwatch = stopwatch;

	// Store callback locally
	this->m_callback = callback;


	// Store the output file name
	// CRITICAL: Copy the string to std::string for permanent storage
	this->output_file = std::string(output_file_char);


	std::string msg = "Output file set to: " + this->output_file;
	report(msg.c_str());

	this->isSelfWeight = isSelfWeight;

	if (this->isSelfWeight == true)
	{
		// Gravity load
		this->accl_x = accl_x;
		this->accl_y = accl_y;
	}


	report("Solver initialized successfully");

	polynomial_2dmesh.generate_2dpolynomial_mesh(stress_system, polynomial_order);

	if (polynomial_2dmesh.isPolynomialMeshCreated == true)
	{
		std::string rprt = "Linear solver mesh created";
		if (polynomial_order > 1)
		{
			rprt = "Higher order solver mesh created (order = " + std::to_string(polynomial_order) + ")";
		}
		report(rprt.c_str());

	}
	else
	{
		report("Failed to create solver mesh");

		return false;
	}

	return true;
}


bool stress2d_solver::perform_solve()
{

	this->numDOF = static_cast<int>(polynomial_2dmesh.polynomial_node_list.size()) * 2;
	
	this->trielement_dof = tri_elem_formulation.get_element_dof();
	this->quadelement_dof = quad_elem_formulation.get_element_dof();


	// Create the global support inclination matrix
	create_global_supportInclination_matrix();


	// Create the global stiffness matrix
	create_global_stiffness_matrix();

	report("Global stiffness matrix created");

	// Create the global load matrix
	create_global_load_vector();

	report("Global load vector created");


	if (this->isSelfWeight == true)
	{
		// Create the global mass matrix
		create_global_mass_matrix();

		report("Global mass matrix created");

		// Apply self-weight to the global load vector
		create_global_load_vector_self_weight();

		report("Self-weight applied to global load vector");
	}


	// Create the global boundary condition flag vector
	create_global_BC_flag_vector();

	report("Global boundary condition flag vector created");


	store_k_m_matrices_text_debug();


	// Perform the solve based on the selected method
	if (solver_type == 0)
	{
		// Elimination method
		solve_BCs_elimination_method();

		report("Solve complete using elimination method");
	}
	else if (solver_type == 1)
	{
		// Lagrange multiplier method
		solve_BCs_lagrange_method();

		report("Solve complete using Lagrange multiplier method");
	}
	

	// Find the global resultant forces
	this->global_reaction_vector = (this->global_stiffness_matrix * this->global_displacement_vector) - this->global_load_vector;


	// Transform the global displacement vector back to the original coordinate system
	this->global_displacement_vector = this->global_supportInclination_matrix * this->global_displacement_vector;
	this->global_reaction_vector = this->global_supportInclination_matrix * this->global_reaction_vector;

	// Check if the global displacement vector contains NaN or Inf values
	if (!check_valid_results(this->global_displacement_vector, "Global displacement vector"))
	{
		report("Error: Global displacement vector contains invalid values");
		return false;
	}

	if (!check_valid_results(this->global_reaction_vector, "Global reaction vector"))
	{
		report("Error: Global reaction vector contains invalid values");
		return false;
	}

	// Map the results back to the original node IDs and store them in the polynomial_2dmesh structure
	for (int i = 0; i < static_cast<int>(polynomial_2dmesh.renderer_node_points.size()); ++i)
	{
		polynomial_2dmesh.renderer_node_points[i].x_displ = this->global_displacement_vector(i * 2);
		polynomial_2dmesh.renderer_node_points[i].y_displ = this->global_displacement_vector(i * 2 + 1);
		// polynomial_2dmesh.renderer_node_points[i].reaction_x = this->global_reaction_vector(i * 2);
		// polynomial_2dmesh.renderer_node_points[i].reaction_y = this->global_reaction_vector(i * 2 + 1);
	}


	bool isResultStoreSuccessfully = store_results();

	return isResultStoreSuccessfully;

}



void stress2d_solver::create_global_supportInclination_matrix()
{
	// Global support inclination matrix
	this->global_supportInclination_matrix.resize(numDOF, numDOF);
	this->global_supportInclination_matrix.setZero();

	std::vector<Eigen::Triplet<double>> s_triplets;


	auto apply_support_inclination = [&](int node_id, double support_angle, std::vector<Eigen::Triplet<double>>& triplets)
		{
			// Convert angle to radians
			double support_angle_rad = (support_angle - 90.0) * M_PI / 180.0;
			double cos_theta = cos(support_angle_rad);
			double sin_theta = sin(support_angle_rad);

			int dof_u = (node_id * 2);     // u DOF
			int dof_v = (node_id * 2) + 1; // v DOF

			triplets.emplace_back(dof_u, dof_u, cos_theta);
			triplets.emplace_back(dof_u, dof_v, -sin_theta);
			triplets.emplace_back(dof_v, dof_u, sin_theta);
			triplets.emplace_back(dof_v, dof_v, cos_theta);
		};



	for (const auto& constraint : polynomial_2dmesh.get_constraint_data())
	{
		const constraint_store& constraint_data = constraint.second;

		for (const auto& node_id : constraint_data.node_ids)
		{
			apply_support_inclination(node_id, constraint_data.constraintangle, s_triplets);
		}
	}

	// Create the global support inclination matrix from triplets
	this->global_supportInclination_matrix.setFromTriplets(s_triplets.begin(), s_triplets.end());

}


void stress2d_solver::create_global_stiffness_matrix()
{

	// Global stiffness matrix
	this->global_stiffness_matrix.resize(numDOF, numDOF);
	this->global_stiffness_matrix.setZero();

	std::vector<Eigen::Triplet<double>> k_triplets;

	if (!polynomial_2dmesh.polynomial_trielement_list.empty())
	{
		// Triangle elements
		for (const auto& tri_elm_m : polynomial_2dmesh.polynomial_trielement_list)
		{
			// Get the element
			const polynomial_trielement_store& tri_elm = tri_elm_m.second;

			// Get the material
			const material_store& element_material = polynomial_2dmesh.get_material_data().at(tri_elm.materialid);

			std::vector<Eigen::Vector2d> node_coords;
			for (int nd_id : tri_elm.ordered_node_ids)
			{
				double x_coord = polynomial_2dmesh.polynomial_node_list[nd_id].x_coord;
				double y_coord = polynomial_2dmesh.polynomial_node_list[nd_id].y_coord;

				node_coords.push_back({x_coord, y_coord});
			}

			// Calculate the element stiffness matrix
			Eigen::MatrixXd element_stiffness_matrix = tri_elem_formulation.compute_trielement_stiffness_matrix(node_coords, element_material);

			// Assemble the global stiffness matrix
			assemble_element_matrix(tri_elm.ordered_node_ids, element_stiffness_matrix, k_triplets);
		}
	}


	if (!polynomial_2dmesh.polynomial_quadelement_list.empty())
	{
		// Quadrilateral elements
		for (const auto& quad_elm_m : polynomial_2dmesh.polynomial_quadelement_list)
		{
			// Get the element
			const polynomial_quadelement_store& quad_elm = quad_elm_m.second;

			// Get the material
			const material_store& element_material = polynomial_2dmesh.get_material_data().at(quad_elm.materialid);

			std::vector<Eigen::Vector2d> node_coords;
			for (int nd_id : quad_elm.ordered_node_ids)
			{
				double x_coord = polynomial_2dmesh.polynomial_node_list[nd_id].x_coord;
				double y_coord = polynomial_2dmesh.polynomial_node_list[nd_id].y_coord;
				node_coords.push_back({ x_coord, y_coord });
			}

			// Calculate the element stiffness matrix
			Eigen::MatrixXd element_stiffness_matrix = quad_elem_formulation.compute_quadelement_stiffness_matrix(node_coords, element_material);
			
			// Assemble the global stiffness matrix
			assemble_element_matrix(quad_elm.ordered_node_ids, element_stiffness_matrix, k_triplets);
		}
	}

	// Set the global stiffness matrix from triplets
	this->global_stiffness_matrix.setFromTriplets(k_triplets.begin(), k_triplets.end());


	// Apply transformation for support inclination to the global stiffness matrix
	this->global_stiffness_matrix = this->global_supportInclination_matrix.transpose() * 
		this->global_stiffness_matrix * this->global_supportInclination_matrix;

}


void stress2d_solver::create_global_load_vector()
{
	// Create the global load vector
	this->global_load_vector.resize(numDOF);
	this->global_load_vector.setZero();


	auto apply_load_to_global_vector = [&](int node_id, const load_store& load) 
		{
			// Resolve the load components from load amplitude and load angle
			double load_ampl = load.loadamplitude;
			double load_angle = load.loadangle; // in degrees

			// Convert angle to radians
			double load_angle_rad = load_angle * M_PI / 180.0;

			double fx = load_ampl * cos(load_angle_rad);
			double fy = load_ampl * sin(load_angle_rad);

			int dof_u = (node_id * 2);     // u DOF
			int dof_v = (node_id * 2) + 1; // v DOF

			this->global_load_vector[dof_u] += fx;
			this->global_load_vector[dof_v] += fy;
		};


	for (const auto& loads : polynomial_2dmesh.get_load_data())
	{
		const load_store& load = loads.second;
		// int load_set_id = load.load_set_id;

		for (const auto& node_id : load.node_ids)
		{
			// Apply the load to the global load vector
			apply_load_to_global_vector(node_id, load);
		}
	}


	// Apply transformation for support inclination to the global load vector
	this->global_load_vector = this->global_supportInclination_matrix * this->global_load_vector;

}



void stress2d_solver::create_global_mass_matrix()
{

	// Global mass matrix
	this->global_mass_matrix.resize(numDOF, numDOF);
	this->global_mass_matrix.setZero();

	std::vector<Eigen::Triplet<double>> m_triplets;

	if (!polynomial_2dmesh.polynomial_trielement_list.empty())
	{
		// Triangle elements
		for (const auto& tri_elm_m : polynomial_2dmesh.polynomial_trielement_list)
		{
			// Get the element
			const polynomial_trielement_store& tri_elm = tri_elm_m.second;

			// Get the material
			const material_store& element_material = polynomial_2dmesh.get_material_data().at(tri_elm.materialid);

			std::vector<Eigen::Vector2d> node_coords;
			for (int nd_id : tri_elm.ordered_node_ids)
			{
				double x_coord = polynomial_2dmesh.polynomial_node_list[nd_id].x_coord;
				double y_coord = polynomial_2dmesh.polynomial_node_list[nd_id].y_coord;

				node_coords.push_back({ x_coord, y_coord });
			}

			// Calculate the element mass matrix
			Eigen::MatrixXd element_mass_matrix = tri_elem_formulation.compute_trielement_mass_matrix(node_coords, element_material);

			// Assemble the global mass matrix
			assemble_element_matrix(tri_elm.ordered_node_ids, element_mass_matrix, m_triplets);
		}
	}

	if (!polynomial_2dmesh.polynomial_quadelement_list.empty())
	{
		// Quadrilateral elements
		for (const auto& quad_elm_m : polynomial_2dmesh.polynomial_quadelement_list)
		{
			// Get the element
			const polynomial_quadelement_store& quad_elm = quad_elm_m.second;

			// Get the material
			const material_store& element_material = polynomial_2dmesh.get_material_data().at(quad_elm.materialid);

			std::vector<Eigen::Vector2d> node_coords;
			for (int nd_id : quad_elm.ordered_node_ids)
			{
				double x_coord = polynomial_2dmesh.polynomial_node_list[nd_id].x_coord;
				double y_coord = polynomial_2dmesh.polynomial_node_list[nd_id].y_coord;
				node_coords.push_back({ x_coord, y_coord });
			}

			// Calculate the element mass matrix
			Eigen::MatrixXd element_mass_matrix = quad_elem_formulation.compute_quadelement_mass_matrix(node_coords, element_material);

			// Assemble the global mass matrix
			assemble_element_matrix(quad_elm.ordered_node_ids, element_mass_matrix, m_triplets);
		}
	}

	// Set the global mass matrix from triplets
	this->global_mass_matrix.setFromTriplets(m_triplets.begin(), m_triplets.end());

}


void stress2d_solver::create_global_load_vector_self_weight()
{
	// Create the global load vector for self-weight
	Eigen::VectorXd xy_acceleration(numDOF);
	Eigen::VectorXd self_weight_load_vector(numDOF);

	xy_acceleration.setZero();
	self_weight_load_vector.setZero();

	for (const auto& nodes : polynomial_2dmesh.polynomial_node_list)
	{
		const polynomial_node_store& node = nodes.second;
		int node_id = node.node_id;

		xy_acceleration[node_id * 2] = this->accl_x; // u DOF
		xy_acceleration[node_id * 2 + 1] = this->accl_y; // v DOF

	}

	// Calculate the self-weight load vector using the global mass matrix and acceleration vector
	self_weight_load_vector = this->global_mass_matrix * xy_acceleration;


	// Transform the self-weight load vector for support inclination
	self_weight_load_vector = this->global_supportInclination_matrix * self_weight_load_vector;


	// Add the self-weight load vector to the global load vector
	this->global_load_vector += self_weight_load_vector;

}


void stress2d_solver::create_global_BC_flag_vector()
{
	this->global_BC_flag_vector = Eigen::VectorXd::Zero(numDOF);

	auto apply_bc_flag = [&](int node_id, int constraint_type)
		{
			if (constraint_type == 0) // Pinned
			{
				this->global_BC_flag_vector[node_id * 2] = 1; // u DOF fixed
				this->global_BC_flag_vector[node_id * 2 + 1] = 1; // v DOF fixed

			}
			else if (constraint_type == 1) // Roller
			{
				this->global_BC_flag_vector[node_id * 2] = 0; // u DOF free
				this->global_BC_flag_vector[node_id * 2 + 1] = 1; // v DOF fixed
			}
		};


	for (const auto& constraint : polynomial_2dmesh.get_constraint_data())
	{
		const constraint_store& constraint_data = constraint.second;

		for (const auto& node_id : constraint_data.node_ids)
		{
			apply_bc_flag(node_id, constraint_data.constrainttype);
		}
	}


}


void stress2d_solver::assemble_element_matrix(const std::vector<int>& node_ids,
	const Eigen::MatrixXd& element_matrix, std::vector<Eigen::Triplet<double>>& triplets)
{
	const int num_nodes = static_cast<int>(node_ids.size());
	const int dof_per_node = 2;
	const int element_dof = num_nodes * dof_per_node;

	// Pre-compute global DOF indices for this element
	std::vector<int> global_dofs;
	global_dofs.reserve(element_dof);

	for (int node_id : node_ids) 
	{
		global_dofs.push_back(node_id * dof_per_node + 0);  // u DOF
		global_dofs.push_back(node_id * dof_per_node + 1);  // v DOF
	}

	// Add triplets
	for (int i = 0; i < element_dof; ++i) 
	{
		int global_i = global_dofs[i];

		for (int j = 0; j < element_dof; ++j) 
		{
			double value = element_matrix(i, j);

			if (std::abs(value) > 1e-15) 
			{  // Skip near-zero entries
				triplets.emplace_back(global_i, global_dofs[j], value);
			}
		}
	}
}


void  stress2d_solver::solve_BCs_elimination_method()
{
	// Solve using Elimination Method
	// Find the fixed and free nodes dof
	std::vector<int> free_dofs;
	std::vector<int> fixed_dofs;

	for (int i = 0; i < this->numDOF; ++i)
	{
		if (global_BC_flag_vector(i) == 1) // 1 = fixed, 0 = free
			fixed_dofs.push_back(i);
		else
			free_dofs.push_back(i);
	}

	// Map free DOF to Local indices
	std::unordered_map<int, int> free_map;

	for (int i = 0; i < static_cast<int>(free_dofs.size()); ++i)
	{
		free_map[free_dofs[i]] = i;
	}


	std::vector<Eigen::Triplet<double>> triplets_ff;

	for (int k = 0; k < static_cast<int>(global_stiffness_matrix.outerSize()); ++k)
	{
		for (Eigen::SparseMatrix<double>::InnerIterator it(global_stiffness_matrix, k); it; ++it)
		{
			int i = it.row();
			int j = it.col();

			// keep only free-free block 1 = Fixed, 0 = Free
			if (global_BC_flag_vector(i) == 0 &&  global_BC_flag_vector(j) == 0)
			{

				// Get the local indices of free nodes
				int ii = free_map[i];
				int jj = free_map[j];

				triplets_ff.emplace_back(ii, jj, it.value());
			}
		}
	}



	// Main stiffness matrix for free DOFs
	Eigen::SparseMatrix<double> K_ff(static_cast<int>(free_dofs.size()), static_cast<int>(free_dofs.size()));
	K_ff.setFromTriplets(triplets_ff.begin(), triplets_ff.end());


	Eigen::VectorXd F_f(static_cast<int>(free_dofs.size())); // Load vector for free DOFs

	for (int i = 0; i < static_cast<int>(free_dofs.size()); ++i)
	{
		F_f(i) = global_load_vector(free_dofs[i]);
	}



	// Perform solve
	Eigen::SparseLU<Eigen::SparseMatrix<double>> solver;
	solver.compute(K_ff);

	Eigen::VectorXd u_f = solver.solve(F_f);


	this->global_displacement_vector.resize(numDOF);
	this->global_displacement_vector.setZero();

	// Free DOF assign the result
	for (int i = 0; i < free_dofs.size(); ++i)
	{
		this->global_displacement_vector(free_dofs[i]) = u_f(i);
	}

	// Fixed DOF assign the zero displacement
	for (int i : fixed_dofs)
	{
		this->global_displacement_vector(i) = 0.0;
	}
	//
}




void stress2d_solver::solve_BCs_lagrange_method()
{
	// Lagrange Augmentation method
	// Find the fixed and free nodes dof
	std::vector<int> free_dofs;
	std::vector<int> fixed_dofs;

	for (int i = 0; i < this->numDOF; ++i)
	{
		if (global_BC_flag_vector(i) == 1) // 1 = fixed, 0 = free
			fixed_dofs.push_back(i);
		else
			free_dofs.push_back(i);
	}

	int aug_size = static_cast<int>(fixed_dofs.size());

	// Buld the Augmented K matrix
	 // Copy the original system to the left corner
	std::vector<Eigen::Triplet<double>> triplets_K_aug;


	for (int k = 0; k < static_cast<int>(global_stiffness_matrix.outerSize()); ++k)
	{
		for (Eigen::SparseMatrix<double>::InnerIterator it(global_stiffness_matrix, k); it; ++it)
		{
			int i = it.row();
			int j = it.col();

			triplets_K_aug.emplace_back(i, j, it.value());
		}
	}

	// Build constraint matrix C
	for (int j = 0; j < aug_size; ++j)
	{
		int dof = fixed_dofs[j];

		// C row j has 1 at DOF position
		triplets_K_aug.emplace_back(numDOF + j, dof, 1.0);
		triplets_K_aug.emplace_back(dof, numDOF + j, 1.0);
	}


	// Main augmented stiffness matrix 
	Eigen::SparseMatrix<double> K_Aug(numDOF + aug_size, numDOF + aug_size);
	K_Aug.setFromTriplets(triplets_K_aug.begin(), triplets_K_aug.end());


	// Build the augmented load vector
	Eigen::VectorXd F_Aug(numDOF + aug_size);
	F_Aug.setZero();

	F_Aug.head(numDOF) = global_load_vector;
	F_Aug.tail(aug_size).setZero(); // Lagrange multipliers are zero



	// Perform solve
	Eigen::SparseLU<Eigen::SparseMatrix<double>> solver;
	solver.compute(K_Aug);

	Eigen::VectorXd u_Aug = solver.solve(F_Aug);

	this->global_displacement_vector.resize(numDOF);
	this->global_displacement_vector.setZero();

	// Free DOF assign the result
	for (int i = 0; i < static_cast<int>(free_dofs.size()); ++i)
	{
		this->global_displacement_vector(free_dofs[i]) = u_Aug(free_dofs[i]);
	}

	// Fixed DOF assign the zero displacement
	for (int i : fixed_dofs)
	{
		this->global_displacement_vector(i) = 0.0;
	}
	//
}


bool stress2d_solver::check_valid_results(const Eigen::VectorXd& results, const std::string& result_name)
{
	// Check if the results contain NaN values
	for (int i = 0; i < results.size(); ++i)
	{
		if (std::isnan(results(i)))
		{
			std::string error_msg = "Error: " + result_name + " contains NaN values at index " + std::to_string(i);
			report(error_msg.c_str());
			return false;
		}

		if (std::isinf(results(i)))
		{
			std::string error_msg = "Error: " + result_name + " contains Inf values at index " + std::to_string(i);
			report(error_msg.c_str());
			return false;
		}
	}

	return true;
}



bool stress2d_solver::store_results()
{

	std::ofstream bin_file(this->output_file.c_str(), std::ios::binary);

	if (!bin_file.is_open())
	{
		std::string error_msg = "Failed to open output file: " + this->output_file;
		report(error_msg.c_str());
		return false;
		// throw std::runtime_error(error_msg);
	}


	int32_t node_points_count = static_cast<int32_t>(polynomial_2dmesh.renderer_node_points.size());
	bin_file.write(reinterpret_cast<const char*>(&node_points_count), sizeof(int32_t));

	// Write the nodes
	for (const auto& node_m : polynomial_2dmesh.renderer_node_points)
	{
		renderer_node node = node_m.second;

		int32_t nodeid = static_cast<int32_t>(node.n_id);
		// double rand_result = std::sin(node.x * 10.0) * std::cos(node.y * 10.0); // Random value between 0 and 1

		// retrive the results
		double displ_x = node.x_displ;
		double displ_y = node.y_displ;


		bin_file.write(reinterpret_cast<const char*>(&nodeid), sizeof(int32_t));
		bin_file.write(reinterpret_cast<const char*>(&node.x), sizeof(double));
		bin_file.write(reinterpret_cast<const char*>(&node.y), sizeof(double));
		bin_file.write(reinterpret_cast<const char*>(&displ_x), sizeof(double));
		bin_file.write(reinterpret_cast<const char*>(&displ_y), sizeof(double));

	}

	report("Results: Nodes written");

	int32_t edge_lines_count = static_cast<int32_t>(polynomial_2dmesh.renderer_edge_lines.size());
	bin_file.write(reinterpret_cast<const char*>(&edge_lines_count), sizeof(int32_t));

	// Write the edges
	for (const auto& edge : polynomial_2dmesh.renderer_edge_lines)
	{
		int32_t start_nodeid = static_cast<int32_t>(edge.nstart);
		int32_t end_nodeid = static_cast<int32_t>(edge.nend);

		bin_file.write(reinterpret_cast<const char*>(&start_nodeid), sizeof(int32_t));
		bin_file.write(reinterpret_cast<const char*>(&end_nodeid), sizeof(int32_t));
	}

	report("Results: Edges written");

	int32_t triangles_count = static_cast<int32_t>(polynomial_2dmesh.renderer_element_triangles.size());
	bin_file.write(reinterpret_cast<const char*>(&triangles_count), sizeof(int32_t));

	// Write the triangles
	for (const auto& tri : polynomial_2dmesh.renderer_element_triangles)
	{
		int32_t n1 = static_cast<int32_t>(tri.n1);
		int32_t n2 = static_cast<int32_t>(tri.n2);
		int32_t n3 = static_cast<int32_t>(tri.n3);

		bin_file.write(reinterpret_cast<const char*>(&n1), sizeof(int32_t));
		bin_file.write(reinterpret_cast<const char*>(&n2), sizeof(int32_t));
		bin_file.write(reinterpret_cast<const char*>(&n3), sizeof(int32_t));
	}

	report("Results: Triangles written");


	bin_file.flush();

	auto file_size = bin_file.tellp();  // tellp() for output file (tellg() is for input)

	bin_file.close();

	// Report Success and file size
	std::string success_msg = "Results stored successfully: " +
		this->output_file +
		" (" + std::to_string(node_points_count) + " nodes, " +
		std::to_string(triangles_count) + " triangles)";
	report(success_msg.c_str());

	return true;
	//
}



void stress2d_solver::store_k_m_matrices_text_debug()
{
	std::string text_file_name = "debug_matrices.txt";
	std::ofstream text_file(text_file_name);

	if (!text_file.is_open())
	{
		std::string error_msg = "Failed to open output file: " + text_file_name;
		report(error_msg.c_str());
		throw std::runtime_error(error_msg);
	}

	// Print the global K and M matrices
	// Only print 200 x 200, inform if the matrix size exceed 200 x 200

	text_file << "# Plane Stress Analysis Solver - Ke & Me matrix\n";
	text_file << "# Format: Debug Text Output\n";
	text_file << "# Generated: " << __DATE__ << " " << __TIME__ << "\n\n";

	int max_print_size = 200;
	int matrix_rows = global_stiffness_matrix.rows();
	int matrix_cols = global_stiffness_matrix.cols();

	// Write Ke Matrix
	text_file << "=== Stiffness Matrix ===\n";
	text_file << "Size: " << matrix_rows << " x " << matrix_cols << "\n";

	if (matrix_rows > max_print_size || matrix_cols > max_print_size)
	{
		text_file << "WARNING: Matrix size exceeds " << max_print_size
			<< " x " << max_print_size << ". Printing only the first "
			<< max_print_size << " x " << max_print_size << " block.\n\n";

		// Print only the top-left corner
		for (int i = 0; i < std::min(max_print_size, matrix_rows); i++)
		{
			for (int j = 0; j < std::min(max_print_size, matrix_cols); j++)
			{
				text_file << std::setw(15) << std::setprecision(6) << global_stiffness_matrix.coeff(i, j) << " ";
			}
			text_file << "\n";
		}
	}
	else
	{
		// Print full matrix
		for (int i = 0; i < matrix_rows; i++)
		{
			for (int j = 0; j < matrix_cols; j++)
			{
				text_file << std::setw(15) << std::setprecision(6) << global_stiffness_matrix.coeff(i, j) << " ";
			}
			text_file << "\n";
		}
	}
	text_file << "\n";

	if (isSelfWeight == true)
	{
		// Write Me Matrix
		text_file << "=== Mass Matrix ===\n";
		text_file << "Size: " << matrix_rows << " x " << matrix_cols << "\n";

		if (matrix_rows > max_print_size || matrix_cols > max_print_size)
		{
			text_file << "WARNING: Matrix size exceeds " << max_print_size
				<< " x " << max_print_size << ". Printing only the first "
				<< max_print_size << " x " << max_print_size << " block.\n\n";

			// Print only the top-left corner
			for (int i = 0; i < std::min(max_print_size, matrix_rows); i++)
			{
				for (int j = 0; j < std::min(max_print_size, matrix_cols); j++)
				{
					text_file << std::setw(15) << std::setprecision(6) << global_mass_matrix.coeff(i, j) << " ";
				}
				text_file << "\n";
			}
		}
		else
		{
			// Print full matrix
			for (int i = 0; i < matrix_rows; i++)
			{
				for (int j = 0; j < matrix_cols; j++)
				{
					text_file << std::setw(15) << std::setprecision(6) << global_mass_matrix.coeff(i, j) << " ";
				}
				text_file << "\n";
			}
		}
		text_file << "\n";
	}
	else
	{
		text_file << "Mass matrix not generated (self-weight not applied)\n\n";
	}

	

	// Optional: Print matrix statistics
	text_file << "=== Matrix Statistics ===\n";

	// K matrix statistics
	double k_min = 0, k_max = 0, k_sum = 0;
	int k_nonzero = 0;
	for (int k = 0; k < global_stiffness_matrix.outerSize(); ++k)
	{
		for (Eigen::SparseMatrix<double>::InnerIterator it(global_stiffness_matrix, k); it; ++it)
		{
			double val = it.value();
			if (k_nonzero == 0) {
				k_min = val;
				k_max = val;
			}
			k_min = std::min(k_min, val);
			k_max = std::max(k_max, val);
			k_sum += std::abs(val);
			k_nonzero++;
		}
	}

	text_file << "Ke (Stiffness) Matrix:\n";
	text_file << "  Non-zero entries: " << k_nonzero << "\n";
	text_file << "  Density: " << (100.0 * k_nonzero / (matrix_rows * matrix_cols)) << "%\n";
	text_file << "  Min value: " << k_min << "\n";
	text_file << "  Max value: " << k_max << "\n";
	text_file << "  Mean absolute value: " << (k_nonzero > 0 ? k_sum / k_nonzero : 0) << "\n\n";

	

	if (isSelfWeight == true)
	{
		// M matrix statistics
		double m_min = 0, m_max = 0, m_sum = 0;
		int m_nonzero = 0;
		for (int k = 0; k < global_mass_matrix.outerSize(); ++k)
		{
			for (Eigen::SparseMatrix<double>::InnerIterator it(global_mass_matrix, k); it; ++it)
			{
				double val = it.value();
				if (m_nonzero == 0) {
					m_min = val;
					m_max = val;
				}
				m_min = std::min(m_min, val);
				m_max = std::max(m_max, val);
				m_sum += std::abs(val);
				m_nonzero++;
			}
		}

		text_file << "Me (Mass) Matrix:\n";
		text_file << "  Non-zero entries: " << m_nonzero << "\n";
		text_file << "  Density: " << (100.0 * m_nonzero / (matrix_rows * matrix_cols)) << "%\n";
		text_file << "  Min value: " << m_min << "\n";
		text_file << "  Max value: " << m_max << "\n";
		text_file << "  Mean absolute value: " << (m_nonzero > 0 ? m_sum / m_nonzero : 0) << "\n\n";

		bool m_symmetric = global_mass_matrix.isApprox(global_mass_matrix.transpose());
		text_file << "Me is symmetric: " << (m_symmetric ? "YES" : "NO") << "\n";
	}

	// Check for symmetry
	bool k_symmetric = global_stiffness_matrix.isApprox(global_stiffness_matrix.transpose());
	

	// text_file << "=== Matrix Properties ===\n";
	text_file << "Ke is symmetric: " << (k_symmetric ? "YES" : "NO") << "\n";
	


	// Print the global load vector
	text_file << "\n=== Load Vector ===\n";

	matrix_rows = global_load_vector.rows();

	for (int i = 0; i < std::min(max_print_size, matrix_rows); i++)
	{
		text_file << std::setw(15) << std::setprecision(6) << global_load_vector(i) << "\n";
	}


	// Print the global BC flag vector
	text_file << "\n=== BC Flag Vector ===\n";

	matrix_rows = global_BC_flag_vector.rows();

	for (int i = 0; i < std::min(max_print_size, matrix_rows); i++)
	{
		text_file << std::setw(15) << std::setprecision(6) << global_BC_flag_vector(i) << "\n";
	}




	text_file.close();

	std::string msg = "Debug matrices written to: " + text_file_name;
	report(msg.c_str());

}




void stress2d_solver::report(const char* msg)
{
	std::stringstream stopwatch_elapsed_str;

	stopwatch_elapsed_str << std::fixed << std::setprecision(6)
		<< this->m_stopwatch->elapsed();

	std::string final_msg = std::string(msg) + " " +
		stopwatch_elapsed_str.str() +
		" secs";

	if (m_callback)
		m_callback(final_msg.c_str());
	//
}

