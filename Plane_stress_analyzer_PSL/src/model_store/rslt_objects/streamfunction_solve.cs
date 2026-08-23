using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static src.model_store.rslt_objects.streamlines_solve;

using MathNet.Numerics.LinearAlgebra;


namespace src.model_store.rslt_objects
{
    public class streamfunction_solve
    {
        internal sealed class nodedata_store
        {
            public int point_id { get; set; }
            public OpenTK.Vector2 location { get; set; }

            // New stress tensor components
            public double sigmaXX { get; set; }
            public double sigmaYY { get; set; }
            public double sigmaXY { get; set; }

        }


        internal sealed class triangledata_store
        {
            public int tri_id { get; set; }
            public int pt_id1 { get; set; }
            public int pt_id2 { get; set; }
            public int pt_id3 { get; set; }

        }



        internal sealed class StressTensor
        {
            public double SigmaXX { get; set; }
            public double SigmaYY { get; set; }
            public double SigmaXY { get; set; }

            public StressTensor(double sigmaXX, double sigmaYY, double sigmaXY)
            {
                SigmaXX = sigmaXX;
                SigmaYY = sigmaYY;
                SigmaXY = sigmaXY;
            }

            // Compute principal stresses
            public (double sigma1, double sigma2, double angle) GetPrincipalStresses()
            {
                double avg = (SigmaXX + SigmaYY) / 2.0  ;
                double diff = (SigmaXX - SigmaYY) / 2.0;
                double radius = Math.Sqrt(diff * diff + SigmaXY * SigmaXY);

                double sigma1 = avg + radius;  // Major principal stress (tension)
                double sigma2 = avg - radius;  // Minor principal stress (compression)

                // Principal angle (in radians)
                double angle = 0.5 * Math.Atan2(2 * SigmaXY, SigmaXX - SigmaYY);

                return (sigma1, sigma2, angle);
            }
        }


        // Stream function values for each node, indexed by point_id
        public Dictionary<int, double> streamfunction_values = new Dictionary<int, double>();


        Dictionary<int, triangledata_store> triangle_data = new Dictionary<int, triangledata_store>();
        Dictionary<int, nodedata_store> point_data = new Dictionary<int, nodedata_store>();

        bool isTensionLine = true; // true for tension lines, false for compression lines


       

        public streamfunction_solve(bool isTensionLine)
        {
            // Initialize the dictionaries
            point_data = new Dictionary<int, nodedata_store>();
            triangle_data = new Dictionary<int, triangledata_store>();

            this.isTensionLine = isTensionLine;
        }

        public void add_point_data(int point_id, OpenTK.Vector2 location, double sigmaXX, double sigmaYY, double sigmaXY)
        {
            nodedata_store data = new nodedata_store
            {
                point_id = point_id,
                location = location,
                sigmaXX = sigmaXX,
                sigmaYY = sigmaYY,
                sigmaXY = sigmaXY
            };
            point_data[point_id] = data;
        }


        public void add_triangle_data(int tri_id, int pt_id1, int pt_id2, int pt_id3)
        {
            triangledata_store data = new triangledata_store
            {
                tri_id = tri_id,
                pt_id1 = pt_id1,
                pt_id2 = pt_id2,
                pt_id3 = pt_id3
            };
            triangle_data[tri_id] = data;
        }



        public void CalculateStreamFunction()
        {

            // Create the node map id
            Dictionary<int, int> nodeid_map = new Dictionary<int, int>();

            int node_index = 0;
            foreach (nodedata_store node in point_data.Values)
            {
                nodeid_map[node.point_id] = node_index;
                node_index++;
            }


            // Create the global K matrix and F vector
            int num_nodes = point_data.Count;

            Matrix<double> K_global = Matrix<double>.Build.Dense(num_nodes, num_nodes);
            Vector<double> F_global = Vector<double>.Build.Dense(num_nodes);

            foreach (triangledata_store tri in triangle_data.Values)
            {
                Matrix3d K_element = getElementKmatrix(tri.tri_id);
                Vector3d F_element = getElementFVector(tri.tri_id);

                // Map local element nodes to global node indices
                int i1 = nodeid_map[tri.pt_id1];
                int i2 = nodeid_map[tri.pt_id2];
                int i3 = nodeid_map[tri.pt_id3];


                // Assemble the global K matrix
                K_global[i1, i1] += K_element.M11;
                K_global[i1, i2] += K_element.M12;
                K_global[i1, i3] += K_element.M13;
                K_global[i2, i1] += K_element.M21;
                K_global[i2, i2] += K_element.M22;
                K_global[i2, i3] += K_element.M23;
                K_global[i3, i1] += K_element.M31;
                K_global[i3, i2] += K_element.M32;
                K_global[i3, i3] += K_element.M33;

                // Assemble the global F vector
                F_global[i1] += F_element.X;
                F_global[i2] += F_element.Y;
                F_global[i3] += F_element.Z;
            }


            // Solve the system of equations K_global * psi = F_global
            // psi = 0.0 at first node

            // Apply boundary condition: psi = 0 at the first node

            // Remove the first row and column from K_global and the first entry from F_global
            Matrix<double> K_reduced = K_global.SubMatrix(1, num_nodes - 1, 1, num_nodes - 1);
            Vector<double> F_reduced = F_global.SubVector(1, num_nodes - 1);

            // Solve for the reduced psi vector
            Vector<double> psi_reduced = K_reduced.Solve(F_reduced);

            // Construct the full psi vector, including the first node with psi = 0
            Vector<double> psi = Vector<double>.Build.Dense(num_nodes);
            psi[0] = 0.0;
            for (int i = 1; i < num_nodes; i++)
            {
                psi[i] = psi_reduced[i - 1];
            }

            // Normalize the stream function values to the range [0, 1]
            double min_psi = psi.Min();
            double max_psi = psi.Max();

            if (max_psi - min_psi > 1e-8) // Avoid division by zero
            {
                for (int i = 0; i < num_nodes; i++)
                {
                    psi[i] = (psi[i] - min_psi) / (max_psi - min_psi);
                }
            }
            else
            {
                // If all values are the same, set them to 0.5 (midpoint of [0, 1])
                for (int i = 0; i < num_nodes; i++)
                {
                    psi[i] = 0.5;
                }
            }




            // Store the stream function values in the dictionary
            streamfunction_values.Clear();

            foreach (nodedata_store node in point_data.Values)
            {
                int global_index = nodeid_map[node.point_id];
                streamfunction_values[node.point_id] = psi[global_index];
            }

        }


        private Matrix3d getElementKmatrix(int element_id)
        {
            triangledata_store tri = triangle_data[element_id];

            // Get the node data for the triangle's vertices
            nodedata_store node1 = point_data[tri.pt_id1];
            nodedata_store node2 = point_data[tri.pt_id2];
            nodedata_store node3 = point_data[tri.pt_id3];

            // Get the node coordinates
            Vector2 p1 = node1.location;
            Vector2 p2 = node2.location;
            Vector2 p3 = node3.location;

            double a1 = (p2.X * p3.Y) - (p3.X * p2.Y);
            double b1 = p2.Y - p3.Y;
            double c1 = p3.X - p2.X;

            double a2 = (p3.X * p1.Y) - (p1.X * p3.Y);
            double b2 = p3.Y - p1.Y;
            double c2 = p1.X - p3.X;

            double a3 = (p1.X * p2.Y) - (p2.X * p1.Y);
            double b3 = p1.Y - p2.Y;
            double c3 = p2.X - p1.X;

            double area = 0.5 * ((p2.X - p1.X) * (p3.Y - p1.Y) - (p3.X - p1.X) * (p2.Y - p1.Y));

            // Construct the K matrix
            double K11 = (b1 * b1 + c1 * c1) / (4 * area);
            double K12 = (b1 * b2 + c1 * c2) / (4 * area);
            double K13 = (b1 * b3 + c1 * c3) / (4 * area);

            double K21 = (b2 * b1 + c2 * c1) / (4 * area);
            double K22 = (b2 * b2 + c2 * c2) / (4 * area);
            double K23 = (b2 * b3 + c2 * c3) / (4 * area);

            double K31 = (b3 * b1 + c3 * c1) / (4 * area);
            double K32 = (b3 * b2 + c3 * c2) / (4 * area);
            double K33 = (b3 * b3 + c3 * c3) / (4 * area);

            return new Matrix3d(
                K11, K12, K13,
                K21, K22, K23,
                K31, K32, K33
            );
        }



        private Vector3d getElementFVector(int element_id)
        {
            triangledata_store tri = triangle_data[element_id];

            // Get the node data for the triangle's vertices
            nodedata_store node1 = point_data[tri.pt_id1];
            nodedata_store node2 = point_data[tri.pt_id2];
            nodedata_store node3 = point_data[tri.pt_id3];


            // Get the node coordinates
            Vector2 p1 = node1.location;
            Vector2 p2 = node2.location;
            Vector2 p3 = node3.location;

            double b1 = p2.Y - p3.Y;
            double c1 = p3.X - p2.X;

            double b2 = p3.Y - p1.Y;
            double c2 = p1.X - p3.X;

            double b3 = p1.Y - p2.Y;
            double c3 = p2.X - p1.X;

            double sigmaXX_avg = (node1.sigmaXX + node2.sigmaXX + node3.sigmaXX) / 3.0;
            double sigmaYY_avg = (node1.sigmaYY + node2.sigmaYY + node3.sigmaYY) / 3.0;
            double tauXY_avg = (node1.sigmaXY + node2.sigmaXY + node3.sigmaXY) / 3.0;


            double sigma_avg = (sigmaXX_avg + sigmaYY_avg) / 2.0;
            double sigma_diff = (sigmaXX_avg - sigmaYY_avg) / 2.0;
            double Radius = Math.Sqrt((sigma_diff * sigma_diff) + (tauXY_avg * tauXY_avg));

   

            double F1 = 0.0, F2 = 0.0, F3 = 0.0;

            // Construct the F Vector
            if (this.isTensionLine == true)
            {
                // Tension lines (major principal stress)
                F1 = (-tauXY_avg * b1 + sigma_diff * c1);
                F2 = (-tauXY_avg * b2 + sigma_diff * c2);
                F3 = (-tauXY_avg * b3 + sigma_diff * c3);
            }
            else
            {
                // Compression lines (minor principal stress)
                F1 = (tauXY_avg * b1 - sigma_diff * c1);
                F2 = (tauXY_avg * b2 - sigma_diff * c2);
                F3 = (tauXY_avg * b3 - sigma_diff * c3);
            }


            // Construct the F vector
            F1 = (1.0 / (2.0 * Radius)) * F1;
            F2 = (1.0 / (2.0 * Radius)) * F2;
            F3 = (1.0 / (2.0 * Radius)) * F3;

            return new Vector3d(F1, F2, F3);

        }





    }
}
