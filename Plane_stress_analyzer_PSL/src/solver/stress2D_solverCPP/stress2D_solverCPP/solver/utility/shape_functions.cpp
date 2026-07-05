#include "shape_functions.h"


// Get shape functions for quadrilateral elements
std::vector<double> shape_functions::get_quad_shape_functions(int order, double xi, double eta)
{
    std::vector<double> N;

    if (order == 1)
    {
        // Q4: Linear quadrilateral
        N = {
            (1.0 - xi) * (1.0 - eta) * 0.25,
            (1.0 + xi) * (1.0 - eta) * 0.25,
            (1.0 + xi) * (1.0 + eta) * 0.25,
            (1.0 - xi) * (1.0 + eta) * 0.25
        };
    }
    else if (order == 2)
    {
        // Q9: Quadratic quadrilateral
        double xi2 = xi * xi;
        double eta2 = eta * eta;

        N = {
            // Corner nodes
            0.25 * xi * (xi - 1.0) * eta * (eta - 1.0),
            0.25 * xi * (xi + 1.0) * eta * (eta - 1.0),
            0.25 * xi * (xi + 1.0) * eta * (eta + 1.0),
            0.25 * xi * (xi - 1.0) * eta * (eta + 1.0),
            // Edge nodes
            0.5 * (1.0 - xi2) * eta * (eta - 1.0),
            0.5 * xi * (xi + 1.0) * (1.0 - eta2),
            0.5 * (1.0 - xi2) * eta * (eta + 1.0),
            0.5 * xi * (xi - 1.0) * (1.0 - eta2),
            // Center node
            (1.0 - xi2) * (1.0 - eta2)
        };
    }
    else if (order == 3 || order == 4)
    {
        // Q16: Cubic quadrilateral
        // Q25: Quartic quadrilateral

        // Orders 3 (Q16) and 4 (Q25): build via tensor product of 1-D Lagrange
        // polynomials evaluated at the Gauss–Lobatto node positions.
        //
        // Nodes are ordered along xi first (columns), then eta (rows):
        //   flat index  m = col + row * (order+1),  col in xi-direction
        //
        // N_m(xi,eta) = L_col(xi) * L_row(eta)

        const std::vector<double> nodes = quad_nodes(order);
        int np = static_cast<int>(nodes.size());           // order+1 nodes per direction


		// Corner nodes
		N.push_back(lagrange1d(0, xi, nodes) * lagrange1d(0, eta, nodes));
		N.push_back(lagrange1d(np - 1, xi, nodes) * lagrange1d(0, eta, nodes));
		N.push_back(lagrange1d(np - 1, xi, nodes) * lagrange1d(np - 1, eta, nodes));
		N.push_back(lagrange1d(0, xi, nodes) * lagrange1d(np - 1, eta, nodes));



        // Edge nodes
        // Edge 1
        for (int col = 1; col < np - 1; ++col)
        {
            N.push_back(lagrange1d(col, xi, nodes) * lagrange1d(0, eta, nodes));
        }

        // Edge 2
        for (int row = 1; row < np - 1; ++row)
        {
            N.push_back(lagrange1d(np-1, xi, nodes) * lagrange1d(row, eta, nodes));
        }


        // Edge 3
        for (int col = np - 2; col > 0; --col)
        {
            N.push_back(lagrange1d(col, xi, nodes) * lagrange1d(np - 1, eta, nodes));
        }

        // Edge 4
        for (int row = np - 2; row > 0; --row)
        {
            N.push_back(lagrange1d(0, xi, nodes) * lagrange1d(row, eta, nodes));
        }


        // Internal nodes
        for (int row = 1; row < np - 1; ++row)
        {
            for (int col = 1; col < np - 1; ++col)
            {
                N.push_back(lagrange1d(col, xi, nodes) * lagrange1d(row, eta, nodes));
            }
        }

    }

    return N;

}



// Get shape functions for triangle elements
std::vector<double> shape_functions::get_tri_shape_functions(int order, double xi, double eta)
{
    std::vector<double> N;

    if (order == 1)
    {
        // T3: Linear triangle
        double zeta = 1.0 - xi - eta;  // Third area coordinate

        N = { xi, eta, zeta };
    }
    else if (order == 2)
    {
        // T6: Quadratic triangle
        double zeta = 1.0 - xi - eta;  // Third area coordinate

        N = {
            // Corner nodes
            (2.0 * xi - 1.0) * xi,
            (2.0 * eta - 1.0) * eta,
            (2.0 * zeta - 1.0) * zeta,
            // Edge nodes
            4.0 * xi * eta,
            4.0 * eta * zeta,
            4.0 * zeta * xi
        };
    }
    else if (order == 3 || order == 4)
    {
        // T10: Cubic triangle
        // T15: Quartic triangle
        
        // Orders 3 (T10) and 4 (T15): Bernstein basis in area coordinates.
        //
        // Node positions in (xi,eta): (i/p, j/p) for i+j+k=p, k=p-i-j>=0.
        // Pascal-triangle ordering (row j=0..p, col i=0..p-j):
        //
        //  T10 (p=3):   T15 (p=4):
        //    N_0  (0,0)    N_0  (0,0)
        //    N_1  (1/3,0)  N_1  (1/4,0)
        //    N_2  (2/3,0)  N_2  (2/4,0)
        //    N_3  (1,0)    N_3  (3/4,0)
        //    N_4  (0,1/3)  N_4  (1,0)
        //    N_5  (1/3,1/3)N_5  (0,1/4)
        //    N_6  (2/3,1/3)N_6  (1/4,1/4)
        //    N_7  (0,2/3)  N_7  (2/4,1/4)
        //    N_8  (1/3,2/3)N_8  (3/4,1/4)
        //    N_9  (0,1)    N_9  (0,2/4)
        //                  N_10 (1/4,2/4)
        //                  N_11 (2/4,2/4)
        //                  N_12 (0,3/4)
        //                  N_13 (1/4,3/4)
        //                  N_14 (0,1)

        std::vector<double> temp_N = tri_bernstein(order, xi, eta);

		if (order == 3)
		{
            // T10
            // Corner nodes
			N.push_back(temp_N[0]); 
            N.push_back(temp_N[3]);
			N.push_back(temp_N[9]);

			// Edge nodes
            // Edge 1
			N.push_back(temp_N[1]);
			N.push_back(temp_N[2]);

            // Edge 2
            N.push_back(temp_N[6]);
            N.push_back(temp_N[8]);

			// Edge 3
			N.push_back(temp_N[7]);
			N.push_back(temp_N[4]);

			// Internal node
			N.push_back(temp_N[5]);

		}
		else if (order == 4)
		{
            // T15
            // Corner nodes
            N.push_back(temp_N[0]);
            N.push_back(temp_N[4]);
            N.push_back(temp_N[14]);

			// Edge nodes
			// Edge 1
			N.push_back(temp_N[1]);
			N.push_back(temp_N[2]);
			N.push_back(temp_N[3]);

			// Edge 2
            N.push_back(temp_N[8]);
            N.push_back(temp_N[11]);
			N.push_back(temp_N[13]);

			// Edge 3
            N.push_back(temp_N[12]);
            N.push_back(temp_N[9]);
            N.push_back(temp_N[5]);

			// Internal nodes
            N.push_back(temp_N[6]);
            N.push_back(temp_N[7]);
            N.push_back(temp_N[10]);

		}

    }

    return N;

}


// Get derivatives of shape functions w.r.t. natural coordinates
std::vector<std::pair<double, double>> shape_functions::get_quad_shape_derivatives(int order, double xi, double eta)
{
    std::vector<std::pair<double, double>> dN;

    if (order == 1)
    {
        // Q4 derivatives
        dN = {
            {-(1.0 - eta) * 0.25, -(1.0 - xi) * 0.25},
            { (1.0 - eta) * 0.25, -(1.0 + xi) * 0.25},
            { (1.0 + eta) * 0.25,  (1.0 + xi) * 0.25},
            {-(1.0 + eta) * 0.25,  (1.0 - xi) * 0.25}
        };
    }
    else if (order == 2)
    {
        // Q9 derivatives
        // dN/dxi and dN/deta for all 9 nodes.
        double xi2 = xi * xi;
        double eta2 = eta * eta;
        dN = {
            // Corner nodes
            // Node 1: 0.25*xi*(xi-1.0)*eta*(eta-1.0)
            {0.25 * (2.0 * xi - 1.0) * eta * (eta - 1.0),
            0.25 * (2.0 * eta - 1.0) * xi * (xi - 1.0)},
            // Node 2: 0.25*xi*(xi+1.0)*eta*(eta-1.0)
            {0.25 * (2.0 * xi + 1.0) * eta * (eta - 1.0),
            0.25 * (2.0 * eta - 1.0) * xi * (xi + 1.0)},
            // Node 3: 0.25*xi*(xi+1.0)*eta*(eta+1.0)
            {0.25 * (2.0 * xi + 1.0) * eta * (eta + 1.0),
            0.25 * (2.0 * eta + 1.0) * xi * (xi + 1.0)},
            // Node 4: 0.25*xi*(xi-1.0)*eta*(eta+1.0)
            {0.25 * (2.0 * xi - 1.0) * eta * (eta + 1.0),
            0.25 * (2.0 * eta + 1.0) * xi * (xi - 1.0)},
            // Edge nodes
            // Node 5: 0.5*(1.0-xi2)*eta*(eta-1.0)
            {-xi * eta * (eta - 1.0),
            0.5 * (1.0 - xi2) * (2.0 * eta - 1.0)},
            // Node 6: 0.5*xi*(xi+1.0)*(1.0-eta2)
            {0.5 * (2.0 * xi + 1.0) * (1.0 - eta2),
            -eta * xi * (xi + 1.0)},
            // Node 7: 0.5*(1.0-xi2)*eta*(eta+1.0)
            {-xi * eta * (eta + 1.0),
            0.5 * (1.0 - xi2) * (2.0 * eta + 1.0)},
            // Node 8: 0.5*xi*(xi-1.0)*(1.0-eta2)
            {0.5 * (2.0 * xi - 1.0) * (1.0 - eta2),
            -eta * xi * (xi - 1.0)},
            // Center node
            // Node 9: (1.0-xi2)*(1.0-eta2)
            {-2.0 * xi * (1.0 - eta2),
             -2.0 * eta * (1.0 - xi2)}
        };

    }
    else if (order == 3 || order == 4)
    {
        // Q16 derivatives
        // Q25 derivatives

        // Orders 3 & 4: tensor product differentiation.
        const std::vector<double> nodes = quad_nodes(order);
        int np =  static_cast<int>(nodes.size());

        double dNdxi = 0.0;
        double dNdeta = 0.0;

		// Corner nodes
        // Corner 1
		dNdxi = dlagrange1d(0, xi, nodes) * lagrange1d(0, eta, nodes);
		dNdeta = lagrange1d(0, xi, nodes) * dlagrange1d(0, eta, nodes);
		dN.push_back({ dNdxi, dNdeta });

		// Corner 2
		dNdxi = dlagrange1d(np - 1, xi, nodes) * lagrange1d(0, eta, nodes);
		dNdeta = lagrange1d(np - 1, xi, nodes) * dlagrange1d(0, eta, nodes);
		dN.push_back({ dNdxi, dNdeta });

		// Corner 3
		dNdxi = dlagrange1d(np - 1, xi, nodes) * lagrange1d(np - 1, eta, nodes);
		dNdeta = lagrange1d(np - 1, xi, nodes) * dlagrange1d(np - 1, eta, nodes);
		dN.push_back({ dNdxi, dNdeta });

		// Corner 4
		dNdxi = dlagrange1d(0, xi, nodes) * lagrange1d(np - 1, eta, nodes);
		dNdeta = lagrange1d(0, xi, nodes) * dlagrange1d(np - 1, eta, nodes);
		dN.push_back({ dNdxi, dNdeta });


		// Edge nodes
        // Edge 1
		for (int col = 1; col < np - 1; ++col)
		{
			dNdxi = dlagrange1d(col, xi, nodes) * lagrange1d(0, eta, nodes);
			dNdeta = lagrange1d(col, xi, nodes) * dlagrange1d(0, eta, nodes);
			dN.push_back({ dNdxi, dNdeta });
		}

        // Edge 2
        for (int row = 1; row < np - 1; ++row)
        {
			dNdxi = dlagrange1d(np - 1, xi, nodes) * lagrange1d(row, eta, nodes);
			dNdeta = lagrange1d(np - 1, xi, nodes) * dlagrange1d(row, eta, nodes);
			dN.push_back({ dNdxi, dNdeta });
        }


		// Edge 3
		for (int col = np - 2; col > 0; --col)
		{
			dNdxi = dlagrange1d(col, xi, nodes) * lagrange1d(np - 1, eta, nodes);
			dNdeta = lagrange1d(col, xi, nodes) * dlagrange1d(np - 1, eta, nodes);
			dN.push_back({ dNdxi, dNdeta });
		}

		// Edge 4
		for (int row = np - 2; row > 0; --row)
		{
			dNdxi = dlagrange1d(0, xi, nodes) * lagrange1d(row, eta, nodes);
			dNdeta = lagrange1d(0, xi, nodes) * dlagrange1d(row, eta, nodes);
			dN.push_back({ dNdxi, dNdeta });
		}


        
        // Internal nodes
        for (int row = 1; row < np - 1; ++row)
        {
            for (int col = 1; col < np - 1; ++col)
            {
                dNdxi = dlagrange1d(col, xi, nodes) * lagrange1d(row, eta, nodes);
                dNdeta = lagrange1d(col, xi, nodes) * dlagrange1d(row, eta, nodes);
                dN.push_back({ dNdxi, dNdeta });
            }
        }

    }


    return dN;

}



std::vector<std::pair<double, double>> shape_functions::get_tri_shape_derivatives(int order, double xi, double eta)
{
    std::vector<std::pair<double, double>> dN;

    if (order == 1)
    {
        // T3 derivatives
        dN = {
            {1.0, 0.0},
            {0.0, 1.0},
            {-1.0, -1.0}
        };
    }
    else if (order == 2)
    {
        // T6 derivatives
        // T6 derivatives.  zeta = 1 - xi - eta,  dzeta/dxi = dzeta/deta = -1.
        double zeta = 1.0 - xi - eta;

        dN = {
            // Corner nodes
            {4.0 * xi - 1.0, 0.0}, // Node 1: (2*xi - 1)*xi
            {0.0, 4.0 * eta - 1.0},             // Node 2: (2*eta - 1)*eta
            {-(4.0 * zeta - 1.0), -(4.0 * zeta - 1.0)}, // Node 3: (2*zeta - 1)*zeta
            // Edge nodes
            {4.0 * eta, 4.0 * xi}, // Node 4: 4*xi*eta
            {-4.0 * eta, 4.0 * (zeta -  eta)},             // Node 5: 4*eta*zeta
            {4.0 * (zeta -  xi), -4.0 * xi} // Node 6: 4*zeta*xi
        };
    }
    else if (order == 3 || order == 4)
    {
        // T10 derivatives
        // T15 derivatives

         // Orders 3 (T10) and 4 (T15): Bernstein derivatives.

        std::vector<std::pair<double, double>> temp_dN = dtri_bernstein(order, xi, eta);

        if (order == 3)
        {
            // Corner nodes
            dN.push_back(temp_dN[0]);
            dN.push_back(temp_dN[3]);
            dN.push_back(temp_dN[9]);

            // Edge nodes
            // Edge 1
            dN.push_back(temp_dN[1]);
            dN.push_back(temp_dN[2]);

            // Edge 2
            dN.push_back(temp_dN[6]);
            dN.push_back(temp_dN[8]);

            // Edge 3
            dN.push_back(temp_dN[7]);
            dN.push_back(temp_dN[4]);

            // Internal node
            dN.push_back(temp_dN[5]);

        }
        else if (order == 4)
        {
            // T15
            // Corner nodes
            dN.push_back(temp_dN[0]);
            dN.push_back(temp_dN[4]);
            dN.push_back(temp_dN[14]);

            // Edge nodes
            // Edge 1
            dN.push_back(temp_dN[1]);
            dN.push_back(temp_dN[2]);
            dN.push_back(temp_dN[3]);

            // Edge 2
            dN.push_back(temp_dN[8]);
            dN.push_back(temp_dN[11]);
            dN.push_back(temp_dN[13]);

            // Edge 3
            dN.push_back(temp_dN[12]);
            dN.push_back(temp_dN[9]);
            dN.push_back(temp_dN[5]);

            // Internal nodes
            dN.push_back(temp_dN[6]);
            dN.push_back(temp_dN[7]);
            dN.push_back(temp_dN[10]);

        }
    }

    return dN;

}



std::vector<double> shape_functions::quad_nodes(int order)
{
    // Quad element node positions in natural coordinates

    switch (order)
    {
    case 1: return { -1.0,  1.0 };                                // Q4
    case 2: return { -1.0,  0.0,  1.0 };                         // Q9
    case 3: return { -1.0, -1.0 / 3.0,  1.0 / 3.0,  1.0 };          // Q16
    case 4: return { -1.0, -0.5,  0.0,  0.5,  1.0 };            // Q25
    default: throw std::invalid_argument("Unsupported quad order");
    }
}




double shape_functions::lagrange1d(int i, double x, const std::vector<double>& nodes)
{
    // Evaluate the 1-D Lagrange basis polynomial at x given the ordered nodes.
    // L_i(x) = prod_{j!=i} (x - nodes[j]) / (nodes[i] - nodes[j])

    double L = 1.0;

    for (int j = 0; j < static_cast<int>(nodes.size()); ++j)
    {
        if (j != i)
            L *= (x - nodes[j]) / (nodes[i] - nodes[j]);
    }


    return L;
}



double shape_functions::dlagrange1d(int i, double x, const std::vector<double>& nodes)
{
    // Derivative of the 1-D Lagrange basis polynomial.
    // dL_i/dx = sum_{k!=i} [ prod_{j!=i, j!=k} (x - nodes[j]) / (nodes[i] - nodes[j]) ]

    double dL = 0.0;
    int n = static_cast<int>(nodes.size());

    for (int k = 0; k < n; ++k)
    {
        if (k == i) continue;

        double term = 1.0;

        for (int j = 0; j < n; ++j)
        {
            if (j != i && j != k)
            {
                term *= (x - nodes[j]) / (nodes[i] - nodes[j]);
            }
        }
            
        dL += term / (nodes[i] - nodes[k]);
    }

    return dL;

}



long long shape_functions::binom(int n, int k)
{
    // Binomial coefficient C(n, k)
    if (k < 0 || k > n) return 0;

    if (k == 0 || k == n) return 1;

    k = std::min(k, n - k);

    long long result = 1;
    for (int i = 0; i < k; ++i)
    {
        result *= (n - i);
        result /= (i + 1);
    }

    return result;
}



long long shape_functions::trinom(int p, int i, int j, int k)
{ 
    // Trinomial coefficient C(p; i,j,k) = p! / (i! j! k!)  with i+j+k=p
    return binom(p, i) * binom(p - i, j); 

}


std::vector<double> shape_functions::tri_bernstein(int p, double xi, double eta)
{

    // Bernstein shape function for a triangular element of order p.
    // L1=xi, L2=eta, L3=1-xi-eta
    // Node index layout: row j (fixed), column i  =>  flat index for (i,j) with i+j<=p
    // N_{i,j,k}(xi,eta) = trinom(p,i,j,k) * L1^i * L2^j * L3^k,  k=p-i-j
    //
    // Returns all (p+1)(p+2)/2 shape functions ordered by
    //   row j=0..p, column i=0..p-j  (standard Pascal-triangle order)

    double zeta = 1.0 - xi - eta;
    std::vector<double> N;
    // N.reserve((p + 1) * (p + 2) / 2);


    // Pre-compute powers 
    std::vector<double> xi_pow(p + 1, 1.0);
    std::vector<double> eta_pow(p + 1, 1.0);
    std::vector<double> zeta_pow(p + 1, 1.0);


    for (int i = 1; i <= p; ++i) 
    {
        xi_pow[i] = xi_pow[i - 1] * xi;
        eta_pow[i] = eta_pow[i - 1] * eta;
        zeta_pow[i] = zeta_pow[i - 1] * zeta;
    }

    for (int j = 0; j <= p; ++j)
    {
        for (int i = 0; i <= p - j; ++i)
        {
            int k = p - i - j;
            double val = static_cast<double>(trinom(p, i, j, k));
            val *= xi_pow[i] * eta_pow[j] * zeta_pow[k];
            N.push_back(val);
        }
    }
       
    return N;
}




std::vector<std::pair<double, double>> shape_functions::dtri_bernstein(int p, double xi, double eta)
{
    // Partial derivatives of Bernstein tri shape functions.
    // dN/dxi  and  dN/deta
    //   dL1/dxi=1, dL2/dxi=0,  dL3/dxi=-1
    //   dL1/deta=0, dL2/deta=1, dL3/deta=-1
    //
    // dN_{ijk}/dxi  = trinom * [ i*L1^{i-1}*L2^j*L3^k  -  k*L1^i*L2^j*L3^{k-1} ]
    // dN_{ijk}/deta = trinom * [ j*L1^i*L2^{j-1}*L3^k  -  k*L1^i*L2^j*L3^{k-1} ]

    double zeta = 1.0 - xi - eta;
    std::vector<std::pair<double, double>> dN;
    // dN.reserve((p + 1) * (p + 2) / 2);

    // Pre-compute powers
    std::vector<double> xi_pow(p + 1, 1.0);
    std::vector<double> eta_pow(p + 1, 1.0);
    std::vector<double> zeta_pow(p + 1, 1.0);


    for (int i = 1; i <= p; ++i) 
    {
        xi_pow[i] = xi_pow[i - 1] * xi;
        eta_pow[i] = eta_pow[i - 1] * eta;
        zeta_pow[i] = zeta_pow[i - 1] * zeta;
    }


    for (int j = 0; j <= p; ++j)
    {
        for (int i = 0; i <= p - j; ++i)
        {
            int k = p - i - j;
            double C = static_cast<double>(trinom(p, i, j, k));

            // dN/dxi = C * (i*L1^(i-1)*L2^j*L3^k - k*L1^i*L2^j*L3^(k-1))
            double dxi = 0.0;
            if (i > 0) dxi += i * xi_pow[i - 1] * eta_pow[j] * zeta_pow[k];
            if (k > 0) dxi -= k * xi_pow[i] * eta_pow[j] * zeta_pow[k - 1];
            dxi *= C;


            // dN/deta = C * (j*L1^i*L2^(j-1)*L3^k - k*L1^i*L2^j*L3^(k-1))
            double deta = 0.0;
            if (j > 0) deta += j * xi_pow[i] * eta_pow[j - 1] * zeta_pow[k];
            if (k > 0) deta -= k * xi_pow[i] * eta_pow[j] * zeta_pow[k - 1];
            deta *= C;

            dN.push_back({ dxi, deta });
        }
    }

    return dN;
}






