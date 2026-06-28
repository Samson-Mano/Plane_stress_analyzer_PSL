#include "integration_rules.h"

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

// Get 2D Gauss-Legendre integration points for quadrilaterals
std::vector<integration_point> integration_rules::get_quad_2d_gauss_points(int order)
{
    std::vector<integration_point> points;
    auto gauss_1d = get_1d_gauss_points(order);


    for (const auto& [eta, wj] : gauss_1d)
    {
        for (const auto& [xi, wi] : gauss_1d)
        {
            points.push_back({ xi, eta, wi * wj });
        }
    }

    return points;

}

// Get integration points for triangles (using Gauss quadrature)
std::vector<integration_point> integration_rules::get_tri_gauss_points(int order)
{
    std::vector<integration_point> points;


    // 6-point rule
    // Orbit 1 — closer to the vertices
    constexpr double a1 = 0.0915762135097700;   // L1=L2=a1, L3=1-2*a1
    constexpr double w1 = 0.0549758718276610;
    // Orbit 2 — closer to the centroid
    constexpr double a2 = 0.4459484909159650;   // L1=L2=a2, L3=1-2*a2
    constexpr double w2 = 0.1116907948390050;


    // 7-point rule
    // Centroid
    constexpr double w0 = 0.225 / 2.0;

    // Orbit a — closer to the vertices (small a)
    constexpr double oa = 0.1012865073235;      // L1=L2=oa, L3=1-2*oa
    constexpr double wa = 0.1259391805448 / 2.0;

    // Orbit b — between centroid and edge midpoints
    constexpr double ob = 0.4701420641051;      // L1=L2=ob, L3=1-2*ob
    constexpr double wb = 0.1323941527885 / 2.0;


    switch (order)
    {
    case 1:  // 1-point rule (linear)
        points = { {1.0 / 3.0, 1.0 / 3.0, 0.5} };
        break;
    case 2:  // 3-point rule (quadratic)
        points = { {0.5, 0.0, 1.0 / 6.0},
                  {0.5, 0.5, 1.0 / 6.0},
                  {0.0, 0.5, 1.0 / 6.0} };
        break;
    case 3:  // 6-point rule — exact for degree 4 polynomials (Dunavant rule 4)
        // Two 3-point orbits with different radii and weights.
        // sum(w) = 3*(w1 + w2) = 3*(0.054975871827661 + 0.111690794839005) = 1/2
        points = {
            // Orbit 1: permutations of (a1, a1, 1-2*a1)
            {a1,          a1,          w1},
            {1.0 - 2.0 * a1,  a1,          w1},
            {a1,          1.0 - 2.0 * a1,  w1},
            // Orbit 2: permutations of (a2, a2, 1-2*a2)
            {a2,          a2,          w2},
            {1.0 - 2.0 * a2,  a2,          w2},
            {a2,          1.0 - 2.0 * a2,  w2}
        };
        break;
    case 4:  // 7-point rule — exact for degree 5 polynomials (Dunavant rule 5)
        // One centroid point + two 3-point orbits.
        // sum(w) = w0 + 3*(wa + wb)
        //        = 0.225/2 + 3*(0.125939180544827/2 + 0.132394152788506/2) = 1/2

        points = {
            // Centroid
            {1.0 / 3.0,       1.0 / 3.0,       w0},
            // Orbit a
            {oa,             oa,             wa},
            {1.0 - 2.0 * oa,     oa,             wa},
            {oa,             1.0 - 2.0 * oa,     wa},
            // Orbit b
            {ob,             ob,             wb},
            {1.0 - 2.0 * ob,     ob,             wb},
            {ob,             1.0 - 2.0 * ob,     wb}
        };
        break;
    default:
        // Use lower order
        return get_tri_gauss_points(std::min(order, 4));
    }

    return points;


}




