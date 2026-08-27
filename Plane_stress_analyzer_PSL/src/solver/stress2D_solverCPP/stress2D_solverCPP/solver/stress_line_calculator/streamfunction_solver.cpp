#include "streamfunction_solver.h"

streamfunction_solver::streamfunction_solver()
{
	// Empty constructor
}

void streamfunction_solver::compute_streamfunction(std::unordered_map<int, renderer_node>& renderer_node_points, 
	const std::vector<renderer_triangle>& renderer_triangles,
	bool _isTensionLine,
	void(*callback)(const char*))
{
	this->isTensionLine = _isTensionLine;
	this->m_callback = callback;

	// Create the node map id
	std::unordered_map<int, int> node_id_to_index;

	int node_index = 0;
	for (const auto& node_pair : renderer_node_points)
	{
		node_id_to_index[node_pair.first] = node_index;
		node_index++;
	}


	int num_nodes = static_cast<int>(renderer_node_points.size());

	Eigen::VectorXd globalFVector = Eigen::VectorXd::Zero(num_nodes);

	std::vector<Eigen::Triplet<double>> kTriplets;


	for (const auto& tri_element : renderer_triangles)
	{

		Eigen::Matrix3d elementK = getElementKMatrix(renderer_node_points, tri_element);
		Eigen::Vector3d elementF = getElementFVector(renderer_node_points, tri_element);

		std::array<int, 3> node_ids = { tri_element.n1, tri_element.n2, tri_element.n3 };

		std::array<int, 3> global_indices = {
			node_id_to_index[node_ids[0]],
			node_id_to_index[node_ids[1]],
			node_id_to_index[node_ids[2]]
		};


		for (int i = 0; i < 3; ++i)
		{

			// Add to the global F vector
			globalFVector(global_indices[i]) += elementF(i);

			for (int j = 0; j < 3; ++j)
			{
				// Add to the global K matrix using triplet format
				kTriplets.emplace_back(global_indices[i], global_indices[j], elementK(i, j));
			}
		}
	}

	Eigen::SparseMatrix<double> globalKMatrix(num_nodes, num_nodes);
	globalKMatrix.setFromTriplets(kTriplets.begin(), kTriplets.end());


	// Apply boundary conditions at the first streamfunction node (set to zero)

	// Create a reduced system by removing the first row and column (corresponding to the fixed node)
	Eigen::SparseMatrix<double> reducedKMatrix = globalKMatrix.block(1, 1, globalKMatrix.rows() - 1, globalKMatrix.cols() - 1);
	Eigen::VectorXd reducedFVector = globalFVector.segment(1, globalFVector.size() - 1);

	// Solve the reduced system
	// Eigen::SparseLU<Eigen::SparseMatrix<double>> solver;

	Eigen::SimplicialLDLT<Eigen::SparseMatrix<double>> solver;
	solver.compute(reducedKMatrix);

	if (solver.info() != Eigen::Success) 
	{
		// Decomposition failed
		report("Stress Lines Stream Function Decomposition failed");
		return;
	}

	Eigen::VectorXd reducedSolution = solver.solve(reducedFVector);

	if (solver.info() != Eigen::Success) 
	{
		// Solving failed
		report("Stress Lines Stream Function Solving failed");
		return;
	}


	// Construct the full solution vector
	Eigen::VectorXd fullSolution(globalFVector.size());
	fullSolution(0) = 0.0; // Boundary condition
	fullSolution.segment(1, reducedSolution.size()) = reducedSolution;


	// Normailze the solution to the range [0, 1]
	double maxStreamFunction = fullSolution.maxCoeff();
	double minStreamFunction = fullSolution.minCoeff();
	double streamFunctionRange = maxStreamFunction - minStreamFunction;

	//for (int i = 0; i < fullSolution.size(); ++i)
	//{
	//	fullSolution(i) = (fullSolution(i) - minStreamFunction) / streamFunctionRange;
	//}

	if (streamFunctionRange > 1e-9) // Avoid division by zero
	{
		fullSolution = (fullSolution.array() - minStreamFunction) / streamFunctionRange;
	}
	else
	{
		fullSolution.setConstant(0.5); // If the range is too small, set all values to 0.5
	}


	// Store the solution
	for (auto& node_pair : renderer_node_points)
	{
		int node_id = node_pair.first;
		int index = node_id_to_index[node_id];

		if (this->isTensionLine == true)
		{
			renderer_node_points[node_id].streamfunction_tension = fullSolution(index);
		}
		else
		{
			renderer_node_points[node_id].streamfunction_compression = fullSolution(index);
		}
	}

	//

}



Eigen::Matrix3d streamfunction_solver::getElementKMatrix(const std::unordered_map<int, renderer_node>& renderer_node_points, 
	const renderer_triangle& tri_element)
{

	// Get the node data for the triangle's vertices
	renderer_node node1 = renderer_node_points.at(tri_element.n1);
	renderer_node node2 = renderer_node_points.at(tri_element.n2);
	renderer_node node3 = renderer_node_points.at(tri_element.n3);

	// Get the node coordinates
	double p1_X = node1.x;
	double p1_Y = node1.y;

	double p2_X = node2.x;
	double p2_Y = node2.y;

	double p3_X = node3.x;
	double p3_Y = node3.y;



	double a1 = (p2_X * p3_Y) - (p3_X * p2_Y);
	double b1 = p2_Y - p3_Y;
	double c1 = p3_X - p2_X;

	double a2 = (p3_X * p1_Y) - (p1_X * p3_Y);
	double b2 = p3_Y - p1_Y;
	double c2 = p1_X - p3_X;

	double a3 = (p1_X * p2_Y) - (p2_X * p1_Y);
	double b3 = p1_Y - p2_Y;
	double c3 = p2_X - p1_X;

	double area = 0.5 * ((p2_X - p1_X) * (p3_Y - p1_Y) - (p3_X - p1_X) * (p2_Y - p1_Y));

	// Construct the K matrix
	double K11 = (b1 * b1 + c1 * c1) / (4 * area);
	double K12 = (b1 * b2 + c1 * c2) / (4 * area);
	double K13 = (b1 * b3 + c1 * c3) / (4 * area);

	double K21 = (b2 * b1 + c2 * c1) / (4 * area);
	double K22 = (b2 * b2 + c2 * c2) / (4 * area);
	double K23 = (b2 * b3 + c2 * c3) / (4 * area);

	double K31 = (b3 * b1 + c3 * c1) / (4 * area);
	double K32 = (b3 * b2 + c3 * c2) / (4 * area);
	double K33 = (b3 * b3 + c3 * c3) / (4 * area);


	Eigen::Matrix3d K;
	K << K11, K12, K13,
		K21, K22, K23,
		K31, K32, K33;

	return K;

}



Eigen::Vector3d streamfunction_solver::getElementFVector(const std::unordered_map<int, renderer_node>& renderer_node_points,
	const renderer_triangle& tri_element)
{

	// Get the node data for the triangle's vertices
	renderer_node node1 = renderer_node_points.at(tri_element.n1);
	renderer_node node2 = renderer_node_points.at(tri_element.n2);
	renderer_node node3 = renderer_node_points.at(tri_element.n3);

	// Get the node coordinates
	double p1_X = node1.x;
	double p1_Y = node1.y;

	double p2_X = node2.x;
	double p2_Y = node2.y;

	double p3_X = node3.x;
	double p3_Y = node3.y;



	double b1 = p2_Y - p3_Y;
	double c1 = p3_X - p2_X;

	double b2 = p3_Y - p1_Y;
	double c2 = p1_X - p3_X;

	double b3 = p1_Y - p2_Y;
	double c3 = p2_X - p1_X;


	double sigmaXX_avg = (node1.sigma_x + node2.sigma_x + node3.sigma_x) / 3.0;
	double sigmaYY_avg = (node1.sigma_y + node2.sigma_y + node3.sigma_y) / 3.0;
	double tauXY_avg = (node1.tau_xy + node2.tau_xy + node3.tau_xy) / 3.0;

	double sigma_diff = (sigmaXX_avg - sigmaYY_avg) / 2.0;
	double R = std::sqrt(sigma_diff * sigma_diff + tauXY_avg * tauXY_avg);

	// Degenerate/isotropic point (sigma_xx = sigma_yy, tau_xy = 0):
		  // principal direction is undefined. Element contributes nothing.
	if (R < 1e-9)
		return Eigen::Vector3d(0.0, 0.0, 0.0);

	double cos2theta = sigma_diff / R;                                   // clamp guards fp drift
	double costheta = std::sqrt(std::max(0.0, (1.0 + cos2theta) / 2.0));
	double sintheta = std::sqrt(std::max(0.0, (1.0 - cos2theta) / 2.0));
	if (tauXY_avg < 0.0) sintheta = -sintheta;

	double gx, gy; // = target ∇phi direction (unit vector)

	if (this->isTensionLine)
	{
		// grad(phi) = minor principal direction (perp to major/tension direction)
		gx = -sintheta;
		gy = costheta;
	}
	else
	{
		// grad(phi) = major principal direction (perp to minor/compression direction)
		gx = costheta;
		gy = sintheta;
	}


	// F_i = ∫ ∇N_i · (gx,gy) dA = (1/2)(b_i*gx + c_i*gy)   [since ∇N_i = (b_i,c_i)/(2A), integrated over area A]
	double F1 = 0.5 * (b1 * gx + c1 * gy);
	double F2 = 0.5 * (b2 * gx + c2 * gy);
	double F3 = 0.5 * (b3 * gx + c3 * gy);


	return Eigen::Vector3d(F1, F2, F3);
}




Eigen::Vector3d streamfunction_solver::getElementFVector_S1(const std::unordered_map<int, renderer_node>& renderer_node_points, 
	const renderer_triangle& tri_element)
{

	// Get the node data for the triangle's vertices
	renderer_node node1 = renderer_node_points.at(tri_element.n1);
	renderer_node node2 = renderer_node_points.at(tri_element.n2);
	renderer_node node3 = renderer_node_points.at(tri_element.n3);

	// Get the node coordinates
	double p1_X = node1.x;
	double p1_Y = node1.y;

	double p2_X = node2.x;
	double p2_Y = node2.y;

	double p3_X = node3.x;
	double p3_Y = node3.y;



	double b1 = p2_Y - p3_Y;
	double c1 = p3_X - p2_X;

	double b2 = p3_Y - p1_Y;
	double c2 = p1_X - p3_X;

	double b3 = p1_Y - p2_Y;
	double c3 = p2_X - p1_X;


	double sigmaXX_avg = (node1.sigma_x + node2.sigma_x + node3.sigma_x) / 3.0;
	double sigmaYY_avg = (node1.sigma_y + node2.sigma_y + node3.sigma_y) / 3.0;
	double tauXY_avg = (node1.tau_xy + node2.tau_xy + node3.tau_xy) / 3.0;

	double sigma_diff = (sigmaXX_avg - sigmaYY_avg) / 2.0;
	double R = std::sqrt(sigma_diff * sigma_diff + tauXY_avg * tauXY_avg);

	// Degenerate/isotropic point (sigma_xx = sigma_yy, tau_xy = 0):
		  // principal direction is undefined. Element contributes nothing.
	if (R < 1e-9)
		return Eigen::Vector3d(0.0, 0.0, 0.0);

	double cos2theta = sigma_diff / R;                                   // clamp guards fp drift
	double costheta = std::sqrt(std::max(0.0, (1.0 + cos2theta) / 2.0));
	double sintheta = std::sqrt(std::max(0.0, (1.0 - cos2theta) / 2.0));
	if (tauXY_avg < 0.0) sintheta = -sintheta;

	double gx, gy; // = target ∇phi direction (unit vector)

	if (this->isTensionLine)
	{
		// grad(phi) = minor principal direction (perp to major/tension direction)
		gx = -sintheta;
		gy = costheta;
	}
	else
	{
		// grad(phi) = major principal direction (perp to minor/compression direction)
		gx = costheta;
		gy = sintheta;
	}


	// F_i = ∫ ∇N_i · (gx,gy) dA = (1/2)(b_i*gx + c_i*gy)   [since ∇N_i = (b_i,c_i)/(2A), integrated over area A]
	double F1 = 0.5 * (b1 * gx + c1 * gy);
	double F2 = 0.5 * (b2 * gx + c2 * gy);
	double F3 = 0.5 * (b3 * gx + c3 * gy);


	return Eigen::Vector3d(F1, F2, F3);
}





void streamfunction_solver::report(const char* msg)
{

	std::string final_msg = std::string(msg) + " ";


	if (m_callback)
		m_callback(final_msg.c_str());
	//
}

