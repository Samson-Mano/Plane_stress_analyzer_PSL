using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static src.model_store.rslt_objects.streamlines_solve;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Collections.Concurrent;



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


        internal sealed class MatrixEntry
        {
            public int Row { get; set; }
            public int Col { get; set; }
            public double Value { get; set; }
        }

        internal sealed class VectorEntry
        {
            public int Node { get; set; }
            public double Value { get; set; }
        }

      

        // Stream function values for each node, indexed by point_id
        public Dictionary<int, double> streamfunction_values = new Dictionary<int, double>();


        Dictionary<int, triangledata_store> triangle_data = new Dictionary<int, triangledata_store>();
        Dictionary<int, nodedata_store> point_data = new Dictionary<int, nodedata_store>();

        bool isTensionLine = true; // true for tension lines, false for compression lines


        private ConcurrentBag<MatrixEntry> matrixEntries;
        private ConcurrentBag<VectorEntry> vectorEntries;

        public streamfunction_solve(bool isTensionLine)
        {
            // Initialize the dictionaries
            point_data = new Dictionary<int, nodedata_store>();
            triangle_data = new Dictionary<int, triangledata_store>();

            matrixEntries = new ConcurrentBag<MatrixEntry>();
            vectorEntries = new ConcurrentBag<VectorEntry>();

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

            int num_nodes = point_data.Count;

            // Use coordinate storage for efficient assembly
            matrixEntries = new ConcurrentBag<MatrixEntry>();
            vectorEntries = new ConcurrentBag<VectorEntry>();

            // Parallel assembly of both K and F
            Parallel.ForEach(triangle_data.Values, tri =>
            {
                Matrix3d localK = getElementKMatrix(tri.tri_id);
                Vector3d localF = getElementFVector(tri.tri_id);

                // Get global node indices
                int g1 = nodeid_map[tri.pt_id1];
                int g2 = nodeid_map[tri.pt_id2];
                int g3 = nodeid_map[tri.pt_id3];
                int[] globalNodes = new int[] { g1, g2, g3 };

                // Add K contributions
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        double value = localK[i, j];
                        if (Math.Abs(value) > 1e-15)
                        {
                            matrixEntries.Add(new MatrixEntry
                            {
                                Row = globalNodes[i],
                                Col = globalNodes[j],
                                Value = value
                            });
                        }
                    }
                }

                // Add F contributions
                vectorEntries.Add(new VectorEntry { Node = g1, Value = localF.X });
                vectorEntries.Add(new VectorEntry { Node = g2, Value = localF.Y });
                vectorEntries.Add(new VectorEntry { Node = g3, Value = localF.Z });
            });


            // Build sparse matrix from coordinate format efficiently
            // Use ToArray() only once per array
            var entriesArray = matrixEntries.ToArray();
            int entryCount = entriesArray.Length;

            var rowIndices = new int[entryCount];
            var colIndices = new int[entryCount];
            var values = new double[entryCount];

            for (int i = 0; i < entryCount; i++)
            {
                rowIndices[i] = entriesArray[i].Row;
                colIndices[i] = entriesArray[i].Col;
                values[i] = entriesArray[i].Value;
            }

            // Create sparse matrix
            var sparseK = Matrix<double>.Build.SparseFromCoordinateFormat(
                num_nodes, num_nodes, entryCount,
                rowIndices, colIndices, values);

            // Assemble F vector from parallel contributions
            var F_global = Vector<double>.Build.Dense(num_nodes);


            foreach (VectorEntry fEntry in vectorEntries.ToArray())
            {
                F_global[fEntry.Node] += fEntry.Value;
            }


            // Apply boundary condition more efficiently for sparse matrices
            // Instead of SubMatrix (which creates a new dense matrix), use a different approach

            // Option 1: If you want to keep the original approach
            // var K_reduced = sparseK.SubMatrix(1, num_nodes - 1, 1, num_nodes - 1);
            // var F_reduced = F_global.SubVector(1, num_nodes - 1);
            // Vector<double> psi_reduced = K_reduced.Solve(F_reduced);

            // Option 2: More efficient for sparse matrices - modify in place
            // Set the first row/column to identity
            var psi = Vector<double>.Build.Dense(num_nodes);

            // Create a copy of the matrix for modification
            var K_modified = sparseK.Clone();

            // Set first row: K[0,0] = 1, K[0,j] = 0 for j>0
            for (int j = 1; j < num_nodes; j++)
            {
                K_modified[0, j] = 0.0;
            }
            K_modified[0, 0] = 1.0;

            // Set first column: K[i,0] = 0 for i>0
            for (int i = 1; i < num_nodes; i++)
            {
                K_modified[i, 0] = 0.0;
            }

            // Set F[0] = 0 (boundary condition)
            F_global[0] = 0.0;

            // Solve the modified system
            psi = K_modified.Solve(F_global);

            // Normalize the stream function values to the range [0, 1]
            double min_psi = psi.Min();
            double max_psi = psi.Max();

            if (max_psi - min_psi > 1e-8)
            {
                // Vectorized normalization
                double range = max_psi - min_psi;
                for (int i = 0; i < num_nodes; i++)
                {
                    psi[i] = (psi[i] - min_psi) / range;
                }
            }
            else
            {
                // If all values are the same, set them to 0.5
                psi = Vector<double>.Build.Dense(num_nodes, 0.5);
            }



            // Store the stream function values in the dictionary
            streamfunction_values.Clear();

            foreach (nodedata_store node in point_data.Values)
            {
                int global_index = nodeid_map[node.point_id];
                streamfunction_values[node.point_id] = psi[global_index];
            }

        }


        private Matrix3d getElementKMatrix(int element_id)
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

            nodedata_store node1 = point_data[tri.pt_id1];
            nodedata_store node2 = point_data[tri.pt_id2];
            nodedata_store node3 = point_data[tri.pt_id3];

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

            double sigma_diff = (sigmaXX_avg - sigmaYY_avg) / 2.0;
            double R = Math.Sqrt(sigma_diff * sigma_diff + tauXY_avg * tauXY_avg);

            // Degenerate/isotropic point (sigma_xx = sigma_yy, tau_xy = 0):
            // principal direction is undefined. Element contributes nothing.
            if (R < 1e-9)
                return new Vector3d(0.0, 0.0, 0.0);

            double cos2theta = sigma_diff / R;                                   // clamp guards fp drift
            double costheta = Math.Sqrt(Math.Max(0.0, (1.0 + cos2theta) / 2.0));
            double sintheta = Math.Sqrt(Math.Max(0.0, (1.0 - cos2theta) / 2.0));
            if (tauXY_avg < 0.0) sintheta = -sintheta;

            double gx, gy; // = target ∇phi direction (unit vector)

            if (this.isTensionLine)
            {
                // grad(phi) = minor principal direction (perp to major/tension direction)
                gx = -sintheta;
                gy = costheta;
            }
            else
            {
                // grad(phi) = major principal direction (perp to minor/compression direction)
                gx = costheta;
                gy = sintheta;
            }

            // F_i = ∫ ∇N_i · (gx,gy) dA = (1/2)(b_i*gx + c_i*gy)   [since ∇N_i = (b_i,c_i)/(2A), integrated over area A]
            double F1 = 0.5 * (b1 * gx + c1 * gy);
            double F2 = 0.5 * (b2 * gx + c2 * gy);
            double F3 = 0.5 * (b3 * gx + c3 * gy);

            return new Vector3d(F1, F2, F3);
        }




    }
}
