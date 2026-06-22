#include "h_refinement_store.h"

h_refinement_store::h_refinement_store()
{
// Empty constructor
}


void h_refinement_store::add_node(const int& node_id, const double& x_coord, const double& y_coord)
{
	// Node addition
	node_store temp_node;
	temp_node.node_id = node_id;
	temp_node.x_coord = x_coord;
	temp_node.y_coord = y_coord;

	// Insert to the node list
	node_list.insert({ node_id, temp_node });

}


void h_refinement_store::add_edge(const int& edge_id, const int& startnodeid, const int& endnodeid)
{
	// Edge addition
	edge_store temp_edge;
	temp_edge.edge_id = edge_id;
	temp_edge.startnodeid = startnodeid;
	temp_edge.endnodeid = endnodeid;

	// Insert to the edge list
	edge_list.insert({ edge_id, temp_edge });

	// Add edge to node-to-edge map for both start and end nodes
	node_edge_map[startnodeid].push_back(edge_id);
	node_edge_map[endnodeid].push_back(edge_id);

}


void h_refinement_store::add_trielement(const int& tri_id, 
	const int& nodeid1, const int& nodeid2, const int& nodeid3, 
	const int& materialid)
{
	// Triangle element addition
	trielement_store temp_trielement;
	temp_trielement.tri_id = tri_id;

	// Test the orientation of the triangle element and 
	// reorder the node IDs if necessary to ensure counter-clockwise ordering
	const node_store& n1 = node_list.at(nodeid1);
	const node_store& n2 = node_list.at(nodeid2);
	const node_store& n3 = node_list.at(nodeid3);

	// Calculate the orientation using the determinant of the matrix formed by the node coordinates
	double orientation = (n2.x_coord - n1.x_coord) * (n3.y_coord - n1.y_coord) -
		(n3.x_coord - n1.x_coord) * (n2.y_coord - n1.y_coord);

	int nd1_id = nodeid1; // Node id 1
	int nd2_id = nodeid2; // Node id 2
	int nd3_id = nodeid3; // Node id 3

	if (orientation < 0)
	{
		// If the orientation is negative, the nodes are in clockwise order, so we need to reorder them
		nd2_id = nodeid3; // Node id 2 becomes node id 3
		nd3_id = nodeid2; // Node id 3 becomes node id 2
	}


	temp_trielement.nodeid1 = nd1_id;
	temp_trielement.nodeid2 = nd2_id;
	temp_trielement.nodeid3 = nd3_id;
	temp_trielement.materialid = materialid;

	// Insert to the tri element list
	trielement_list.insert({ tri_id, temp_trielement });


	// Set the edge face IDs for the three edges of the triangle element
	set_edge_faceid(nd1_id, nd2_id, tri_id); // Edge 1
	set_edge_faceid(nd2_id, nd3_id, tri_id); // Edge 2
	set_edge_faceid(nd3_id, nd1_id, tri_id); // Edge 3
	//

}


void h_refinement_store::add_quadelement(const int& quad_id, 
	const int& nodeid1, const int& nodeid2, const int& nodeid3, const int& nodeid4, const int& materialid)
{
	// Quadrilateral element addition
	quadelement_store temp_quadelement;
	temp_quadelement.quad_id = quad_id;

	// Test the orientation of the triangle element and 
	// reorder the node IDs if necessary to ensure counter-clockwise ordering
	const node_store& n1 = node_list.at(nodeid1);
	const node_store& n2 = node_list.at(nodeid2);
	const node_store& n3 = node_list.at(nodeid3);
	const node_store& n4 = node_list.at(nodeid4);

	// Compute signed area (shoelace formula)
	double area =
		n1.x_coord * n2.y_coord - n2.x_coord * n1.y_coord +
		n2.x_coord * n3.y_coord - n3.x_coord * n2.y_coord +
		n3.x_coord * n4.y_coord - n4.x_coord * n3.y_coord +
		n4.x_coord * n1.y_coord - n1.x_coord * n4.y_coord;

	int nd1_id = nodeid1; // Node id 1
	int nd2_id = nodeid2; // Node id 2
	int nd3_id = nodeid3; // Node id 3
	int nd4_id = nodeid4; // Node id 4

	if (area < 0)
	{
		// If the orientation is negative, the nodes are in clockwise order, so we need to reorder them
		nd2_id = nodeid4; // Node id 2 becomes node id 4
		nd4_id = nodeid2; // Node id 4 becomes node id 2
	}



	temp_quadelement.nodeid1 = nd1_id;
	temp_quadelement.nodeid2 = nd2_id;
	temp_quadelement.nodeid3 = nd3_id;
	temp_quadelement.nodeid4 = nd4_id;
	temp_quadelement.materialid = materialid;

	// Insert to the quad element list
	quadelement_list.insert({ quad_id, temp_quadelement });


	// Set the edge face IDs for the four edges of the quadrilateral element
	set_edge_faceid(nd1_id, nd2_id, quad_id); // Edge 1
	set_edge_faceid(nd2_id, nd3_id, quad_id); // Edge 2
	set_edge_faceid(nd3_id, nd4_id, quad_id); // Edge 3
	set_edge_faceid(nd4_id, nd1_id, quad_id); // Edge 4
	//
}


void h_refinement_store::add_material(const int& materialid, 
	const double& youngsmodulus, const double& matdensity, const double& poissonsratio)
{
	// Material addition
	material_store temp_material;
	temp_material.materialid = materialid;
	temp_material.youngsmodulus = youngsmodulus;
	temp_material.matdensity = matdensity;
	temp_material.poissonsratio = poissonsratio;

	// Insert to the material list
	material_list.insert({ materialid, temp_material });

}


void h_refinement_store::add_nodeconstraint(const int& node_id, 
	const int& constrainttype, const double& constraintangle)
{


}


void h_refinement_store::add_nodeload(const int& node_id, 
	const double& loadamplitude, const double& loadangle)
{


}

void h_refinement_store::renumber_model()
{
	
	//_________________________________________________________________
	// Use reserve to avoid rehashing (performance optimization)
	std::unordered_map<int, node_store> temp_node_list;
	std::unordered_map<int, edge_store> temp_edge_list;
	std::unordered_map<int, trielement_store> temp_trielement_list;
	std::unordered_map<int, quadelement_store> temp_quadelement_list;

	 // Reserve space to prevent rehashing
	 temp_node_list.reserve(node_list.size());
	 temp_edge_list.reserve(edge_list.size());
	 temp_trielement_list.reserve(trielement_list.size());
	 temp_quadelement_list.reserve(quadelement_list.size());

	// Create the node map
	std::unordered_map<int, int> nodeid_map;
	nodeid_map.reserve(node_list.size());

	int nd_id_t = 0;
	for (const auto& nd : node_list)
	{
		// Node addition with move semantics
		node_store temp_node;
		temp_node.node_id = nd_id_t;
		temp_node.x_coord = nd.second.x_coord;
		temp_node.y_coord = nd.second.y_coord;

		temp_node_list.emplace(nd_id_t, std::move(temp_node));
		nodeid_map.emplace(nd.second.node_id, nd_id_t);
		nd_id_t++;
	}

	// Create the element id map
	std::unordered_map<int, int> elemid_map;
	elemid_map.reserve(trielement_list.size() + quadelement_list.size());

	int elem_id_t = 0;

	// Process triangles
	for (const auto& tri : trielement_list)
	{
		trielement_store temp_trielement;
		temp_trielement.tri_id = elem_id_t;
		temp_trielement.nodeid1 = nodeid_map[tri.second.nodeid1];
		temp_trielement.nodeid2 = nodeid_map[tri.second.nodeid2];
		temp_trielement.nodeid3 = nodeid_map[tri.second.nodeid3];
		temp_trielement.materialid = tri.second.materialid;

		temp_trielement_list.emplace(elem_id_t, std::move(temp_trielement));
		elemid_map.emplace(tri.second.tri_id, elem_id_t);
		elem_id_t++;
	}

	// Process quads
	for (const auto& quad : quadelement_list)
	{
		quadelement_store temp_quadelement;
		temp_quadelement.quad_id = elem_id_t;
		temp_quadelement.nodeid1 = nodeid_map[quad.second.nodeid1];
		temp_quadelement.nodeid2 = nodeid_map[quad.second.nodeid2];
		temp_quadelement.nodeid3 = nodeid_map[quad.second.nodeid3];
		temp_quadelement.nodeid4 = nodeid_map[quad.second.nodeid4];
		temp_quadelement.materialid = quad.second.materialid;

		temp_quadelement_list.emplace(elem_id_t, std::move(temp_quadelement));
		elemid_map.emplace(quad.second.quad_id, elem_id_t);
		elem_id_t++;
	}

	// Create the edge id map
	// std::unordered_map<int, int> edgeid_map;
	// edgeid_map.reserve(edge_list.size());

	std::unordered_map<int, std::vector<int>> temp_node_edge_map;

	int edge_id_t = 0;
	for (const auto& edge : edge_list)
	{
		edge_store temp_edge;
		temp_edge.edge_id = edge_id_t;
		temp_edge.startnodeid = nodeid_map[edge.second.startnodeid];
		temp_edge.endnodeid = nodeid_map[edge.second.endnodeid];

		// Handle face IDs 
		temp_edge.leftfaceid = -1;
		temp_edge.rightfaceid = -1;

		//auto left_it = elemid_map.find(edge.second.leftfaceid);
		//if (left_it != elemid_map.end())
		//{
		//	temp_edge.leftfaceid = left_it->second;
		//}

		//auto right_it = elemid_map.find(edge.second.rightfaceid);
		//if (right_it != elemid_map.end())
		//{
		//	temp_edge.rightfaceid = right_it->second;
		//}

		if (edge.second.leftfaceid != -1)
		{
			temp_edge.leftfaceid = elemid_map[edge.second.leftfaceid];
		}

		if (edge.second.rightfaceid != -1)
		{
			temp_edge.rightfaceid = elemid_map[edge.second.rightfaceid];
		}

		temp_edge_list.emplace(edge_id_t, std::move(temp_edge));


		// Add edge to node-to-edge map for both start and end nodes
		temp_node_edge_map[temp_edge.startnodeid].push_back(edge_id_t);
		temp_node_edge_map[temp_edge.endnodeid].push_back(edge_id_t);

		// edgeid_map.emplace(edge.second.edge_id, edge_id_t);
		edge_id_t++;
	}


	// Move to original (more efficient than clear + insert)
	node_list = std::move(temp_node_list);
	edge_list = std::move(temp_edge_list);
	trielement_list = std::move(temp_trielement_list);
	quadelement_list = std::move(temp_quadelement_list);

	node_edge_map = std::move(temp_node_edge_map);

}


void h_refinement_store :: refine_element()
{
	std::unordered_map<int, int> edge_to_node_ids;
	edge_to_node_ids.reserve(edge_list.size());  // Reserve space for performance

	// Get the current node count (will be updated as we add nodes)
	int node_id = static_cast<int>(node_list.size());

	// Lambda to create mid-node (captures by reference)
	auto create_midnode = [&](int startnodeid, int endnodeid) -> int
		{
			// Get the nodes
			auto start_it = node_list.find(startnodeid);
			auto end_it = node_list.find(endnodeid);

			if (start_it == node_list.end() || end_it == node_list.end())
			{
				// Handle error: node not found
				return -1;
			}

			const node_store& start_node = start_it->second;
			const node_store& end_node = end_it->second;

			// Create the mid point
			double midpt_xcoord = (start_node.x_coord + end_node.x_coord) * 0.5;
			double midpt_ycoord = (start_node.y_coord + end_node.y_coord) * 0.5;

			node_store temp_node;
			temp_node.node_id = node_id;
			temp_node.x_coord = midpt_xcoord;
			temp_node.y_coord = midpt_ycoord;

			// Insert the new node to the original list
			node_list.emplace(node_id, std::move(temp_node));

			return node_id++;
		};

	// Create refined element lists
	std::unordered_map<int, trielement_store> refined_trielement_list;
	std::unordered_map<int, quadelement_store> refined_quadelement_list;

	refined_trielement_list.reserve(trielement_list.size() * 4);
	refined_quadelement_list.reserve(quadelement_list.size() * 4);

	int elem_id = 0;

	// Lambda to create triangle element (captures by reference)
	auto create_trielement = [&](int nd1, int nd2, int nd3, int matid) -> void
		{
			trielement_store tri;
			tri.tri_id = elem_id;
			tri.nodeid1 = nd1;
			tri.nodeid2 = nd2;
			tri.nodeid3 = nd3;
			tri.materialid = matid;

			refined_trielement_list.emplace(elem_id, std::move(tri));
			elem_id++;
		};

	// Process triangles
	for (const auto& tri : trielement_list)
	{
		const trielement_store& trielement = tri.second;

		// Get the three node ids
		int nd1 = trielement.nodeid1;
		int nd2 = trielement.nodeid2;
		int nd3 = trielement.nodeid3;

		// Get the edge ids
		int edge1_id = get_edge_id(nd1, nd2);
		int edge2_id = get_edge_id(nd2, nd3);
		int edge3_id = get_edge_id(nd3, nd1);

		int mid_node1 = -1;
		int mid_node2 = -1;
		int mid_node3 = -1;

		// Create three mid nodes (with edge sharing)
		auto edge1_it = edge_to_node_ids.find(edge1_id);
		if (edge1_it != edge_to_node_ids.end())
		{
			mid_node1 = edge1_it->second;
		}
		else
		{
			mid_node1 = create_midnode(nd1, nd2);
			edge_to_node_ids.emplace(edge1_id, mid_node1);
		}

		auto edge2_it = edge_to_node_ids.find(edge2_id);
		if (edge2_it != edge_to_node_ids.end())
		{
			mid_node2 = edge2_it->second;
		}
		else
		{
			mid_node2 = create_midnode(nd2, nd3);
			edge_to_node_ids.emplace(edge2_id, mid_node2);
		}

		auto edge3_it = edge_to_node_ids.find(edge3_id);
		if (edge3_it != edge_to_node_ids.end())
		{
			mid_node3 = edge3_it->second;
		}
		else
		{
			mid_node3 = create_midnode(nd3, nd1);
			edge_to_node_ids.emplace(edge3_id, mid_node3);
		}

		// Create 4 triangle elements
		// Corner triangles
		create_trielement(nd1, mid_node1, mid_node3, trielement.materialid);
		create_trielement(nd2, mid_node2, mid_node1, trielement.materialid);
		create_trielement(nd3, mid_node3, mid_node2, trielement.materialid);
		// Center triangle
		create_trielement(mid_node1, mid_node2, mid_node3, trielement.materialid);
	}

	// Process quads (if you have them)
	auto create_quadelement = [&](int nd1, int nd2, int nd3, int nd4, int matid) -> void
		{
			quadelement_store quad;
			quad.quad_id = elem_id;
			quad.nodeid1 = nd1;
			quad.nodeid2 = nd2;
			quad.nodeid3 = nd3;
			quad.nodeid4 = nd4;
			quad.materialid = matid;

			refined_quadelement_list.emplace(elem_id, std::move(quad));
			elem_id++;
		};

	for (const auto& quad : quadelement_list)
	{
		const quadelement_store& quadelement = quad.second;

		int nd1 = quadelement.nodeid1;
		int nd2 = quadelement.nodeid2;
		int nd3 = quadelement.nodeid3;
		int nd4 = quadelement.nodeid4;

		// Get edge ids
		int edge1_id = get_edge_id(nd1, nd2);
		int edge2_id = get_edge_id(nd2, nd3);
		int edge3_id = get_edge_id(nd3, nd4);
		int edge4_id = get_edge_id(nd4, nd1);

		// Create mid nodes (4 edges)
		int mid_node1 = -1, mid_node2 = -1, mid_node3 = -1, mid_node4 = -1;

		// Edge 1 (nd1-nd2)
		auto it = edge_to_node_ids.find(edge1_id);
		if (it != edge_to_node_ids.end())
			mid_node1 = it->second;
		else
		{
			mid_node1 = create_midnode(nd1, nd2);
			edge_to_node_ids.emplace(edge1_id, mid_node1);
		}

		// Edge 2 (nd2-nd3)
		it = edge_to_node_ids.find(edge2_id);
		if (it != edge_to_node_ids.end())
			mid_node2 = it->second;
		else
		{
			mid_node2 = create_midnode(nd2, nd3);
			edge_to_node_ids.emplace(edge2_id, mid_node2);
		}

		// Edge 3 (nd3-nd4)
		it = edge_to_node_ids.find(edge3_id);
		if (it != edge_to_node_ids.end())
			mid_node3 = it->second;
		else
		{
			mid_node3 = create_midnode(nd3, nd4);
			edge_to_node_ids.emplace(edge3_id, mid_node3);
		}

		// Edge 4 (nd4-nd1)
		it = edge_to_node_ids.find(edge4_id);
		if (it != edge_to_node_ids.end())
			mid_node4 = it->second;
		else
		{
			mid_node4 = create_midnode(nd4, nd1);
			edge_to_node_ids.emplace(edge4_id, mid_node4);
		}

		// Create center node
		// Get the coordinates of the four corners
		auto n1_it = node_list.find(nd1);
		auto n2_it = node_list.find(nd2);
		auto n3_it = node_list.find(nd3);
		auto n4_it = node_list.find(nd4);

		if (n1_it != node_list.end() && n2_it != node_list.end() &&
			n3_it != node_list.end() && n4_it != node_list.end())
		{
			double center_x = (n1_it->second.x_coord + n2_it->second.x_coord +
				n3_it->second.x_coord + n4_it->second.x_coord) * 0.25;
			double center_y = (n1_it->second.y_coord + n2_it->second.y_coord +
				n3_it->second.y_coord + n4_it->second.y_coord) * 0.25;

			node_store center_node;
			center_node.node_id = node_id;
			center_node.x_coord = center_x;
			center_node.y_coord = center_y;

			node_list.emplace(node_id, std::move(center_node));
			int center_node_id = node_id++;

			// Create 4 quad elements
			create_quadelement(nd1, mid_node1, center_node_id, mid_node4, quadelement.materialid);
			create_quadelement(mid_node1, nd2, mid_node2, center_node_id, quadelement.materialid);
			create_quadelement(center_node_id, mid_node2, nd3, mid_node3, quadelement.materialid);
			create_quadelement(mid_node4, center_node_id, mid_node3, nd4, quadelement.materialid);
		}
	}

	// Replace old element lists with refined ones
	trielement_list = std::move(refined_trielement_list);
	quadelement_list = std::move(refined_quadelement_list);

}


void h_refinement_store::refine_elements1()
{
	// Refine the elements
	// Subdivide element at edge mid point

	// Temp edge list
	std::unordered_map<int, edge_store> temp_edge_list;

	std::unordered_map<int, std::vector<int>> temp_node_edge_map;

	int node_id = static_cast<int>(node_list.size());
	int edge_id = 0;

	// Create nodes at the mid point of edges
	for (const auto& edge : edge_list)
	{
		node_store start_node = node_list[edge.second.startnodeid];
		node_store end_node = node_list[edge.second.endnodeid];

		// Create the mid point
		double midpt_xcoord = (start_node.x_coord + end_node.x_coord) * 0.5;
		double midpt_ycoord = (start_node.y_coord + end_node.y_coord) * 0.5;

		node_store temp_node;
		temp_node.node_id = node_id;
		temp_node.x_coord = midpt_xcoord;
		temp_node.y_coord = midpt_ycoord;

		// Insert the new node to the original list
		node_list.insert({ node_id, temp_node });

		//_______________________________________________________________________
		// Create edge 1
		edge_store temp_edge1;
		temp_edge1.edge_id = edge_id;
		temp_edge1.startnodeid = edge.second.startnodeid;
		temp_edge1.endnodeid = node_id;

		// Handle face IDs 
		temp_edge1.leftfaceid = edge.second.leftfaceid;
		temp_edge1.rightfaceid = edge.second.rightfaceid;

		// Insert the new edge, edge 1
		temp_edge_list.insert({ edge_id, temp_edge1 });

		// Add edge to node-to-edge map for both start and end nodes
		temp_node_edge_map[edge.second.startnodeid].push_back(edge_id);
		temp_node_edge_map[node_id].push_back(edge_id);

		edge_id++;

		//_______________________________________________________________________
		// Create edge 2
		edge_store temp_edge2;
		temp_edge2.edge_id = edge_id;
		temp_edge2.startnodeid = node_id;
		temp_edge2.endnodeid = edge.second.endnodeid;

		// Handle face IDs 
		temp_edge2.leftfaceid = edge.second.leftfaceid;
		temp_edge2.rightfaceid = edge.second.rightfaceid;

		// Insert the new edge, edge 2 
		temp_edge_list.insert({ edge_id, temp_edge2 });

		// Add edge to node-to-edge map for both start and end nodes
		temp_node_edge_map[node_id].push_back(edge_id);
		temp_node_edge_map[edge.second.endnodeid].push_back(edge_id);

		edge_id++;

		// Increment the node id
		node_id++;
	}

	// Move to original (more efficient than clear + insert)
	edge_list = std::move(temp_edge_list);
	node_edge_map = std::move(temp_node_edge_map);

}



void h_refinement_store::perform_refinement(int h_refinement)
{
	// Renumber the nodes and elements
	renumber_model();

	if (h_refinement == 1)
	{
		// 1 element to 4 elements
		refine_elements();
	}
	else if (h_refinement == 2)
	{
		// 1 element to 16 elements
		refine_elements();

		refine_elements();
	}

}



void h_refinement_store::set_edge_faceid(const int& startnodeid,
	const int& endnodeid, const int& face_id)
{
	// Fix the direction of the edges of the element based on the node ordering
	int edge_id = get_edge_id(startnodeid, endnodeid); // Edge

	//if (edge_list.find(edge_id) != edge_list.end())
	//{
		// If the edge already exists, check if the direction matches the node ordering
	edge_store& edge = edge_list.at(edge_id);
	if (edge.startnodeid == startnodeid && edge.endnodeid == endnodeid)
	{
		// The edge direction matches the node ordering
		edge.leftfaceid = face_id; // Set the left face ID for this edge
	}
	else if (edge.startnodeid == endnodeid && edge.endnodeid == startnodeid)
	{
		edge.rightfaceid = face_id; // Set the right face ID for this edge
	}
	//else
	//{
	//	// std::cerr << "Error: Edge " << edge1_id << " does not connect the correct nodes for triangle element " << tri_id << std::endl;
	//	exit(1);
	//}
	//}

}



int h_refinement_store::get_edge_id(const int& startnodeid, const int& endnodeid)
{

	// Get the connected edges to start node
	const std::vector<int>& connected_edges = this->node_edge_map[startnodeid];

	for (const int& edge_id : connected_edges)
	{
		const auto& edge = this->edge_list[edge_id];
		if ((edge.startnodeid == startnodeid && edge.endnodeid == endnodeid) ||
			(edge.startnodeid == endnodeid && edge.endnodeid == startnodeid))
		{
			// Line with the same start and end nodes
			return edge_id;
		}

	}

	return -1;
}





