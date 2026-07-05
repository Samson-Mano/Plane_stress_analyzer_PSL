#include "stress_system_store.h"


stress_system_store::stress_system_store()
{
	// Empty constructor
}





Eigen::Matrix3d material_store::get_elasticity_matrix() const
{
	Eigen::Matrix3d C;
	C.setZero();

	double E = youngsmodulus;   // 910; //
	double nu = poissonsratio; // 0.3; //

	if (formulation == 0)
	{
		// Plane stress
		double factor = E / (1.0 - nu * nu);
		C(0, 0) = factor;
		C(0, 1) = factor * nu;
		C(1, 0) = factor * nu;
		C(1, 1) = factor;
		C(2, 2) = factor * (1.0 - nu) / 2.0;
	}
	else
	{
		// Plane strain
		double factor = E / ((1.0 + nu) * (1.0 - 2.0 * nu));
		C(0, 0) = factor * (1.0 - nu);
		C(0, 1) = factor * nu;
		C(1, 0) = factor * nu;
		C(1, 1) = factor * (1.0 - nu);
		C(2, 2) = factor * (1.0 - 2.0 * nu) / 2.0;
	}

	return C;
}


