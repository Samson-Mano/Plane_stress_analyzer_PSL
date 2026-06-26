#include "polynomial_2dmesh_store.h"

polynomial_2dmesh_store::polynomial_2dmesh_store()
{
// Empty constructor

}


void polynomial_2dmesh_store::generate_2dpolynomial_mesh(stress_system_store* stress_system)
{
	// Generate 2d polynomial mesh
	this->stress_system = std::move(*stress_system);








}

