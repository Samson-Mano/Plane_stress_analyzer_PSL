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
	std::unordered_map<int, int> edgeid_map;
	edgeid_map.reserve(edge_list.size());

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
		edgeid_map.emplace(edge.second.edge_id, edge_id_t);
		edge_id_t++;
	}


	// Move to original (more efficient than clear + insert)
	node_list = std::move(temp_node_list);
	edge_list = std::move(temp_edge_list);
	trielement_list = std::move(temp_trielement_list);
	quadelement_list = std::move(temp_quadelement_list);

}




void h_refinement_store::perform_refinement(int h_refinement)
{
	// Renumber the nodes and elements
	renumber_model();



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





