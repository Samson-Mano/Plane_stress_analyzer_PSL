using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using Plane_stress_analyzer_PSL.src.global_variables;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace src.model_store.rslt_objects
{


    public class streamlines_solve
    {

        internal sealed class nodedata_store
        {
            public int point_id { get; set; }
            public OpenTK.Vector2 location { get; set; }

            // New stress tensor components
            public float sigmaXX { get; set; }
            public float sigmaYY { get; set; }
            public float sigmaXY { get; set; }

        }


        internal sealed class triangledata_store
        {
            public int tri_id { get; set; }
            public int pt_id1 { get; set; }
            public int pt_id2 { get; set; }
            public int pt_id3 { get; set; }

        }

        internal sealed class boundaryedges_store
        {
            public int edge_id { get; set; }
            public int pt_id1 { get; set; }
            public int pt_id2 { get; set; }

            public OpenTK.Vector2 midpoint { get; set; }
            public OpenTK.Vector2 outward_normal { get; set; }

        }



        internal sealed class StressTensor
        {
            public float SigmaXX { get; set; }
            public float SigmaYY { get; set; }
            public float SigmaXY { get; set; }

            public StressTensor(float sigmaXX, float sigmaYY, float sigmaXY)
            {
                SigmaXX = sigmaXX;
                SigmaYY = sigmaYY;
                SigmaXY = sigmaXY;
            }

            // Compute principal stresses
            public (float sigma1, float sigma2, float angle) GetPrincipalStresses()
            {
                float avg = (SigmaXX + SigmaYY) / 2f;
                float diff = (SigmaXX - SigmaYY) / 2f;
                float radius = (float)Math.Sqrt(diff * diff + SigmaXY * SigmaXY);

                float sigma1 = avg + radius;  // Major principal stress (tension)
                float sigma2 = avg - radius;  // Minor principal stress (compression)

                // Principal angle (in radians)
                float angle = 0.5f * (float)Math.Atan2(2 * SigmaXY, SigmaXX - SigmaYY);

                return (sigma1, sigma2, angle);
            }
        }



        private StressTensor GetStressAtPoint(OpenTK.Vector2 point)
        {
            // Find the triangle containing this point
            var triangle = FindContainingTriangle(point);
            if (triangle == null) return null;

            // Get the three vertices and their stress tensors
            var v1 = point_data[triangle.pt_id1];
            var v2 = point_data[triangle.pt_id2];
            var v3 = point_data[triangle.pt_id3];

            // Compute barycentric coordinates
            var weights = ComputeBarycentricWeights(point, v1.location, v2.location, v3.location);

            // Interpolate stress tensor components
            // Assuming you have stress tensor data (σxx, σyy, σxy) at each node
            float sigmaXX = weights.w1 * v1.sigmaXX + weights.w2 * v2.sigmaXX + weights.w3 * v3.sigmaXX;
            float sigmaYY = weights.w1 * v1.sigmaYY + weights.w2 * v2.sigmaYY + weights.w3 * v3.sigmaYY;
            float sigmaXY = weights.w1 * v1.sigmaXY + weights.w2 * v2.sigmaXY + weights.w3 * v3.sigmaXY;

            return new StressTensor(sigmaXX, sigmaYY, sigmaXY);
        }


        // Get stress line direction (reusing your existing method)
        private OpenTK.Vector2 GetStressLineDirection(OpenTK.Vector2 point, bool isTensionLine)
        {
            var stress = GetStressAtPoint(point);
            if (stress == null) return OpenTK.Vector2.Zero;

            var (sigma1, sigma2, angle) = stress.GetPrincipalStresses();

            // Check for singularities or very large stresses
            float maxStress = Math.Max(Math.Abs(sigma1), Math.Abs(sigma2));
            if (maxStress > 1e10f) // Threshold for singularity
            {
                return OpenTK.Vector2.Zero;
            }

            if (isTensionLine)
            {
                // Major principal stress direction (tension)
                return new OpenTK.Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            }
            else
            {
                // Minor principal stress direction (compression) - perpendicular to tension
                return new OpenTK.Vector2((float)Math.Cos(angle + Math.PI / 2),
                                          (float)Math.Sin(angle + Math.PI / 2));
            }
        }



        public class streamline_result
        {
            public List<OpenTK.Vector2> streamline_points { get; set; } // connected points along the streamline
            public List<double> streamline_magnitudes { get; set; }
            public streamline_result()
            {
                streamline_points = new List<OpenTK.Vector2>();
                streamline_magnitudes = new List<double>();
            }
        }



        Dictionary<int, triangledata_store> triangle_data = new Dictionary<int, triangledata_store>();
        Dictionary<int, nodedata_store> point_data = new Dictionary<int, nodedata_store>();
        List<boundaryedges_store> boundary_edges = new List<boundaryedges_store>();

        public List<streamline_result> streamlines { get; set; } = new List<streamline_result>();

        bool isTensionLine = true; // true for tension lines, false for compression lines

        private Dictionary<int, List<triangledata_store>> spatialGrid;
        private float gridSize;
        private bool spatialGridBuilt = false;



        #region Triangle Search and Barycentric Methods

        private triangledata_store FindContainingTriangle(OpenTK.Vector2 point)
        {
            // Build spatial grid if not already built
            if (!spatialGridBuilt)
            {
                BuildSpatialGrid();
                spatialGridBuilt = true;
            }

            // Check spatial grid first
            var triangle = FindTriangleSpatial(point);
            if (triangle != null)
                return triangle;

            // Fallback: brute force
            return FindTriangleBruteForce(point);
        }

        private void BuildSpatialGrid()
        {
            // Get mesh bounds
            var bounds = GetMeshBounds();
            float width = bounds.Right - bounds.Left;
            float height = bounds.Top - bounds.Bottom;

            // Use adaptive grid size based on mesh density
            int numCells = Math.Max(10, (int)Math.Sqrt(triangle_data.Count / 10));
            gridSize = Math.Max(width, height) / numCells;

            spatialGrid = new Dictionary<int, List<triangledata_store>>();

            foreach (var triangle in triangle_data.Values)
            {
                // Get triangle bounding box
                var v1 = point_data[triangle.pt_id1].location;
                var v2 = point_data[triangle.pt_id2].location;
                var v3 = point_data[triangle.pt_id3].location;

                float minX = Math.Min(v1.X, Math.Min(v2.X, v3.X));
                float maxX = Math.Max(v1.X, Math.Max(v2.X, v3.X));
                float minY = Math.Min(v1.Y, Math.Min(v2.Y, v3.Y));
                float maxY = Math.Max(v1.Y, Math.Max(v2.Y, v3.Y));

                int cellMinX = (int)(minX / gridSize);
                int cellMaxX = (int)(maxX / gridSize);
                int cellMinY = (int)(minY / gridSize);
                int cellMaxY = (int)(maxY / gridSize);

                for (int x = cellMinX; x <= cellMaxX; x++)
                {
                    for (int y = cellMinY; y <= cellMaxY; y++)
                    {
                        int key = GetCellKey(x, y);
                        if (!spatialGrid.ContainsKey(key))
                            spatialGrid[key] = new List<triangledata_store>();
                        spatialGrid[key].Add(triangle);
                    }
                }
            }
        }

        private triangledata_store FindTriangleSpatial(OpenTK.Vector2 point)
        {
            int cellX = (int)(point.X / gridSize);
            int cellY = (int)(point.Y / gridSize);
            int key = GetCellKey(cellX, cellY);

            // Check main cell
            if (spatialGrid.ContainsKey(key))
            {
                foreach (var triangle in spatialGrid[key])
                {
                    if (IsPointInTriangle(point, triangle))
                        return triangle;
                }
            }

            // Check neighboring cells
            int[] offsets = { -1, 0, 1 };
            foreach (int dx in offsets)
            {
                foreach (int dy in offsets)
                {
                    if (dx == 0 && dy == 0) continue;
                    int neighborKey = GetCellKey(cellX + dx, cellY + dy);

                    if (spatialGrid.ContainsKey(neighborKey))
                    {
                        foreach (var triangle in spatialGrid[neighborKey])
                        {
                            if (IsPointInTriangle(point, triangle))
                                return triangle;
                        }
                    }
                }
            }

            return null;
        }

        private triangledata_store FindTriangleBruteForce(OpenTK.Vector2 point)
        {
            foreach (var triangle in triangle_data.Values)
            {
                if (IsPointInTriangle(point, triangle))
                    return triangle;
            }
            return null;
        }

        private bool IsPointInTriangle(OpenTK.Vector2 point, triangledata_store triangle)
        {
            var v1 = point_data[triangle.pt_id1].location;
            var v2 = point_data[triangle.pt_id2].location;
            var v3 = point_data[triangle.pt_id3].location;

            return IsPointInTriangle(point, v1, v2, v3);
        }

        private bool IsPointInTriangle(OpenTK.Vector2 p, OpenTK.Vector2 a, OpenTK.Vector2 b, OpenTK.Vector2 c)
        {
            OpenTK.Vector2 v0 = c - a;
            OpenTK.Vector2 v1 = b - a;
            OpenTK.Vector2 v2 = p - a;

            float dot00 = OpenTK.Vector2.Dot(v0, v0);
            float dot01 = OpenTK.Vector2.Dot(v0, v1);
            float dot02 = OpenTK.Vector2.Dot(v0, v2);
            float dot11 = OpenTK.Vector2.Dot(v1, v1);
            float dot12 = OpenTK.Vector2.Dot(v1, v2);

            float invDenom = 1f / (dot00 * dot11 - dot01 * dot01);
            float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            float epsilon = -1e-6f;
            return (u >= epsilon) && (v >= epsilon) && (u + v <= 1 - epsilon);
        }

        private (float w1, float w2, float w3) ComputeBarycentricWeights(
            OpenTK.Vector2 point,
            OpenTK.Vector2 v1,
            OpenTK.Vector2 v2,
            OpenTK.Vector2 v3)
        {
            OpenTK.Vector2 v0 = v2 - v1;
            OpenTK.Vector2 v1_vec = v3 - v1;
            OpenTK.Vector2 v2_vec = point - v1;

            float dot00 = OpenTK.Vector2.Dot(v0, v0);
            float dot01 = OpenTK.Vector2.Dot(v0, v1_vec);
            float dot02 = OpenTK.Vector2.Dot(v0, v2_vec);
            float dot11 = OpenTK.Vector2.Dot(v1_vec, v1_vec);
            float dot12 = OpenTK.Vector2.Dot(v1_vec, v2_vec);

            float invDenom = 1f / (dot00 * dot11 - dot01 * dot01);
            float w2 = (dot11 * dot02 - dot01 * dot12) * invDenom;
            float w3 = (dot00 * dot12 - dot01 * dot02) * invDenom;
            float w1 = 1f - w2 - w3;

            // Clamp for numerical stability
            w1 = (float)gvariables_static.Clamp(w1, 0, 1);
            w2 = (float)gvariables_static.Clamp(w2, 0, 1);
            w3 = (float)gvariables_static.Clamp(w3, 0, 1);

            // Re-normalize to ensure sum = 1
            float sum = w1 + w2 + w3;
            if (sum > 0)
            {
                w1 /= sum;
                w2 /= sum;
                w3 /= sum;
            }

            return (w1, w2, w3);
        }

        private int GetCellKey(int x, int y)
        {
            // Simple key encoding for 2D grid
            return (x << 16) | (y & 0xFFFF);
        }

        private RectangleF GetMeshBounds()
        {
            float minX = float.MaxValue, minY = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue;

            foreach (var point in point_data.Values)
            {
                if (point.location.X < minX) minX = point.location.X;
                if (point.location.X > maxX) maxX = point.location.X;
                if (point.location.Y < minY) minY = point.location.Y;
                if (point.location.Y > maxY) maxY = point.location.Y;
            }

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        #endregion



        public streamlines_solve(bool isTensionLine)
        {
            // Initialize the dictionaries
            point_data = new Dictionary<int, nodedata_store>();
            triangle_data = new Dictionary<int, triangledata_store>();
            boundary_edges = new List<boundaryedges_store>();

            this.isTensionLine = isTensionLine;
        }

        public void add_point_data(int point_id, OpenTK.Vector2 location, float sigmaXX, float sigmaYY, float sigmaXY)
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



        public void calculate_streamlines(int targetSeedCount)
        {
            // Create boundary edges from triangle data
            GenerateBoundaryEdges();
            ComputeBoundaryEdgeProperties();

            // Generate seed points for streamlines
            List<OpenTK.Vector2> seed_points = GenerateSeedPoints(targetSeedCount);
            // List<OpenTK.Vector2> seed_points = GenerateGridSeedPoints(targetSeedCount);

            this.streamlines.Clear();

            float geomsize = gvariables_static.geom_size;

            foreach (OpenTK.Vector2 seed_point in seed_points)
            {
                streamline_result streamline = TraceStressLine(seed_point, isTensionLine, stepSize: geomsize / 1000.0f, maxSteps: 1000);
                this.streamlines.Add(streamline);

            }


        }




        private streamline_result TraceStressLine(OpenTK.Vector2 seed, bool isTensionLine,
                                         float stepSize, int maxSteps)
        {
            var result = new streamline_result();

            // Use a stack for points (we'll reverse for backward)
            List<OpenTK.Vector2> forwardPoints = new List<OpenTK.Vector2>();
            List<OpenTK.Vector2> backwardPoints = new List<OpenTK.Vector2>();

            // Trace in both directions
            TraceDirection(seed, isTensionLine, stepSize, maxSteps / 2, forwardPoints, true);
            TraceDirection(seed, isTensionLine, stepSize, maxSteps / 2, backwardPoints, false);

            // Combine: backward (reversed) + forward
            if (backwardPoints.Count > 1)
            {
                backwardPoints.Reverse();
                // Remove duplicate seed
                backwardPoints.RemoveAt(backwardPoints.Count - 1);
            }

            result.streamline_points.AddRange(backwardPoints);
            result.streamline_points.AddRange(forwardPoints);

            // Remove duplicates and smooth
            result.streamline_points = RemoveConsecutiveDuplicates(result.streamline_points);

            // Store magnitudes
            foreach (var point in result.streamline_points)
            {
                var stress = GetStressAtPoint(point);
                if (stress != null)
                {
                    var (sigma1, sigma2, _) = stress.GetPrincipalStresses();
                    float magnitude = isTensionLine ? sigma1 : Math.Abs(sigma2);
                    result.streamline_magnitudes.Add(magnitude);
                }
                else
                {
                    result.streamline_magnitudes.Add(0);
                }
            }

            return result;
        }



        private void TraceDirection(OpenTK.Vector2 startPoint, bool isTensionLine,
                                   float stepSize, int maxSteps,
                                   List<OpenTK.Vector2> points, bool forward)
        {
            OpenTK.Vector2 currentPoint = startPoint;
            float currentStepSize = stepSize;
            int direction = forward ? 1 : -1;

            int i = 0;

            //for (int i = 0; i < maxSteps; i++)
            while (true)
            {
                points.Add(currentPoint);

                // Get direction based on stress state
                OpenTK.Vector2 directionVector = GetStressLineDirection(currentPoint, isTensionLine);

                //if (directionVector.LengthSquared < 0.0001f)
                //    break;

                directionVector.Normalize();
                if (!forward) directionVector = -directionVector;

                // Adaptive RK4 with error control
                OpenTK.Vector2 nextPoint = StepRK4(currentPoint, directionVector, direction, currentStepSize, isTensionLine);

                // Check if next point is inside mesh
                if (!IsPointInsideMesh(nextPoint))
                {
                    //// Try smaller step
                    //if (currentStepSize > 0.001f)
                    //{
                    //    currentStepSize *= 0.5f;
                    //    i--; // Retry this iteration
                    //    continue;
                    //}
                    break;
                }

                //// Check for convergence
                //if ((nextPoint - currentPoint).Length < 1e-7f)
                //    break;

                currentPoint = nextPoint;

                //// Adaptive step size based on curvature
                //if (i % 2 == 0 && i > 0)
                //{
                //    float curvature = EstimateCurvature(points);
                //    if (curvature > 0.1f && currentStepSize > 0.001f)
                //        currentStepSize *= 0.8f; // Smaller steps in high curvature
                //    else if (curvature < 0.01f && currentStepSize < stepSize)
                //        currentStepSize = Math.Min(stepSize, currentStepSize * 1.2f);
                //}

                if(i > 100000) break;

                i++;
            }
        }



        private OpenTK.Vector2 StepRK4(OpenTK.Vector2 point, OpenTK.Vector2 directionVector,
                                       int direction,
                                       float stepSize, bool isTensionLine)
        {
            // k1
            OpenTK.Vector2 k1 = directionVector;

            // k2
            OpenTK.Vector2 mid1 = point + 0.5f * stepSize * k1 * direction;
            OpenTK.Vector2 dir2 = GetStressLineDirection(mid1, isTensionLine);
            // if (dir2.LengthSquared < 0.0001f) return point;
            dir2.Normalize();
            OpenTK.Vector2 k2 = dir2;

            // k3
            OpenTK.Vector2 mid2 = point + 0.5f * stepSize * k2 * direction;
            OpenTK.Vector2 dir3 = GetStressLineDirection(mid2, isTensionLine);
            // if (dir3.LengthSquared < 0.0001f) return point;
            dir3.Normalize();
            OpenTK.Vector2 k3 = dir3;

            // k4
            OpenTK.Vector2 end = point + stepSize * k3 * direction;
            OpenTK.Vector2 dir4 = GetStressLineDirection(end, isTensionLine);
            // if (dir4.LengthSquared < 0.0001f) return point;
            dir4.Normalize();
            OpenTK.Vector2 k4 = dir4;

            // Combine
            OpenTK.Vector2 step = (stepSize / 6f) * (k1 + 2f * k2 + 2f * k3 + k4) * direction;
            return point + step;
        }

        // Helper method to check if a point is inside the mesh
        private bool IsPointInsideMesh(OpenTK.Vector2 point)
        {
            return FindContainingTriangle(point) != null;
        }


        // Helper method to remove consecutive duplicate points
        private List<OpenTK.Vector2> RemoveConsecutiveDuplicates(List<OpenTK.Vector2> points)
        {
            if (points.Count <= 1) return points;

            List<OpenTK.Vector2> cleaned = new List<OpenTK.Vector2>();
            cleaned.Add(points[0]);

            float tolerance = 1e-6f;
            for (int i = 1; i < points.Count; i++)
            {
                float dx = points[i].X - points[i - 1].X;
                float dy = points[i].Y - points[i - 1].Y;

                if (dx * dx + dy * dy > tolerance * tolerance)
                {
                    cleaned.Add(points[i]);
                }
            }

            return cleaned;
        }



        private float EstimateCurvature(List<OpenTK.Vector2> points)
        {
            if (points.Count < 3) return 0;

            int n = points.Count;
            OpenTK.Vector2 p1 = points[n - 3];
            OpenTK.Vector2 p2 = points[n - 2];
            OpenTK.Vector2 p3 = points[n - 1];

            // Simple curvature estimate using three points
            float dx1 = p2.X - p1.X;
            float dy1 = p2.Y - p1.Y;
            float dx2 = p3.X - p2.X;
            float dy2 = p3.Y - p2.Y;

            float len1 = (float)Math.Sqrt(dx1 * dx1 + dy1 * dy1);
            float len2 = (float)Math.Sqrt(dx2 * dx2 + dy2 * dy2);

            if (len1 < 1e-7f || len2 < 1e-7f) return 0;

            // Angle between segments
            float cosAngle = (dx1 * dx2 + dy1 * dy2) / (len1 * len2);
            float angle = (float)Math.Acos(gvariables_static.Clamp(cosAngle, -1f, 1f));

            return Math.Abs(angle) / Math.Max(len1, len2);
        }





        private void GenerateBoundaryEdges()
        {
            // Use long as key: (minId << 32) | maxId
            Dictionary<long, int> edge_count = new Dictionary<long, int>();

            foreach (var triangle in triangle_data.Values)
            {
                // Encode edges as long
                long edge1 = EncodeEdge(triangle.pt_id1, triangle.pt_id2);
                long edge2 = EncodeEdge(triangle.pt_id2, triangle.pt_id3);
                long edge3 = EncodeEdge(triangle.pt_id3, triangle.pt_id1);

                IncrementEdgeCount(edge_count, edge1);
                IncrementEdgeCount(edge_count, edge2);
                IncrementEdgeCount(edge_count, edge3);
            }

            // Build boundary edges
            boundary_edges.Clear();
            foreach (var kvp in edge_count)
            {
                if (kvp.Value == 1)
                {
                    int id1 = (int)(kvp.Key >> 32);
                    int id2 = (int)(kvp.Key & 0xFFFFFFFF);

                    boundary_edges.Add(new boundaryedges_store
                    {
                        pt_id1 = id1,
                        pt_id2 = id2
                    });
                }
            }
        }

        private long EncodeEdge(int id1, int id2)
        {
            // Always put smaller ID first for consistency
            if (id1 > id2)
            {
                (id1, id2) = (id2, id1);
            }
            return ((long)id1 << 32) | (uint)id2;
        }

        private void IncrementEdgeCount(Dictionary<long, int> edge_count, long edge)
        {
            if (edge_count.ContainsKey(edge))
                edge_count[edge]++;
            else
                edge_count[edge] = 1;
        }


        private void ComputeBoundaryEdgeProperties()
        {
            foreach (var edge in boundary_edges)
            {
                // Get the two vertices
                var p1 = point_data[edge.pt_id1].location;
                var p2 = point_data[edge.pt_id2].location;

                // Compute midpoint
                edge.midpoint = (p1 + p2) / 2f;

                // Compute the edge vector
                OpenTK.Vector2 edge_vector = p2 - p1;

                // Compute the normal (rotate 90 degrees)
                OpenTK.Vector2 normal = new OpenTK.Vector2(-edge_vector.Y, edge_vector.X);
                normal.Normalize();

                // Determine if normal points outward
                // Find the triangle that contains this edge and use its third vertex
                int third_vertex_id = FindThirdVertexForEdge(edge.pt_id1, edge.pt_id2);
                if (third_vertex_id != -1)
                {
                    var third_vertex = point_data[third_vertex_id].location;
                    OpenTK.Vector2 to_center = third_vertex - edge.midpoint;

                    // If normal points toward the triangle interior, flip it
                    if (OpenTK.Vector2.Dot(normal, to_center) > 0)
                    {
                        normal = -normal;
                    }
                }

                edge.outward_normal = normal;
            }
        }

        private int FindThirdVertexForEdge(int id1, int id2)
        {
            // Find the triangle that has both id1 and id2, and return the third vertex
            foreach (var triangle in triangle_data.Values)
            {
                bool has1 = triangle.pt_id1 == id1 || triangle.pt_id2 == id1 || triangle.pt_id3 == id1;
                bool has2 = triangle.pt_id1 == id2 || triangle.pt_id2 == id2 || triangle.pt_id3 == id2;

                if (has1 && has2)
                {
                    // Return the vertex that isn't id1 or id2
                    if (triangle.pt_id1 != id1 && triangle.pt_id1 != id2) return triangle.pt_id1;
                    if (triangle.pt_id2 != id1 && triangle.pt_id2 != id2) return triangle.pt_id2;
                    if (triangle.pt_id3 != id1 && triangle.pt_id3 != id2) return triangle.pt_id3;
                }
            }
            return -1; // Should never happen for boundary edges
        }


        private List<OpenTK.Vector2> GenerateGridSeedPoints(int targetSeedCount)
        {
            List<OpenTK.Vector2> seed_points = new List<OpenTK.Vector2>();
         
            // Get mesh bounds
            var bounds = GetMeshBounds();
            float width = bounds.Width;
            float height = bounds.Height;
            
            // Determine grid size based on target seed count
            int gridCols = (int)Math.Sqrt(targetSeedCount * (width / height));
            int gridRows = (int)Math.Sqrt(targetSeedCount * (height / width));
            float cellWidth = width / gridCols;
            float cellHeight = height / gridRows;
            for (int i = 0; i < gridCols; i++)
            {
                for (int j = 0; j < gridRows; j++)
                {
                    // Calculate the center of the cell
                    float x = bounds.Left + (i + 0.5f) * cellWidth;
                    float y = bounds.Top + (j + 0.5f) * cellHeight;
                    OpenTK.Vector2 seedPoint = new OpenTK.Vector2(x, y);
                    // Check if the point is inside the mesh
                    if (IsPointInsideMesh(seedPoint))
                    {
                        seed_points.Add(seedPoint);
                    }
                }
            }
            return seed_points;
        }


        private List<OpenTK.Vector2> GenerateSeedPoints(int targetSeedCount)
        {
            List<OpenTK.Vector2> seed_points = new List<OpenTK.Vector2>();

            // Step 1: Calculate the total length of all boundary edges
            float totalEdgeLength = 0.0f;
            List<float> edgeLengths = new List<float>();

            foreach (boundaryedges_store edge in boundary_edges)
            {
                OpenTK.Vector2 p1 = point_data[edge.pt_id1].location;
                OpenTK.Vector2 p2 = point_data[edge.pt_id2].location;

                // Calculate the length of the edge
                float length = (p2 - p1).Length;
                edgeLengths.Add(length);
                totalEdgeLength += length;
            }


            if (targetSeedCount > boundary_edges.Count)
            {

                // Step 2A: Calculate seeds per edge based on edge length (longer edges get more seeds)
                int seedsPlaced = 0;
                for (int i = 0; i < boundary_edges.Count; i++)
                {
                    boundaryedges_store edge = boundary_edges[i];
                    float edgeRatio = edgeLengths[i] / totalEdgeLength;

                    int seedsForThisEdge = Math.Max(1, (int)(targetSeedCount * edgeRatio));

                    var p1 = point_data[edge.pt_id1].location;
                    var p2 = point_data[edge.pt_id2].location;

                    // Distribute seeds along this edge
                    for (int j = 0; j < seedsForThisEdge; j++)
                    {
                        float t = (j + 0.5f) / seedsForThisEdge;
                        OpenTK.Vector2 seed = OpenTK.Vector2.Lerp(p1, p2, t);

                        // Offset slightly inside the domain (opposite to outward normal)
                        seed -= edge.outward_normal * 0.01f; // Move into the domain

                        seed_points.Add(seed);
                        seedsPlaced++;
                    }
                }
            }
            else
            {
                // Step 2B: Uniform sampling across all edges, ensuring no duplicates
                float step = (float)boundary_edges.Count / (float)targetSeedCount;

                for (int i = 0; i < targetSeedCount; i++)
                {
                    int edgeIndex = (int)(i * step);

                    // Ensure edgeIndex is within bounds
                    edgeIndex = Math.Min(edgeIndex, boundary_edges.Count - 1);

                    boundaryedges_store edge = boundary_edges[edgeIndex];

                    var p1 = point_data[edge.pt_id1].location;
                    var p2 = point_data[edge.pt_id2].location;
                    // Place seed at the midpoint of the edge
                    OpenTK.Vector2 seed = (p1 + p2) / 2f;
                    // // Offset slightly inside the domain (opposite to outward normal)
                    // seed -= edge.outward_normal * 0.01f; // Move into the domain
                    seed_points.Add(seed);
                }

                //for (int i = 0; i < boundary_edges.Count; i++)
                //{
                //    boundaryedges_store edge = boundary_edges[i];

                //    var p1 = point_data[edge.pt_id1].location;
                //    var p2 = point_data[edge.pt_id2].location;


                //    OpenTK.Vector2 seed = OpenTK.Vector2.Lerp(p1, p2, 0.5f);

                //    // Offset slightly inside the domain (opposite to outward normal)
                //    seed -= edge.outward_normal * 0.01f; // Move into the domain

                //    seed_points.Add(seed);

                //}



            }

            return seed_points;
        }




    }
}
