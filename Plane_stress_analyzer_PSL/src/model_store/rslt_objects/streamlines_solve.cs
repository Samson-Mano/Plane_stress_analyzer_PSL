using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace src.model_store.rslt_objects
{


    public class streamlines_solve
    {
        private class vectordata_store
        {
            public int point_id { get; set; }
            public OpenTK.Vector2 location { get; set; }
            public OpenTK.Vector2 vector { get; set; } // Normalized vector
            public double magnitude { get; set; } // Magnitude of the vector

        }


        private class triangledata_store
        {
            public int tri_id { get; set; }
            public int pt_id1 { get; set; }
            public int pt_id2 { get; set; }
            public int pt_id3 { get; set; }

        }

        private class boundaryedges_store
        {
            public int edge_id { get; set; }
            public int pt_id1 { get; set; }
            public int pt_id2 { get; set; }

            public OpenTK.Vector2 midpoint { get; set; }
            public OpenTK.Vector2 outward_normal { get; set; }
            public bool is_inlet { get; set; } // Computed later

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
        Dictionary<int, vectordata_store> point_data = new Dictionary<int, vectordata_store>();
        List<boundaryedges_store> boundary_edges = new List<boundaryedges_store>();

        public List<streamline_result> streamlines { get; set; } = new List<streamline_result>();


        public streamlines_solve()
        {
            // Initialize the dictionaries
            point_data = new Dictionary<int, vectordata_store>();
            triangle_data = new Dictionary<int, triangledata_store>();
            boundary_edges = new List<boundaryedges_store>();

        }

        public void add_point_data(int point_id, OpenTK.Vector2 location, OpenTK.Vector2 vector, double magnitude)
        {
            vectordata_store data = new vectordata_store
            {
                point_id = point_id,
                location = location,
                vector = vector,
                magnitude = magnitude
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



        public void calculate_streamlines()
        {
            // Create boundary edges from triangle data
            GenerateBoundaryEdges();
            ComputeBoundaryEdgeProperties();

            // Generate seed points for streamlines
            List<OpenTK.Vector2> seed_points = GenerateSeedPoints(100);



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

                // Step 2: Calculate seeds per edge based on edge length (longer edges get more seeds)
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

                        // // Offset slightly inside the domain (opposite to outward normal)
                        // seed -= edge.outward_normal * 0.01f; // Move into the domain

                        seed_points.Add(seed);
                        seedsPlaced++;
                    }
                }
            }




            return seed_points;
        }




    }
}
