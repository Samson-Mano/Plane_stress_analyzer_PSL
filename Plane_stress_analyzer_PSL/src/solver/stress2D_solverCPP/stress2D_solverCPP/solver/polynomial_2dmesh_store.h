#pragma once
#include "../system_store/stress_system_store.h"
#include <unordered_set>


// Renderer Triangle
struct renderer_triangle
{
	// int tri_id;
	int n1 = -1, n2 = -1, n3 = -1;
};


// Renderer Edge
struct renderer_edge
{
	int nstart = -1, nend = -1;

	bool operator==(const renderer_edge& other) const
	{
		return nstart == other.nstart && nend == other.nend;
	}
};


// Renderer Node
struct renderer_node
{
	int n_id = -1;
	double x = 0.0, y = 0.0;

	double displ_x = 0.0; // Displacement in x direction
	double displ_y = 0.0; // Displacement in y direction

	int constraint_type = 0; // 0: Free, 1: Pinned, 2: Roller
	double constraint_angle = 0.0; // Constraint angle in degrees (for inclined supports)


	double reaction_x = 0.0; // Reaction force in x direction
	double reaction_y = 0.0; // Reaction force in y direction

	// Averaged nodal values
	double sigma_x = 0.0; // Stress in x direction
	double sigma_y = 0.0; // Stress in y direction
	double tau_xy = 0.0;   // Shear stress in xy plane

	double sigma_1 = 0.0; // Principal stress 1
	double sigma_2 = 0.0; // Principal stress 2
	double von_mises = 0.0; // Von Mises stress
	double max_shear = 0.0; // Maximum shear stress
	double theta_p = 0.0; // Principal stress angle

	double streamfunction_tension = 0.0; // Streamfunction tension (for visualization of stress flow)
	double streamfunction_compression = 0.0; // Streamfunction compression (for visualization of stress flow)


};


struct element_results
{
	// Stress results
	double sigma_x = 0.0; // Stress in x direction
	double sigma_y = 0.0; // Stress in y direction
	double tau_xy = 0.0;   // Shear stress in xy plane

	double sigma_1 = 0.0; // Principal stress 1
	double sigma_2 = 0.0; // Principal stress 2
	double von_mises = 0.0; // Von Mises stress
	double max_shear = 0.0; // Maximum shear stress
	double theta_p = 0.0; // Principal stress angle

	// Strain results
	double epsilon_x = 0.0; // Strain in x direction
	double epsilon_y = 0.0; // Strain in y direction
	double gamma_xy = 0.0;   // Shear strain in xy plane

	double epsilon_1 = 0.0; // Principal strain 1
	double epsilon_2 = 0.0; // Principal strain 2
	double von_mises_strain = 0.0; // Von Mises strain
	double max_shear_strain = 0.0; // Maximum shear strain

};


struct polynomial_node_store
{
	int node_id = 0;
	double x_coord = 0.0;
	double y_coord = 0.0;

	double displ_x = 0.0; // Displacement in x direction
	double displ_y = 0.0; // Displacement in y direction

	double reaction_x = 0.0; // Reaction force in x direction
	double reaction_y = 0.0; // Reaction force in y direction

	// Averaged nodal values
	double sigma_x = 0.0; // Stress in x direction
	double sigma_y = 0.0; // Stress in y direction
	double tau_xy = 0.0;   // Shear stress in xy plane

	double sigma_1 = 0.0; // Principal stress 1
	double sigma_2 = 0.0; // Principal stress 2
	double von_mises = 0.0; // Von Mises stress
	double max_shear = 0.0; // Maximum shear stress
	double theta_p = 0.0; // Principal stress angle


	bool is_internal = false;

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

	std::vector<int> ordered_node_ids; // Ordered node ids of the triangle element

	int materialid = 0;

	// Element results
	std::vector<element_results> results_at_ip; // Stress and strain results at integration points

};


struct polynomial_quadelement_store
{
	int quad_id = 0;

	std::vector<int> corner_nodes; // 4 corner nodes of the quadrilateral element

	// edge_node_ids[0] for edge 1, edge_node_ids[1] for edge 2, edge_node_ids[2] for edge 3, edge_node_ids[3] for edge 4
	std::vector<std::vector<int>> edge_node_ids{ 4 };

	std::vector<int> internal_nodes; // Internal nodes of the quadrialteral element (for higher-order spectral elements)
	
	std::vector<int> ordered_node_ids; // Ordered node ids of the quadrilateral element

	int materialid = 0;
	
	// Element results
	std::vector<element_results> results_at_ip; // Stress and strain results at integration points

};




class polynomial_2dmesh_store
{
public:
	std::unordered_map<int, polynomial_node_store> polynomial_node_list;
	std::unordered_map<int, polynomial_edge_store> polynomial_edge_list;
	std::unordered_map<int, polynomial_trielement_store> polynomial_trielement_list;
	std::unordered_map<int, polynomial_quadelement_store> polynomial_quadelement_list;

	bool isPolynomialMeshCreated = false;

	// Store the renderer data
	std::unordered_map<int, renderer_node> renderer_node_points;
	std::vector<renderer_edge> renderer_edge_lines;
	std::vector<renderer_triangle> renderer_element_triangles;



	polynomial_2dmesh_store();
	~polynomial_2dmesh_store() = default;

	void generate_2dpolynomial_mesh(stress_system_store* stress_system, int polynomial_order);

	// Access constraint data
	const std::unordered_map<int, constraint_store>& get_constraint_data() const {
		return stress_system.constraint_list;
	}

	// Access load data
	const std::unordered_map<int, load_store>& get_load_data() const {
		return stress_system.load_list;
	}

	// Access material data
	const std::unordered_map<int, material_store>& get_material_data() const { 
		return stress_system.material_list; 
	}

	// Access polynomial order
	const int get_polynomial_order() const {
		return polynomial_order;
	}


private:
	stress_system_store stress_system;

	int polynomial_order = 0;



	void copy_mesh_nodes();

	void create_edge_internal_nodes();

	void create_polynomial_tri_elements();
	void create_polynomial_quad_elements();

	std::vector<int> get_ordered_edge_internal_nodes(const polynomial_edge_store& p_edge, int element_id);

	std::vector<int> create_tri_internal_nodes(int nd1, int nd2, int nd3);

	std::vector<int> create_quad_internal_nodes(int nd1, int nd2, int nd3, int nd4);

	int get_edge_id(const int& startnodeid, const int& endnodeid);

	void create_renderer_mesh();

	void create_trimesh_renderer_elements();

	void create_quadmesh_renderer_elements();

	void create_renderer_edges();


};

