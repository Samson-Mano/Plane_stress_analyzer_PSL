using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using Plane_stress_analyzer_PSL.src.events_handler;
using Plane_stress_analyzer_PSL.src.global_variables;
using Plane_stress_analyzer_PSL.src.model_store.geom_objects;
using Plane_stress_analyzer_PSL.src.opentk_control.opentk_buffer;
using Plane_stress_analyzer_PSL.src.opentk_control.shader_compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plane_stress_analyzer_PSL.src.model_store.rslt_objects
{
    public class rsltdata_store : IDisposable
    {
        private struct point_store
        {
            public int point_id;
            public float x_coord;
            public float y_coord;

            public float displ_x;
            public float displ_y;
            public float displ_magnitude;

        }

        private struct line_store
        {
            public int line_id;
            public int line_start_id;
            public int line_end_id;
        }

        private struct tri_store
        {
            public int tri_id;
            public int pt_id1;
            public int pt_id2;
            public int pt_id3;

        }

        private struct result_extremes
        {
            public float max_displacement;

            // Stress extremes in X direction
            public float max_stressX;
            public float min_stressX;

            // Stress extremes in Y direction
            public float max_stressY; 
            public float min_stressY;

            // Shear stress extremes
            public float max_tauXY; 
            public float min_tauXY;



        }

        private Dictionary<int, point_store> points = new Dictionary<int, point_store>();
        private List<line_store> wireframe_lines = new List<line_store>();
        private List<tri_store> tris = new List<tri_store>();

        private result_extremes rslt_extremes;

        // public bool isResultSet = false;


        private Shader rsltmeshShader;
        private Shader rsltmeshwireframeShader;

        // Vertex Buffer object and Vertex Array object 
        private VertexBuffer point_vbo;
        private VertexArray point_vao;

        // Index buffer for the points, wireframe lines, and triangles (EBO)
        private IndexBuffer point_ibo;
        private IndexBuffer wireframe_ibo;
        private IndexBuffer triangle_ibo;

        // Shrunk mesh data
        private shrunkrsltdata_store shrunk_rsltmesh_data = new shrunkrsltdata_store();


        private bool buffersInitialized = false;

        public rsltdata_store()
        {
            InitializeShader();
        }


        private void InitializeShader()
        {
            // Create Shader
            rsltmeshShader = new Shader(
                ShaderLibrary.get_vertex_shader(ShaderLibrary.ShaderType.RsltMeshShader),
                ShaderLibrary.get_fragment_shader(ShaderLibrary.ShaderType.RsltMeshShader)
                );

            rsltmeshwireframeShader = new Shader(
                ShaderLibrary.get_vertex_shader(ShaderLibrary.ShaderType.RsltWireframeShader),
                ShaderLibrary.get_fragment_shader(ShaderLibrary.ShaderType.RsltWireframeShader)
                );
        }



        public void add_point(int point_id, float x, float y, float displ_x, float displ_y)
        {
            float displ_magnitude = (float)Math.Sqrt(displ_x * displ_x + displ_y * displ_y);    

            points.Add(point_id, new point_store()
            {
                point_id = point_id,
                x_coord = x,
                y_coord = y,
                displ_x = displ_x,
                displ_y = displ_y,
                displ_magnitude = displ_magnitude
            });

        }

        public void add_wireframe_line(int line_id, int line_start_id, int line_end_id)
        {
            wireframe_lines.Add(new line_store()
            {
                line_id = line_id,
                line_start_id = line_start_id,
                line_end_id = line_end_id
            });
        }


        public void add_tri(int tri_id, int pt_id1, int pt_id2, int pt_id3)
        {
            tris.Add(new tri_store()
            {
                tri_id = tri_id,
                pt_id1 = pt_id1,
                pt_id2 = pt_id2,
                pt_id3 = pt_id3,
            });

        }


        public void set_result_extremes()
        {
            // Result extremes are calculated based on the points data
            rslt_extremes = new result_extremes();
            rslt_extremes.max_displacement = 0.0f;

            foreach (var pt in points.Values)
            {
                if (pt.displ_magnitude > rslt_extremes.max_displacement)
                {
                    rslt_extremes.max_displacement = pt.displ_magnitude;
                }
            }

        }


        public void paint_results()
        {
            paint_result_mesh();

            paint_result_mesh_wireframe();

            paint_result_mesh_points();
        }


        private void paint_result_mesh()
        {
            if (!gvariables_static.is_paint_resultmesh || !buffersInitialized)
                return;

            rsltmeshShader.Bind();

            if (gvariables_static.is_paint_shrunk_triangle)
            {
                // Paint the shrunk mesh 
                shrunk_rsltmesh_data.paint_shrunk_rsltmesh();
                rsltmeshShader.UnBind();
                return;
            }


            point_vao.Bind();

            if (triangle_ibo.BufferCount > 0)
            {
                // Paint the Result triangle mesh
                triangle_ibo.Bind();
                GL.DrawElements(PrimitiveType.Triangles, triangle_ibo.BufferCount,
                    DrawElementsType.UnsignedInt, 0);
                triangle_ibo.UnBind();
            }

            point_vao.UnBind();
            rsltmeshShader.UnBind();


        }


        private void paint_result_mesh_wireframe()
        {
            if (!gvariables_static.is_paint_resultmesh_boundaries || !buffersInitialized)
                return;


            if (wireframe_ibo.BufferCount > 0)
            {
                rsltmeshwireframeShader.Bind();

                point_vao.Bind();
                wireframe_ibo.Bind();
                
                GL.DrawElements(PrimitiveType.Lines, wireframe_ibo.BufferCount, DrawElementsType.UnsignedInt, 0);

                rsltmeshwireframeShader.UnBind();
                point_vao.UnBind();
                wireframe_ibo.UnBind();
            }

        }


        private void paint_result_mesh_points()
        {
            if (!gvariables_static.is_paint_resultmeshpoints || !buffersInitialized)
                return;


            if (point_ibo.BufferCount > 0)
            {
                // Paint the result mesh points
                rsltmeshShader.Bind();

                point_vao.Bind();
                point_ibo.Bind();

                GL.PointSize(2.0f);
                GL.DrawElements(PrimitiveType.Points, point_ibo.BufferCount, DrawElementsType.UnsignedInt, 0);
                GL.PointSize(1.0f);

                rsltmeshShader.UnBind();
                point_vao.UnBind();
                point_ibo.UnBind();

            }

        }


        public void create_buffer_data()
        {

            //_______________________________________________________________
            // prepare the Vertex data for openGL
            List<float> vertexData = new List<float>();
            List<int> pointIndexData = new List<int>();

            for (int i = 0; i < points.Count; i++)
            {
                point_store pt = points[i];
                vertexData.Add(pt.x_coord);
                vertexData.Add(pt.y_coord);

                // Normalized displacement values for plotting
                if(pt.displ_magnitude > 0)
                {
                    vertexData.Add((float)(pt.displ_x / pt.displ_magnitude));
                    vertexData.Add((float)(pt.displ_y / pt.displ_magnitude));
                }
                else
                {
                    vertexData.Add(0);
                    vertexData.Add(0);
                }

                // Calculate the magnitude of the displacement vector for color mapping
                vertexData.Add((float)(pt.displ_magnitude / rslt_extremes.max_displacement)); // normalized scalar value

                pointIndexData.Add(i);
            }


            // Create VAO and VBO for points
            point_vao = new VertexArray();
            point_vbo = new VertexBuffer(Math.Max(10, vertexData.Count));
            point_ibo = new IndexBuffer(Math.Max(10, pointIndexData.Count));


            VertexBufferLayout pointLayout = new VertexBufferLayout();
            pointLayout.AddFloat(2);  // x and y coordinates
            pointLayout.AddFloat(2); // displ_x and displ_y
            pointLayout.AddFloat(1); // scalar value

            point_vao.Add_vertexBuffer(point_vbo, pointLayout);


            point_vbo.AppendVertexBuffer(vertexData.ToArray());
            point_ibo.AppendIndexBuffer(pointIndexData.ToArray());

            //_______________________________________________________________
            // prepare wireframe index data for openGL
            List<int> wireframeIndexData = new List<int>();

            foreach (line_store ln in wireframe_lines)
            {

                wireframeIndexData.Add(ln.line_start_id);
                wireframeIndexData.Add(ln.line_end_id);
            }


            wireframe_ibo = new IndexBuffer(Math.Max(10, wireframeIndexData.Count));
            if (wireframeIndexData.Count > 0)
            {
                wireframe_ibo.AppendIndexBuffer(wireframeIndexData.ToArray());
            }

            //_______________________________________________________________
            // prepare triangle index data for openGL
            List<int> triangleIndexData = new List<int>();

            foreach (tri_store tri in tris)
            {

                triangleIndexData.Add(tri.pt_id1);
                triangleIndexData.Add(tri.pt_id2);
                triangleIndexData.Add(tri.pt_id3);

            }

            triangle_ibo = new IndexBuffer(Math.Max(10, triangleIndexData.Count));
            if (triangleIndexData.Count > 0)
            {
                triangle_ibo.AppendIndexBuffer(triangleIndexData.ToArray());
            }

            // Shrunk Mesh buffers
            generate_shrunk_mesh();

            buffersInitialized = true;
        }



        private void generate_shrunk_mesh()
        {

            // Generate shrunk vertices for triangles
            foreach (tri_store tri in tris)
            {
                var p1 = points[tri.pt_id1];
                var p2 = points[tri.pt_id2];
                var p3 = points[tri.pt_id3];

                float[] pt1_values = new float[5];
                pt1_values[0] = p1.x_coord;
                pt1_values[1] = p1.y_coord;
                if (p1.displ_magnitude > 0)
                {
                    pt1_values[2] = (float)(p1.displ_x / p1.displ_magnitude);
                    pt1_values[3] = (float)(p1.displ_y / p1.displ_magnitude);
                }
                else
                {
                    pt1_values[2] = 0;
                    pt1_values[3] = 0;
                }
                pt1_values[4] = (float)(p1.displ_magnitude / rslt_extremes.max_displacement);


                float[] pt2_values = new float[5];
                pt2_values[0] = p2.x_coord;
                pt2_values[1] = p2.y_coord;
                if (p2.displ_magnitude > 0)
                {
                    pt2_values[2] = (float)(p2.displ_x / p2.displ_magnitude);
                    pt2_values[3] = (float)(p2.displ_y / p2.displ_magnitude);
                }
                else
                {
                    pt2_values[2] = 0;
                    pt2_values[3] = 0;
                }
                pt2_values[4] = (float)(p2.displ_magnitude / rslt_extremes.max_displacement);


                float[] pt3_values = new float[5];
                pt3_values[0] = p3.x_coord;
                pt3_values[1] = p3.y_coord;
                if (p3.displ_magnitude > 0)
                {
                    pt3_values[2] = (float)(p3.displ_x / p3.displ_magnitude);
                    pt3_values[3] = (float)(p3.displ_y / p3.displ_magnitude);
                }
                else
                {
                    pt3_values[2] = 0;
                    pt3_values[3] = 0;
                }
                pt3_values[4] = (float)(p3.displ_magnitude / rslt_extremes.max_displacement);

                shrunk_rsltmesh_data.add_shrunk_triangle(tri.tri_id, pt1_values, pt2_values, pt3_values);
            }


            // Initialize the buffer
            shrunk_rsltmesh_data.create_shrunkrsltmesh_buffer_data();

        }


        public void update_openTK_uniforms(drawing_events graphic_events_control)
        {
            Matrix4 uMVP = graphic_events_control.projectionMatrix *
                graphic_events_control.viewMatrix * graphic_events_control.modelMatrix;

            rsltmeshShader.SetMatrix4("uMVP", uMVP);
            rsltmeshShader.SetFloat("geomscale", gvariables_static.geom_size);

            float model_percent = (float)(gvariables_static.displacement_scale / 1000.0);

            rsltmeshShader.SetFloat("modelpercent", model_percent);

            rsltmeshShader.SetFloat("vertexTransparency", gvariables_static.rslt_transparency);


            rsltmeshwireframeShader.SetMatrix4("uMVP", uMVP);
            rsltmeshwireframeShader.SetFloat("geomscale", gvariables_static.geom_size);
            rsltmeshwireframeShader.SetFloat("modelpercent", model_percent);

            // rsltmeshwireframeShader.SetFloat("vertexTransparency", gvariables_static.rslt_transparency);

            Vector3 customColor = new Vector3(1.0f, 1.0f, 1.0f); // Fallback color

            // rsltmeshwireframeShader.SetVector3("wireframeColor", customColor);
            rsltmeshwireframeShader.SetFloat("wireframeAlpha", 0.5f);

        }


        public void Dispose()
        {
            point_vbo?.Dispose();
            point_vao?.Dispose();
            point_ibo?.Dispose();
            wireframe_ibo?.Dispose();
            triangle_ibo?.Dispose();
            // meshShader?.Dispose();

        }



    }
}
