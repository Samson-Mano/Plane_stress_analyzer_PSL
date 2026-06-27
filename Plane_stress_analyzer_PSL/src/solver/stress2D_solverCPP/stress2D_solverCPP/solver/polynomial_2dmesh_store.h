#pragma once
#include "../system_store/stress_system_store.h"



// Renderer Triangle
struct renderer_triangle
{
	int n1, n2, n3;
};


// Renderer Edge
struct renderer_edge
{
	int nstart, nend;

	bool operator==(const renderer_edge& other) const
	{
		return nstart == other.nstart && nend == other.nend;
	}
};


// Renderer Node
struct renderer_node
{
	int n_id;
	double x, y;
	double r1, r2, r3, r4; // Scalar Result data
};



struct polynomial_node_store
{
	int node_id = 0;
	double x_coord = 0.0;
	double y_coord = 0.0;

};


struct polynomial_edge_store
{
	int edge_id = 0;
	int startnodeid = 0;
	int endnodeid = 0;

	std::vector<int> edge_internal_node_ids; // Internal node IDs on the edge (for higher-order polynomial elements)

	int leftfaceid = -1; // The face on the left side of the edge (when looking from start node to end node)
	int rightfaceid = -1; // The face on the right side of the edge (when looking from start node to end node)

};


struct polynomial_trielement_store
{

	int tri_id = 0;

	double tri_area = 0.0;

	std::vector<int> corner_nodes; // 3 corner nodes of the triangle element

	// edge_node_ids[0] for edge 1, edge_node_ids[1] for edge 2, edge_node_ids[2] for edge 3
	std::vector<std::vector<int>> edge_node_ids{ 3 };

	std::vector<int> internal_nodes; // Internal nodes of the triangle element (for higher-order spectral elements)

	int materialid = 0;

};


struct polynomial_quadelement_store
{
	int quad_id = 0;

	std::vector<int> corner_nodes; // 4 corner nodes of the quadrilateral element

	// edge_node_ids[0] for edge 1, edge_node_ids[1] for edge 2, edge_node_ids[2] for edge 3, edge_node_ids[3] for edge 4
	std::vector<std::vector<int>> edge_node_ids{ 4 };

	std::vector<int> internal_nodes; // Internal nodes of the quadrialteral element (for higher-order spectral elements)

	int materialid = 0;

};




class polynomial_2dmesh_store
{
public:
	polynomial_2dmesh_store();
	~polynomial_2dmesh_store() = default;

	void generate_2dpolynomial_mesh(stress_system_store* stress_system, int polynomial_order);

	bool isPolynomialMeshCreated = false;

private:
	stress_system_store stress_system;

	int polynomial_order = 0;

	std::unordered_map<int, polynomial_node_store> polynomial_node_list;
	std::unordered_map<int, polynomial_edge_store> polynomial_edge_list;
	std::unordered_map<int, polynomial_trielement_store> polynomial_trielement_list;
	std::unordered_map<int, polynomial_quadelement_store> polynomial_quadelement_list;


	void copy_mesh_nodes();

	void create_linear_mesh();
	void create_quadratic_mesh();
	void create_cubic_mesh();
	void create_quartic_mesh();


	int get_edge_id(const int& startnodeid, const int& endnodeid);


};

