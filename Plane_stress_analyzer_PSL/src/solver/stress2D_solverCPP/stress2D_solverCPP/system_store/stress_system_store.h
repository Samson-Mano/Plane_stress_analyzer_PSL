#pragma once
#include <Eigen/Dense>
#include <unordered_map>




struct node_store
{
	int node_id = 0;
	double x_coord = 0.0;
	double y_coord = 0.0;

};


struct edge_store
{
	int edge_id = 0;
	int startnodeid = 0;
	int endnodeid = 0;

	int leftfaceid = -1; // The face on the left side of the edge (when looking from start node to end node)
	int rightfaceid = -1; // The face on the right side of the edge (when looking from start node to end node)

};


struct trielement_store
{
	int tri_id = 0;
	int nodeid1 = 0;
	int nodeid2 = 0;
	int nodeid3 = 0;
	int materialid = 0;

};

struct quadelement_store
{
	int quad_id = 0;
	int nodeid1 = 0;
	int nodeid2 = 0;
	int nodeid3 = 0;
	int nodeid4 = 0;
	int materialid = 0;

};


class material_store
{
public:
	int materialid = 0;
	double youngsmodulus = 0.0;
	double matdensity = 0.0;
	double poissonsratio = 0.0;
	double yieldpoint = 0.0;
	double thickness = 0.0;

	int formulation = -1; // 0 = Plane stress, 1 = Plane strain

	material_store(int t_materialid, double t_youngsmodulus, double t_matdensity, 
		double t_poissonsratio, double t_yieldpoint, double t_thickness,
		int t_formulation)
		: materialid(t_materialid), youngsmodulus(t_youngsmodulus), 
		matdensity(t_matdensity), poissonsratio(t_poissonsratio),
		yieldpoint(t_yieldpoint), thickness(t_thickness),
		formulation(t_formulation){}


	// Get elasticity matrix C
	Eigen::Matrix3d get_elasticity_matrix() const;

};


struct constraint_store
{
	int constraint_set_id = 0;
	int constrainttype = -1;
	double constraintangle = 0.0;
	std::vector<int> node_ids;

};


struct load_store
{
	int load_set_id = 0;
	double loadamplitude = 0.0;
	double loadangle = 0.0;
	std::vector<int> node_ids;

};



class stress_system_store
{
public:
	int polynomial_order = 1; // Polynomial order
	// 1 Linear/ Bilinear T3, Q4
	// 2 Quadratic T6, Q9
	// 3 Cubic T10, Q16
	// 4 Quartic T15, Q25

	std::unordered_map<int, node_store> node_list;
	std::unordered_map<int, edge_store> edge_list;
	std::unordered_map<int, trielement_store> trielement_list;
	std::unordered_map<int, quadelement_store> quadelement_list;
	
	std::unordered_map<int, material_store> material_list;

	std::unordered_map<int, constraint_store> constraint_list;
	std::unordered_map<int, load_store> load_list;

	std::unordered_map<int, std::vector<int>> node_edge_map;

	stress_system_store();
	~stress_system_store() = default;



private:




};


