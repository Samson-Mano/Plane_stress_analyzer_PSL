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

	copy_mesh_nodes();


	this->polynomial_trielement_list.clear();
	this->polynomial_quadelement_list.clear();

	this->isPolynomialMeshCreated = false;

	if (this->polynomial_order == 1)
	{
		// p = 1 (Linear / Bilinear)
		// No change - T3 + Q4
		create_linear_mesh();

	}
	else if (this->polynomial_order == 2)
	{
		// p = 2 (Quadratic)
		// T6: Adds 1 node per edge
		// Q9: Adds 1 node per edge + 1 center node
		create_quadratic_mesh();

	}
	else if (this->polynomial_order == 3)
	{
		// p = 3 (Cubic)
		// T10: Adds 2 nodes per edge + 1 internal node
		// Q16: Adds 2 nodes per edge + 4 internal nodes
		create_cubic_mesh();

	}
	else if (this->polynomial_order == 4)
	{
		// p = 4 (Quartic)
		// T15: Adds 3 nodes per edge + 3 internal nodes
		// Q25: Adds 3 nodes per edge + 9 internal nodes
		create_quartic_mesh();

	}
	else
	{
		this->isPolynomialMeshCreated = false;
	}

}


void polynomial_2dmesh_store::copy_mesh_nodes()
{

	this->polynomial_node_list.clear();
	this->polynomial_edge_list.clear();

	// Copy the existing node to the polynomial node system
	for (const auto& nd : this->stress_system.node_list)
	{
		polynomial_node_store p_node;
		p_node.node_id = nd.second.node_id;
		p_node.x_coord = nd.second.x_coord;
		p_node.y_coord = nd.second.y_coord;

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

		// p_edge.edge_internal_node_ids.clear();

		this->polynomial_edge_list.insert({ edge.first, p_edge });
	}

}


void polynomial_2dmesh_store::create_linear_mesh()
{

	// int node_id = static_cast<int>(this->stress_system.node_list.size());

	// Copy the triangle element to polynomial triangle element system
	for (const auto& tri : this->stress_system.trielement_list)
	{
		polynomial_trielement_store p_tri;
		p_tri.tri_id = tri.second.tri_id;

		std::vector<int> corner_nodes;
		corner_nodes.push_back(tri.second.nodeid1);
		corner_nodes.push_back(tri.second.nodeid2);
		corner_nodes.push_back(tri.second.nodeid3);

		p_tri.corner_nodes = std::move(corner_nodes);

		// p_tri.edge_node_ids.clear();

		// p_tri.internal_nodes.clear();

		p_tri.materialid = tri.second.materialid;

		this->polynomial_trielement_list.insert({ tri.first, p_tri });
	}


	// Copy the quadrilateral element to polynomial quadrilateral element system

	for (const auto& quad : this->stress_system.quadelement_list)
	{
		polynomial_quadelement_store p_quad;
		p_quad.quad_id = quad.second.quad_id;

		std::vector<int> corner_nodes;
		corner_nodes.push_back(quad.second.nodeid1);
		corner_nodes.push_back(quad.second.nodeid2);
		corner_nodes.push_back(quad.second.nodeid3);
		corner_nodes.push_back(quad.second.nodeid4);

		p_quad.corner_nodes = std::move(corner_nodes);

		// p_quad.edge_node_ids.clear();

		// p_quad.internal_nodes.clear();

		p_quad.materialid = quad.second.materialid;

		this->polynomial_quadelement_list.insert({ quad.first, p_quad });
	}

}



void polynomial_2dmesh_store::create_quadratic_mesh()
{

	int node_id = static_cast<int>(this->stress_system.node_list.size());


	// Create the triangle element to polynomial triangle element system
	for (const auto& tri : this->stress_system.trielement_list)
	{
		polynomial_trielement_store p_tri;
		p_tri.tri_id = tri.second.tri_id;

		std::vector<int> corner_nodes;
		corner_nodes.push_back(tri.second.nodeid1);
		corner_nodes.push_back(tri.second.nodeid2);
		corner_nodes.push_back(tri.second.nodeid3);

		p_tri.corner_nodes = std::move(corner_nodes);

		// p_tri.edge_node_ids.clear();

		// p_tri.internal_nodes.clear();

		p_tri.materialid = tri.second.materialid;

		this->polynomial_trielement_list.insert({ tri.first, p_tri });
	}


	// Create the quadrilateral element to polynomial quadrilateral element system

	for (const auto& quad : this->stress_system.quadelement_list)
	{
		polynomial_quadelement_store p_quad;
		p_quad.quad_id = quad.second.quad_id;

		std::vector<int> corner_nodes;
		corner_nodes.push_back(quad.second.nodeid1);
		corner_nodes.push_back(quad.second.nodeid2);
		corner_nodes.push_back(quad.second.nodeid3);
		corner_nodes.push_back(quad.second.nodeid4);

		p_quad.corner_nodes = std::move(corner_nodes);

		// p_quad.edge_node_ids.clear();

		// p_quad.internal_nodes.clear();

		p_quad.materialid = quad.second.materialid;

		this->polynomial_quadelement_list.insert({ quad.first, p_quad });
	}


}


void polynomial_2dmesh_store::create_cubic_mesh()
{


}


void polynomial_2dmesh_store::create_quartic_mesh()
{


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
