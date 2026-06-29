#include "integration_rules.h"

std::vector<integration_point> integration_rules::get_tri_dunavant_points(int polynomial_order)
{
    // Degree of exactness = 2 * polynomial_order - 1
    // p=1 -> degree=1 (1 point)
    // p=2 -> degree=3 (4 points) 
    // p=3 -> degree=5 (7 points)
    // p=4 -> degree=7 (12 points)
    // 
    // degree = required degree of exactness (1,3,5,7 for T3..T15)
    // n=2,3,4,5 for p=1,2,3,4
    int degree = (2 * polynomial_order) - 1;

    std::vector<integration_point> points;

    switch (degree)
    {
    case 1:
    {
        // 1-point rule, exact for degree 1
        points = {
            { 1.0 / 3.0, 1.0 / 3.0, 1.0 }
        };
        break;
    }
    case 3:
    {
        // 4-point rule - optimal for degree 3
        const double w_centroid = -27.0 / 48.0;  //  -27.0 / 96.0 = -0.28125
        const double w_edge = 25.0 / 48.0;       // 25.0 / 96.0 = 0.260416666666667

        points = {
            {1.0 / 3.0, 1.0 / 3.0, w_centroid},
            {0.2, 0.2, w_edge},
            {0.6, 0.2, w_edge},
            {0.2, 0.6, w_edge}
        };
        break;
    }
    case 5:
    {
        // 7-point rule, exact for degree 5
        const double w0 = 0.225000000000000;

        const double a1 = 0.470142064105115;
        const double b1 = 0.059715871789770;
        const double w1 = 0.132394358246997;

        const double a2 = 0.101286507323456;
        const double b2 = 0.797426985353087;
        const double w2 = 0.125939180544827;

        points = {
            // Centroid
            { 1.0 / 3.0, 1.0 / 3.0, w0 },
            // 3 permutations of (a1, a1, b1)
            { a1, a1, w1 },
            { b1, a1, w1 },
            { a1, b1, w1 },
            // 3 permutations of (a2, a2, b2)
            { a2, a2, w2 },
            { b2, a2, w2 },
            { a2, b2, w2 }
        };
        break;
    }
    case 7:
    {
        // 13-point rule, exact for degree 7 (Dunavant 1985)
        const double a1 = 0.260345966079038;
        const double b1 = 0.479308067841923;
        const double w1 = 0.175615257433204;

        const double a2 = 0.065130102902216;
        const double b2 = 0.869739794195568;
        const double w2 = 0.053347235608839;

        const double a3 = 0.048690315425316;
        const double b3 = 0.312865496004875;
        const double c3 = 0.638444188569809;
        const double w3 = 0.077113596235860;

        const double w0 = -0.149570044467670;  // negative weight at centroid

        points = {
            // Centroid
            { 1.0 / 3.0, 1.0 / 3.0, w0 },
            // 3 permutations of (a1, a1, b1)
            { a1, a1, w1 },
            { b1, a1, w1 },
            { a1, b1, w1 },
            // 3 permutations of (a2, a2, b2)
            { a2, a2, w2 },
            { b2, a2, w2 },
            { a2, b2, w2 },
            // 6 permutations of (a3, b3, c3) — scalene triangle, all 3 orderings x2
            { a3, b3, w3 },
            { b3, a3, w3 },
            { a3, c3, w3 },
            { c3, a3, w3 },
            { b3, c3, w3 },
            { c3, b3, w3 }
        };
        break;
    }
    default:
        // Fallback to higher order
        return get_quad_gauss_points(std::min(polynomial_order, 4));
    }

    return points;
}


//// 6-point rule, exact for degree xx
//const double a1 = 0.445948490915965;
//const double b1 = 0.108103018168070;
//const double w1 = 0.223381589678011;
//
//const double a2 = 0.091576213509771;
//const double b2 = 0.816847572980459;
//const double w2 = 0.109951743655322;
//
//points = {
//    // 3 permutations of (a1, a1, b1) — symmetric about centroid
//    { a1, a1, w1 },
//    { b1, a1, w1 },
//    { a1, b1, w1 },
//    // 1 permutation of (a2, a2, b2)
//    { a2, a2, w2 },
//    { b2, a2, w2 },
//    { a2, b2, w2 }
//};



std::vector<integration_point> integration_rules::get_quad_gauss_points(int polynomial_order)
{
    // n = points per direction (2..5); builds tensor product
     int n = polynomial_order + 1;

    std::vector<std::pair<double, double>> gauss_1d = get_1d_gauss_points(n);

    std::vector<integration_point> points;

    for (const auto& [eta, wj] : gauss_1d)
    {
        for (const auto& [xi, wi] : gauss_1d)
        {
            points.push_back({ xi, eta, wi * wj });
        }
    }

    return points;

}



// Get Gauss-Legendre integration points for 1D
std::vector<std::pair<double, double>> integration_rules::get_1d_gauss_points(int order)
{
    std::vector<std::pair<double, double>> points;

    switch (order)
    {
    case 1:  // 1-point rule
        points = { {0.0, 2.0} };
        break;
    case 2:  // 2-point rule
        points = { {-0.5773502691896257, 1.0},
                  {0.5773502691896257, 1.0} };
        break;
    case 3:  // 3-point rule
        points = { {-0.7745966692414834, 0.5555555555555556},
                  {0.0, 0.8888888888888888},
                  {0.7745966692414834, 0.5555555555555556} };
        break;
    case 4:  // 4-point rule
        points = { {-0.8611363115940526, 0.3478548451374538},
                  {-0.3399810435848563, 0.6521451548625461},
                  {0.3399810435848563, 0.6521451548625461},
                  {0.8611363115940526, 0.3478548451374538} };
        break;
    case 5:  // 5-point rule
        points = { {-0.9061798459386640, 0.2369268850561891},
                  {-0.5384693101056831, 0.4786286704993665},
                  {0.0, 0.5688888888888889},
                  {0.5384693101056831, 0.4786286704993665},
                  {0.9061798459386640, 0.2369268850561891} };
        break;
    default:
        // Fallback to higher order
        return get_1d_gauss_points(std::min(order, 5));
    }

    return points;

}









