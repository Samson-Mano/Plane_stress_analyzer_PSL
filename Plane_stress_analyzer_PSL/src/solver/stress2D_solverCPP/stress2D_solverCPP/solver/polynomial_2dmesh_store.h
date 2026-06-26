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



class polynomial_2dmesh_store
{
public:
	polynomial_2dmesh_store();
	~polynomial_2dmesh_store() = default;

	void generate_2dpolynomial_mesh(stress_system_store* stress_system);

private:
	stress_system_store stress_system;


};

