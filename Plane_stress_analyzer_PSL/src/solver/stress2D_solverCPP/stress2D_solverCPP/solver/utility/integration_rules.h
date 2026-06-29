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

    // n = points per direction (2..5); builds tensor product
    static std::vector<integration_point> get_quad_gauss_points(int polynomial_order);


    // degree = required degree of exactness (1,3,5,7 for T3..T15)
    static std::vector<integration_point> get_tri_dunavant_points(int polynomial_order);


private:

    // Get Gauss-Legendre integration points for 1D
    static std::vector<std::pair<double, double>> get_1d_gauss_points(int order);

};


