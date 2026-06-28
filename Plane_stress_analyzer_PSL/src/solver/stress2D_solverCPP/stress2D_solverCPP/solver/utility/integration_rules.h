#pragma once
#include <vector>



// Integration point structure
struct integration_point
{
    double xi;      // Natural coordinate 1
    double eta;     // Natural coordinate 2
    double weight;  // Integration weight
};




class integration_rules
{

public:

    // Get 2D Gauss-Legendre integration points for quadrilaterals
    static std::vector<integration_point> get_quad_2d_gauss_points(int order);

    // Get integration points for triangles (using Gauss quadrature)
    static std::vector<integration_point> get_tri_gauss_points(int order);


private:

    // Get Gauss-Legendre integration points for 1D
    static std::vector<std::pair<double, double>> get_1d_gauss_points(int order);

};


