#pragma once
#include <vector>
#include <iostream>

class shape_functions
{
public:
    // Get shape functions for quadrilateral elements
    static std::vector<double> get_quad_shape_functions(int order, double xi, double eta);

    // Get shape functions for triangle elements
    static std::vector<double> get_tri_shape_functions(int order, double xi, double eta);

    // Get derivatives of shape functions w.r.t. natural coordinates
    static std::vector<std::pair<double, double>> get_quad_shape_derivatives(int order, double xi, double eta);

    static std::vector<std::pair<double, double>> get_tri_shape_derivatives(int order, double xi, double eta);


private:

    static  double lagrange1d(int i, double x, const std::vector<double>& nodes);

    static double dlagrange1d(int i, double x, const std::vector<double>& nodes);

    static long long binom(int n, int k);

    static long long trinom(int p, int i, int j, int k);

    static std::vector<double> tri_bernstein(int p, double xi, double eta);

    static std::vector<std::pair<double, double>> dtri_bernstein(int p, double xi, double eta);

    static std::vector<double> quad_nodes(int order);




};