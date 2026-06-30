#include "polynomial_2dmesh_store.h"

polynomial_2dmesh_store::polynomial_2dmesh_store()
{
	// Empty constructor

}


void polynomial_2dmesh_store::generate_2dpolynomial_mesh(stress_system_store* stress_system, int polynomial_order)
{
	// Generate 2d polynomial mesh
	this->stress_system = std::move(*stress_system);


	this->polynomial_order = polynomial_order;

	if (this->polynomial_order < 1 || this->polynomial_order >4)
	{
		this->isPolynomialMeshCreated = false;
		return;
	}

	// Copy the existing mesh nodes and edges
	copy_mesh_nodes();

	// Create the edges interior nodes
	create_edge_internal_nodes();

	// Create triangle and quadrilateral elements
	create_polynomial_tri_elements();
	create_polynomial_quad_elements();

	// Create the renderer mesh
	create_renderer_mesh();


	this->isPolynomialMeshCreated = true;

}


void polynomial_2dmesh_store::copy_mesh_nodes()
{
	this->polynomial_node_list.clear();
	this->polynomial_edge_list.clear();
	this->polynomial_trielement_list.clear();
	this->polynomial_quadelement_list.clear();

	// Copy the existing node to the polynomial node system
	for (const auto& nd : this->stress_system.node_list)
	{
		polynomial_node_store p_node;
		p_node.node_id = nd.second.node_id;
		p_node.x_coord = nd.second.x_coord;
		p_node.y_coord = nd.second.y_coord;
		p_node.is_internal = false;

		this->polynomial_node_list.insert({ nd.first, p_node });
	}

	// Copy the existing edge to the polynomial edge system
	for (const auto& edge : this->stress_system.edge_list)
	{
		polynomial_edge_store p_edge;
		p_edge.edge_id = edge.second.edge_id;
		p_edge.startnodeid = edge.second.startnodeid;
		p_edge.endnodeid = edge.second.endnodeid;
		p_edge.leftfaceid = edge.second.leftfaceid;
		p_edge.rightfaceid = edge.second.rightfaceid;
		p_edge.edge_internal_node_ids.clear();

		this->polynomial_edge_list.insert({ edge.first, p_edge });
	}
}


void polynomial_2dmesh_store::create_edge_internal_nodes()
{

	// Node spacing based on the polynomial order
	std::vector<double> node_spacing;

	if (this->polynomial_order == 1)
	{
		// p = 1 (Linear / Bilinear)
		// No change - T3 + Q4
		return;

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


	// Start node ID from existing nodes
	int node_id = static_cast<int>(this->stress_system.node_list.size());

	// Lambda to create edge internal nodes
	auto create_edgenode = [&](double x_coord, double y_coord) -> int
		{
			polynomial_node_store p_node;
			p_node.node_id = node_id;
			p_node.x_coord = x_coord;
			p_node.y_coord = y_coord;
			p_node.is_internal = true;

			this->polynomial_node_list.insert({ node_id, p_node });
			return node_id++;
		};



	// Create the edges internal nodes
	for (auto& p_edge : this->polynomial_edge_list)
	{
		const polynomial_node_store& start_node = this->polynomial_node_list[p_edge.second.startnodeid];
		const polynomial_node_store& end_node = this->polynomial_node_list[p_edge.second.endnodeid];

		p_edge.second.edge_internal_node_ids.clear();

		for (const auto& nd_space : node_spacing)
		{
			double x_coord = start_node.x_coord * (1.0 - nd_space) + end_node.x_coord * (nd_space);
			double y_coord = start_node.y_coord * (1.0 - nd_space) + end_node.y_coord * (nd_space);

			int added_node_id = create_edgenode(x_coord, y_coord);

			p_edge.second.edge_internal_node_ids.push_back(added_node_id);
		}
	}
	//
}

std::vector<int> polynomial_2dmesh_store::get_ordered_edge_internal_nodes(const polynomial_edge_store& p_edge, int element_id)
{
	// Copy the internal node ids
	std::vector<int> edge_nodes = p_edge.edge_internal_node_ids;

	if (p_edge.leftfaceid == element_id)
	{
		// no need to reverse the direction
	}
	else
	{
		// Reverse the edge nodes to maintain consistent orientation
		std::reverse(edge_nodes.begin(), edge_nodes.end());
	}

	return edge_nodes;
}


void polynomial_2dmesh_store::create_polynomial_tri_elements()
{
	// Create triangle elements based on polynomial order
	for (const auto& tri : this->stress_system.trielement_list)
	{
		polynomial_trielement_store p_tri;
		p_tri.tri_id = tri.second.tri_id;
		p_tri.materialid = tri.second.materialid;

		// Get the 3 corner nodes
		int nd1 = tri.second.nodeid1;
		int nd2 = tri.second.nodeid2;
		int nd3 = tri.second.nodeid3;

		p_tri.corner_nodes = { nd1, nd2, nd3 };

		// Get edge IDs
		int edge1_id = get_edge_id(nd1, nd2); // Edge 1: nd1 -> nd2
		int edge2_id = get_edge_id(nd2, nd3); // Edge 2: nd2 -> nd3
		int edge3_id = get_edge_id(nd3, nd1); // Edge 3: nd3 -> nd1

		// Get edge internal nodes from polynomial edge list
		const polynomial_edge_store& p_edge1 = this->polynomial_edge_list[edge1_id];
		const polynomial_edge_store& p_edge2 = this->polynomial_edge_list[edge2_id];
		const polynomial_edge_store& p_edge3 = this->polynomial_edge_list[edge3_id];

		// Get ordered edge nodes
		std::vector<int> edge1_nodes = get_ordered_edge_internal_nodes(p_edge1, tri.second.tri_id);
		std::vector<int> edge2_nodes = get_ordered_edge_internal_nodes(p_edge1, tri.second.tri_id);
		std::vector<int> edge3_nodes = get_ordered_edge_internal_nodes(p_edge1, tri.second.tri_id);

		// Store edge nodes
		p_tri.edge_node_ids[0] = std::move(edge1_nodes);
		p_tri.edge_node_ids[1] = std::move(edge2_nodes);
		p_tri.edge_node_ids[2] = std::move(edge3_nodes);

		// Create internal nodes for triangle (if polynomial_order >= 3)
		p_tri.internal_nodes = create_tri_internal_nodes(nd1, nd2, nd3);

		// create the ordered nodes
		p_tri.ordered_node_ids.clear();

		// Add corners (CCW)
		p_tri.ordered_node_ids.insert(p_tri.ordered_node_ids.end(), p_tri.corner_nodes.begin(), p_tri.corner_nodes.end());

		// Add edge nodes (CCW)
		// Bottom edge
		p_tri.ordered_node_ids.insert(p_tri.ordered_node_ids.end(),
			p_tri.edge_node_ids[0].begin(),
			p_tri.edge_node_ids[0].end());

		// Right edge
		p_tri.ordered_node_ids.insert(p_tri.ordered_node_ids.end(),
			p_tri.edge_node_ids[1].begin(),
			p_tri.edge_node_ids[1].end());

		// Left edge
		p_tri.ordered_node_ids.insert(p_tri.ordered_node_ids.end(),
			p_tri.edge_node_ids[2].begin(),
			p_tri.edge_node_ids[2].end());

		// Add internal nodes
		p_tri.ordered_node_ids.insert(p_tri.ordered_node_ids.end(),
			p_tri.internal_nodes.begin(),
			p_tri.internal_nodes.end());


		this->polynomial_trielement_list.insert({ tri.first, p_tri });
	}

}


void polynomial_2dmesh_store::create_polynomial_quad_elements()
{
	for (const auto& quad : this->stress_system.quadelement_list)
	{
		polynomial_quadelement_store p_quad;
		p_quad.quad_id = quad.second.quad_id;
		p_quad.materialid = quad.second.materialid;

		int nd1 = quad.second.nodeid1;
		int nd2 = quad.second.nodeid2;
		int nd3 = quad.second.nodeid3;
		int nd4 = quad.second.nodeid4;

		p_quad.corner_nodes = { nd1, nd2, nd3, nd4 };

		// Get edge IDs (in counter-clockwise order)
		int edge1_id = get_edge_id(nd1, nd2);  // Bottom edge
		int edge2_id = get_edge_id(nd2, nd3);  // Right edge
		int edge3_id = get_edge_id(nd3, nd4);  // Top edge
		int edge4_id = get_edge_id(nd4, nd1);  // Left edge

		// Get edge nodes with correct orientation
		const polynomial_edge_store& p_edge1 = this->polynomial_edge_list[edge1_id];
		const polynomial_edge_store& p_edge2 = this->polynomial_edge_list[edge2_id];
		const polynomial_edge_store& p_edge3 = this->polynomial_edge_list[edge3_id];
		const polynomial_edge_store& p_edge4 = this->polynomial_edge_list[edge4_id];

		std::vector<int> edge1_nodes = get_ordered_edge_internal_nodes(p_edge1, quad.second.quad_id);
		std::vector<int> edge2_nodes = get_ordered_edge_internal_nodes(p_edge2, quad.second.quad_id);
		std::vector<int> edge3_nodes = get_ordered_edge_internal_nodes(p_edge3, quad.second.quad_id);
		std::vector<int> edge4_nodes = get_ordered_edge_internal_nodes(p_edge4, quad.second.quad_id);

		p_quad.edge_node_ids[0] = std::move(edge1_nodes);
		p_quad.edge_node_ids[1] = std::move(edge2_nodes);
		p_quad.edge_node_ids[2] = std::move(edge3_nodes);
		p_quad.edge_node_ids[3] = std::move(edge4_nodes);

		// Create internal nodes
		p_quad.internal_nodes = create_quad_internal_nodes(nd1, nd2, nd3, nd4);


		// create the ordered nodes
		p_quad.ordered_node_ids.clear();

		// Add corners (CCW)
		p_quad.ordered_node_ids.insert(p_quad.ordered_node_ids.end(), p_quad.corner_nodes.begin(), p_quad.corner_nodes.end());

		// Add edge nodes (CCW)
		// Bottom edge
		p_quad.ordered_node_ids.insert(p_quad.ordered_node_ids.end(),
			p_quad.edge_node_ids[0].begin(),
			p_quad.edge_node_ids[0].end());

		// Right edge
		p_quad.ordered_node_ids.insert(p_quad.ordered_node_ids.end(),
			p_quad.edge_node_ids[1].begin(),
			p_quad.edge_node_ids[1].end());

		// Top edge
		p_quad.ordered_node_ids.insert(p_quad.ordered_node_ids.end(),
			p_quad.edge_node_ids[2].begin(),
			p_quad.edge_node_ids[2].end());

		// Left edge
		p_quad.ordered_node_ids.insert(p_quad.ordered_node_ids.end(),
			p_quad.edge_node_ids[3].begin(),
			p_quad.edge_node_ids[3].end());

		// Add internal nodes
		p_quad.ordered_node_ids.insert(p_quad.ordered_node_ids.end(),
			p_quad.internal_nodes.begin(),
			p_quad.internal_nodes.end());

		this->polynomial_quadelement_list.insert({ quad.first, p_quad });
	}

}


std::vector<int> polynomial_2dmesh_store::create_tri_internal_nodes(int nd1, int nd2, int nd3)
{
	std::vector<int> internal_nodes;

	if (this->polynomial_order < 3)
	{
		// T3 (p=1) and T6 (p=2) have no internal nodes
		return internal_nodes;
	}


	// Get corner node coordinates
	const polynomial_node_store& node1 = this->polynomial_node_list[nd1];
	const polynomial_node_store& node2 = this->polynomial_node_list[nd2];
	const polynomial_node_store& node3 = this->polynomial_node_list[nd3];

	// Start node ID from existing nodes
	int node_id = static_cast<int>(this->polynomial_node_list.size());

	auto create_internal_node = [&](double x, double y) -> int
		{
			polynomial_node_store p_node;
			p_node.node_id = node_id;
			p_node.x_coord = x;
			p_node.y_coord = y;
			p_node.is_internal = true;

			this->polynomial_node_list.insert({ node_id, p_node });
			return node_id++;
		};


	if (this->polynomial_order == 3)
	{
		// T10: 1 internal node at the centroid
		double center_x = (node1.x_coord + node2.x_coord + node3.x_coord) / 3.0;
		double center_y = (node1.y_coord + node2.y_coord + node3.y_coord) / 3.0;
		internal_nodes.push_back(create_internal_node(center_x, center_y));
	}
	else if (this->polynomial_order == 4)
	{
		// T15: 3 internal nodes
		// Using area coordinates: (1/3, 1/3, 1/3) and variations
		std::vector<std::pair<double, double>> internal_points =
		{
			{1.0 / 3.0, 1.0 / 3.0},
			{1.0 / 6.0, 1.0 / 6.0},
			{2.0 / 3.0, 1.0 / 6.0}
		};

		for (const auto& [alpha, beta] : internal_points)
		{
			double gamma = 1.0 - alpha - beta;
			double x = alpha * node1.x_coord + beta * node2.x_coord + gamma * node3.x_coord;
			double y = alpha * node1.y_coord + beta * node2.y_coord + gamma * node3.y_coord;
			internal_nodes.push_back(create_internal_node(x, y));
		}
	}

	return internal_nodes;
}


std::vector<int> polynomial_2dmesh_store::create_quad_internal_nodes(int nd1, int nd2, int nd3, int nd4)
{
	std::vector<int> internal_nodes;

	if (this->polynomial_order < 2)
	{
		// Q4 (p=1) has no internal nodes
		return internal_nodes;
	}

	// Get corner node coordinates
	const polynomial_node_store& node1 = this->polynomial_node_list[nd1];
	const polynomial_node_store& node2 = this->polynomial_node_list[nd2];
	const polynomial_node_store& node3 = this->polynomial_node_list[nd3];
	const polynomial_node_store& node4 = this->polynomial_node_list[nd4];

	// Start node ID from existing nodes
	int node_id = static_cast<int>(this->polynomial_node_list.size());

	auto create_internal_node = [&](double x, double y) -> int
		{
			polynomial_node_store p_node;
			p_node.node_id = node_id;
			p_node.x_coord = x;
			p_node.y_coord = y;
			p_node.is_internal = true;

			this->polynomial_node_list.insert({ node_id, p_node });
			return node_id++;
		};

	if (this->polynomial_order == 2)
	{
		// Q9: 1 center node
		double center_x = (node1.x_coord + node2.x_coord + node3.x_coord + node4.x_coord) * 0.25;
		double center_y = (node1.y_coord + node2.y_coord + node3.y_coord + node4.y_coord) * 0.25;
		internal_nodes.push_back(create_internal_node(center_x, center_y));
	}
	else if (this->polynomial_order == 3)
	{
		// Q16: 4 internal nodes (2x2 grid)
		double xi[4] = { -0.5, 0.5, -0.5, 0.5 };
		double eta[4] = { -0.5, -0.5, 0.5, 0.5 };

		// Map from natural coordinates (-1,1) to physical coordinates
		for (int i = 0; i < 4; i++)
		{
			double N1 = 0.25 * (1 - xi[i]) * (1 - eta[i]);
			double N2 = 0.25 * (1 + xi[i]) * (1 - eta[i]);
			double N3 = 0.25 * (1 + xi[i]) * (1 + eta[i]);
			double N4 = 0.25 * (1 - xi[i]) * (1 + eta[i]);

			double x = (N1 * node1.x_coord) + (N2 * node2.x_coord) + (N3 * node3.x_coord) + (N4 * node4.x_coord);
			double y = (N1 * node1.y_coord) + (N2 * node2.y_coord) + (N3 * node3.y_coord) + (N4 * node4.y_coord);

			internal_nodes.push_back(create_internal_node(x, y));
		}
	}
	else if (this->polynomial_order == 4)
	{
		// Q25: 9 internal nodes (3x3 grid)
		double xi[9] = { -0.666, 0.0, 0.666, -0.666, 0.0, 0.666, -0.666, 0.0, 0.666 };
		double eta[9] = { -0.666, -0.666, -0.666, 0.0, 0.0, 0.0, 0.666, 0.666, 0.666 };

		for (int i = 0; i < 9; i++)
		{
			double N1 = 0.25 * (1 - xi[i]) * (1 - eta[i]);
			double N2 = 0.25 * (1 + xi[i]) * (1 - eta[i]);
			double N3 = 0.25 * (1 + xi[i]) * (1 + eta[i]);
			double N4 = 0.25 * (1 - xi[i]) * (1 + eta[i]);

			double x = (N1 * node1.x_coord) + (N2 * node2.x_coord) + (N3 * node3.x_coord) + (N4 * node4.x_coord);
			double y = (N1 * node1.y_coord) + (N2 * node2.y_coord) + (N3 * node3.y_coord) + (N4 * node4.y_coord);

			internal_nodes.push_back(create_internal_node(x, y));
		}
	}

	return internal_nodes;
}



int polynomial_2dmesh_store::get_edge_id(const int& startnodeid, const int& endnodeid)
{
	// Get the connected edges to start node
	const std::vector<int>& connected_edges = this->stress_system.node_edge_map[startnodeid];

	for (const int& edge_id : connected_edges)
	{
		const auto& edge = this->polynomial_edge_list[edge_id];
		if ((edge.startnodeid == startnodeid && edge.endnodeid == endnodeid) ||
			(edge.startnodeid == endnodeid && edge.endnodeid == startnodeid))
		{
			// Line with the same start and end nodes
			return edge_id;
		}

	}

	return -1;
}




void polynomial_2dmesh_store::create_renderer_mesh()
{
	this->renderer_node_points.clear();

	// Copy the polynomial node store to renderer node
	for (const auto& nd : this->polynomial_node_list)
	{
		renderer_node rd_node;
		rd_node.n_id = nd.second.node_id;
		rd_node.x = nd.second.x_coord;
		rd_node.y = nd.second.y_coord;

		// Null Result data
		rd_node.r1 = 0.0;
		rd_node.r2 = 0.0;
		rd_node.r3 = 0.0;
		rd_node.r4 = 0.0;

		this->renderer_node_points.insert({ nd.second.node_id, rd_node });
	}


	// Create the renderer triangles
	this->renderer_element_triangles.clear();

	create_trimesh_renderer_elements();

	create_quadmesh_renderer_elements();

	// Create the rendere element edges
	this->renderer_edge_lines.clear();
	create_renderer_edges();

}



void polynomial_2dmesh_store::create_trimesh_renderer_elements()
{

	auto create_trielement_renderer_triangles = [&](const std::vector<int>& layer_0, const std::vector<int>& layer_1) -> void
		{
			// layer_1    c -- d
			//            |    |
			// layer_0    a -- b

			int count = static_cast<int>(layer_0.size());

			for (int i = 0; i < count - 1; i++)
			{
				// First triangle: (a, b, c)
				renderer_triangle tri1{
					layer_0[i],
					layer_0[i + 1],
					layer_1[i]
				};
				this->renderer_element_triangles.push_back(tri1);

				// Second triangle: (b, d, c)
				if (i < count - 2)
				{
					renderer_triangle tri2{
						layer_0[i + 1],
						layer_1[i + 1],
						layer_1[i]
					};
					this->renderer_element_triangles.push_back(tri2);
				}
			}
			//
		};


	int order = this->polynomial_order; // 1 = T3, 2 = T6, 3 = T10, 4 = T15

	// Process triangle elements
	for (const auto& p_tri_m : this->polynomial_trielement_list)
	{

		// Example T15 Node ordering
		// 3 nodes per edge + 3 internal nodes
		//         3
		//        / \
		//       10  9
		//      /     \
		//     11  15  8
		//    /         \
		//   12  13  14  7
		//  /             \
		// 1---4---5---6---2

		// Node	r(ξ)	s(η)	Type
		// 1	0		0		Corner
		// 2	1		0		Corner
		// 3	0		1		Corner
		// 4	1/4		0		Edge 1
		// 5	1/2		0		Edge 1
		// 6	3/4		0		Edge 1
		// 7	3/4		1/4		Edge 2
		// 8	1/2		1/2		Edge 2
		// 9	1/4		3/4		Edge 2
		// 10	0		3/4		Edge 3
		// 11	0		1/2		Edge 3
		// 12	0		1/4		Edge 3
		// 13	1/4		1/4		Internal
		// 14	1/2		1/4		Internal
		// 15	1/4		1/2		Internal

		const polynomial_trielement_store& p_tri = p_tri_m.second;

		// Set the first layer nodes
		std::vector<int> layer_0_nodes;
		std::vector<int> layer_1_nodes;

		// Layer 0: Corner 0 -> Edge 0 nodes -> Corner 1
		layer_0_nodes.clear();
		layer_0_nodes.push_back(p_tri.corner_nodes[0]);

		for (const auto& edge0_id : p_tri.edge_node_ids[0])
		{
			// First edge nodes (Node 0 -> Node 1) [Edge 0]
			layer_0_nodes.push_back(edge0_id);
		}

		layer_0_nodes.push_back(p_tri.corner_nodes[1]);


		// Process internal layers
		int interior_node_index = 0;

		for (int i = 1; i < order; i++)
		{
			layer_1_nodes.clear();

			// Start is edge node (Node 2 -> Node 0) [Edge 2]
			layer_1_nodes.push_back(p_tri.edge_node_ids[2][order - i - 1]);

			// Interior nodes layer 1
			for (int j = 0; j < order - i - 1; j++)
			{
				layer_1_nodes.push_back(p_tri.internal_nodes[interior_node_index]);
				interior_node_index++;
			}

			// End is edge node (Node 1 -> Node 2) [Edge 1]
			layer_1_nodes.push_back(p_tri.edge_node_ids[1][i - 1]);

			// Using layer_0 and layer 1 create the triangles
			create_trielement_renderer_triangles(layer_0_nodes, layer_1_nodes);

			layer_0_nodes = layer_1_nodes;
			//
		}


		// Final layer is the final corner node Corner 2
		layer_1_nodes.clear();
		layer_1_nodes.push_back(p_tri.corner_nodes[2]);

		// Create final triangles
		create_trielement_renderer_triangles(layer_0_nodes, layer_1_nodes);

	}
	//
}



void polynomial_2dmesh_store::create_quadmesh_renderer_elements()
{

	auto create_quadelement_renderer_triangles = [&](const std::vector<int>& layer_0, const std::vector<int>& layer_1) -> void
		{
			// layer_1    c -- d
			//            |    |
			// layer_0    a -- b

			int count = static_cast<int>(layer_0.size());

			for (int i = 0; i < count - 1; i++)
			{
				// First triangle: (a, b, c)
				renderer_triangle tri1{
					layer_0[i],
					layer_0[i + 1],
					layer_1[i]
				};
				this->renderer_element_triangles.push_back(tri1);

				// Second triangle: (b, d, c)
				renderer_triangle tri2{
					layer_0[i + 1],
					layer_1[i + 1],
					layer_1[i]
				};
				this->renderer_element_triangles.push_back(tri2);
			}
			//
		};


	int order = this->polynomial_order; // 1 = Q4, 2 = Q9, 3 = Q16, 4 = Q25

	for (const auto& p_quad_m : this->polynomial_quadelement_list)
	{
		// Example Q25 Node ordering
		// 3 nodes per edge + 9 internal nodes
		// 4---13--12--11--3
		// |               |
		// 14  23  24  25  10
		// |               |
		// 15  20  21  22  9
		// |               |
		// 16  17  18  19  8 
		// |               |
		// 1---5---6---7---2

		// Node	ξ		η		Type
		// 1   -1		-1		Corner
		// 2	1		-1		Corner
		// 3	1		1		Corner
		// 4   -1		1		Corner
		// 5  -0.5		-1		Edge 1 (bottom)
		// 6	0		-1		Edge 1 (bottom)
		// 7	0.5		-1		Edge 1 (bottom)
		// 8	1		-0.5	Edge 2 (right)
		// 9	1		0		Edge 2 (right)
		// 10	1		0.5		Edge 2 (right)
		// 11	0.5		1		Edge 3 (top)
		// 12	0		1		Edge 3 (top)
		// 13  -0.5		1		Edge 3 (top)
		// 14  -1		0.5		Edge 4 (left)
		// 15  -1		0		Edge 4 (left)
		// 16  -1		-0.5	Edge 4 (left)
		// 17 - 0.5		-0.5	Internal
		// 18	0		-0.5	Internal
		// 19	0.5		-0.5	Internal
		// 20  -0.5		0		Internal
		// 21	0		0		Internal
		// 22	0.5		0		Internal
		// 23  -0.5		0.5		Internal
		// 24	0		0.5		Internal
		// 25	0.5		0.5		Internal

		int rd_tri_id = p_quad_m.second.quad_id;
		polynomial_quadelement_store p_quad = p_quad_m.second;

		// Set the first layer nodes
		std::vector<int> layer_0_nodes;
		std::vector<int> layer_1_nodes;

		// Layer 0: Corner 0 -> Edge 0 nodes -> Corner 1
		layer_0_nodes.clear();
		layer_0_nodes.push_back(p_quad.corner_nodes[0]);

		for (const auto& edge0_id : p_quad.edge_node_ids[0])
		{
			// First edge nodes (Node 0 -> Node 1) [Edge 0]
			layer_0_nodes.push_back(edge0_id);
		}

		layer_0_nodes.push_back(p_quad.corner_nodes[1]);


		// Process internal layers
		int interior_node_index = 0;

		for (int i = 1; i < order; i++)
		{
			layer_1_nodes.clear();

			// Start is edge node (Node 2 -> Node 0) [Edge 2]
			layer_1_nodes.push_back(p_quad.edge_node_ids[3][order - i - 1]);

			// Interior nodes layer 1
			for (int j = 0; j < order - 1; j++)
			{
				layer_1_nodes.push_back(p_quad.internal_nodes[interior_node_index]);
				interior_node_index++;
			}

			// End is edge node (Node 1 -> Node 2) [Edge 1]
			layer_1_nodes.push_back(p_quad.edge_node_ids[1][i - 1]);

			// Using layer_0 and layer 1 create the triangles
			create_quadelement_renderer_triangles(layer_0_nodes, layer_1_nodes);

			layer_0_nodes = layer_1_nodes;
			//
		}


		// Final layer is the final edge
		layer_1_nodes.clear();
		layer_1_nodes.push_back(p_quad.corner_nodes[3]);

		// Add in reverse
		for (const auto& edge2_id : std::vector<int>(p_quad.edge_node_ids[2].rbegin(), p_quad.edge_node_ids[2].rend()))
		{

			// First edge nodes (Node 1 -> Node 2)
			layer_1_nodes.push_back(edge2_id);
		}

		layer_1_nodes.push_back(p_quad.corner_nodes[2]);


		// Create final triangles
		create_quadelement_renderer_triangles(layer_0_nodes, layer_1_nodes);

	}
	//
}





void polynomial_2dmesh_store::create_renderer_edges()
{

	struct EdgeHash
	{
		size_t operator()(const renderer_edge& e) const
		{
			return std::hash<int>()(e.nstart) ^ (std::hash<int>()(e.nend) << 1);
		}
	};

	// Store the local edge
	std::unordered_set<renderer_edge, EdgeHash> edge_set;

	auto make_edge = [](int a, int b)
		{
			return (a < b) ? renderer_edge{ a, b } : renderer_edge{ b, a };
		};


	// --- Renderer triangle edges ---
	for (const auto& tri : renderer_element_triangles)
	{
		renderer_edge e1 = make_edge(tri.n1, tri.n2);
		renderer_edge e2 = make_edge(tri.n2, tri.n3);
		renderer_edge e3 = make_edge(tri.n3, tri.n1);

		if (edge_set.insert(e1).second)
			renderer_edge_lines.push_back({ tri.n1, tri.n2 });

		if (edge_set.insert(e2).second)
			renderer_edge_lines.push_back({ tri.n2, tri.n3 });

		if (edge_set.insert(e3).second)
			renderer_edge_lines.push_back({ tri.n3, tri.n1 });
	}
	//
}








