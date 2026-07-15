using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using Plane_stress_analyzer_PSL.src.events_handler;
using Plane_stress_analyzer_PSL.src.global_variables;
using Plane_stress_analyzer_PSL.src.model_store.geom_objects;
using Plane_stress_analyzer_PSL.src.opentk_control.opentk_buffer;
using Plane_stress_analyzer_PSL.src.opentk_control.shader_compiler;
using src.model_store.geom_objects;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
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
            public double x_coord;
            public double y_coord;

            public double displ_x;
            public double displ_y;
            public double displ_magnitude;

            public double sigma_x;
            public double sigma_y;
            public double tau_xy;

            public double sigma_1;
            public double sigma_2;

            public double von_mises;
            public double max_shear;
            public double theta_p; // principal stress angle

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


        private struct reaction_store
        {
            public int point_id;
            public double x_coord;
            public double y_coord;

            public int constraint_type; // 0 = free, 1 = pinned, 2 = roller
            public double constraint_angle;

            public double reaction_x;
            public double reaction_y;
        }


        public struct result_extremes
        {
            // Displacement (option = 1)
            public double max_displacement;

            // Stress extremes in X direction (option = 2)
            public double max_stressX;
            public double min_stressX;

            // Stress extremes in Y direction (option = 3)
            public double max_stressY; 
            public double min_stressY;

            // Shear stress extremes (option = 4)
            public double max_tauXY; 
            public double min_tauXY;

            // Von Mises stress extremes (option = 5)
            public double max_vonMises;
            public double min_vonMises;

            // Principal stress 1 extremes (option = 6)
            public double max_principalStress1;
            public double min_principalStress1;

            // Principal stress 2 extremes (option = 7)
            public double max_principalStress2;
            public double min_principalStress2;

            // Max shear stress extremes (option = 8)
            public double max_shearStress;
            public double min_shearStress;

            // PSL Lines (option = 9)

        }



        private Dictionary<int, point_store> points = new Dictionary<int, point_store>();
        private List<line_store> wireframe_lines = new List<line_store>();
        private List<tri_store> tris = new List<tri_store>();
        private List<reaction_store> reactions = new List<reaction_store>();


        public result_extremes rslt_extremes {  get { return _rslt_extremes; } }
        private result_extremes _rslt_extremes;

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



        public void add_point(int point_id, double x_coord, double y_coord, 
            double displ_x, double displ_y,
            int constraint_type,
            double constraint_angle,
            double reaction_x, double reaction_y,
            double sigma_x, double sigma_y, 
            double tau_xy, 
            double sigma_1, double sigma_2, 
            double von_mises, 
            double max_shear, 
            double theta_p)
        {

            if(constraint_type != 0)
            {
                reactions.Add(new reaction_store()
                {
                    point_id = point_id,
                    x_coord = x_coord,
                    y_coord = y_coord,
                    constraint_type = constraint_type,
                    constraint_angle = constraint_angle,
                    reaction_x = reaction_x,
                    reaction_y = reaction_y
                });
            }

            double displ_magnitude = Math.Sqrt(displ_x * displ_x + displ_y * displ_y);

            points.Add(point_id, new point_store()
            {
                point_id = point_id,
                x_coord = x_coord,
                y_coord = y_coord,
                displ_x = displ_x,
                displ_y = displ_y,
                displ_magnitude = displ_magnitude,
                sigma_x = sigma_x,
                sigma_y = sigma_y,
                tau_xy = tau_xy,
                sigma_1 = sigma_1,
                sigma_2 = sigma_2,
                von_mises = von_mises,
                max_shear = max_shear,
                theta_p = theta_p
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


        public bool set_result_extremes()
        {
            // Result extremes are calculated based on the points data
            _rslt_extremes = new result_extremes();
            _rslt_extremes.max_displacement = 0.0;
            _rslt_extremes.max_stressX = double.MinValue;
            _rslt_extremes.min_stressX = double.MaxValue;
            _rslt_extremes.max_stressY = double.MinValue;
            _rslt_extremes.min_stressY = double.MaxValue;
            _rslt_extremes.max_tauXY = double.MinValue;
            _rslt_extremes.min_tauXY = double.MaxValue;
            _rslt_extremes.max_principalStress1 = double.MinValue;
            _rslt_extremes.min_principalStress1 = double.MaxValue;
            _rslt_extremes.max_principalStress2 = double.MinValue;
            _rslt_extremes.min_principalStress2 = double.MaxValue;
            _rslt_extremes.max_vonMises = double.MinValue;
            _rslt_extremes.min_vonMises = double.MaxValue;
            _rslt_extremes.max_shearStress = double.MinValue;
            _rslt_extremes.min_shearStress = double.MaxValue;
            

            foreach (var pt in points.Values)
            {
                // Maximum displacement magnitude
                _rslt_extremes.max_displacement = Math.Max(_rslt_extremes.max_displacement, pt.displ_magnitude);

                // Maximum and minimum stress in X and Y directions
                _rslt_extremes.max_stressX = Math.Max(_rslt_extremes.max_stressX, pt.sigma_x);
                _rslt_extremes.min_stressX = Math.Min(_rslt_extremes.min_stressX, pt.sigma_x);
                _rslt_extremes.max_stressY = Math.Max(_rslt_extremes.max_stressY, pt.sigma_y);
                _rslt_extremes.min_stressY = Math.Min(_rslt_extremes.min_stressY, pt.sigma_y);

                // Maximum and minimum shear stress
                _rslt_extremes.max_tauXY = Math.Max(_rslt_extremes.max_tauXY, pt.tau_xy);
                _rslt_extremes.min_tauXY = Math.Min(_rslt_extremes.min_tauXY, pt.tau_xy);

                // Maximum and minimum principal stresses
                _rslt_extremes.max_principalStress1 = Math.Max(_rslt_extremes.max_principalStress1, pt.sigma_1);
                _rslt_extremes.min_principalStress1 = Math.Min(_rslt_extremes.min_principalStress1, pt.sigma_1);
                _rslt_extremes.max_principalStress2 = Math.Max(_rslt_extremes.max_principalStress2, pt.sigma_2);
                _rslt_extremes.min_principalStress2 = Math.Min(_rslt_extremes.min_principalStress2, pt.sigma_2);

                // Maximum and minimum von Mises stress
                _rslt_extremes.max_vonMises = Math.Max(_rslt_extremes.max_vonMises, pt.von_mises);
                _rslt_extremes.min_vonMises = Math.Min(_rslt_extremes.min_vonMises, pt.von_mises);

                // Maximum and minimum shear stress
                _rslt_extremes.max_shearStress = Math.Max(_rslt_extremes.max_shearStress, pt.max_shear);
                _rslt_extremes.min_shearStress = Math.Min(_rslt_extremes.min_shearStress, pt.max_shear);

            }


            // Validate the result extremes to ensure they are meaningful
            if (_rslt_extremes.max_displacement <= 0 || !check_double(_rslt_extremes.max_displacement))
            {
                return false;
            }
            if (!check_double(_rslt_extremes.max_stressX) || !check_double(_rslt_extremes.min_stressX))
            {
                return false;
            }
            if (!check_double(_rslt_extremes.max_stressY) || !check_double(_rslt_extremes.min_stressY))
            {
                return false;
            }
            if (!check_double(_rslt_extremes.max_tauXY) || !check_double(_rslt_extremes.min_tauXY))
            {
                return false;
            }
            if (!check_double(_rslt_extremes.max_vonMises) || !check_double(_rslt_extremes.min_vonMises))
            {
                return false;
            }
            if (!check_double(_rslt_extremes.max_principalStress1) || !check_double(_rslt_extremes.min_principalStress1))
            {
                return false;
            }
            if (!check_double(_rslt_extremes.max_principalStress2) || !check_double(_rslt_extremes.min_principalStress2))
            {
                return false;
            }
            if (!check_double(_rslt_extremes.max_shearStress) || !check_double(_rslt_extremes.min_shearStress))
            {
                return false;
            }

            return true;
        }


        private bool check_double(double value)
        {
            // Check if the double value is valid (not NaN or Infinity)
            return !double.IsNaN(value) && !double.IsInfinity(value);
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


        public void switch_result_option()
        {
            // Switch the result option for visualization
            // 1 = Displacement, 2 = StressX, 3 = StressY, 4 = Shear stress, 
            // 5 = Von Mises stress, 6 = Principal stress 1, 7 = Principal stress 2, 
            // 8 = Max shear stress

            int option =  gvariables_static.result_option;

            List<float> vertexData = new List<float>();

            for (int i = 0; i < points.Count; i++)
            {
                point_store pt = points[i];
                vertexData.Add((float)pt.x_coord);
                vertexData.Add((float)pt.y_coord);

                // Normalized displacement values for plotting
                if (pt.displ_magnitude > 0)
                {
                    vertexData.Add((float)(pt.displ_x / pt.displ_magnitude));
                    vertexData.Add((float)(pt.displ_y / pt.displ_magnitude));
                }
                else
                {
                    vertexData.Add(0);
                    vertexData.Add(0);
                }

                vertexData.Add((float)(pt.displ_magnitude / _rslt_extremes.max_displacement));
                float normalized_contourValue = 0.0f;
                float scaled_contourValue = 0.0f;

                switch (option)
                {
                    case 1: // Displacement
                        vertexData.Add((float)(pt.displ_magnitude / _rslt_extremes.max_displacement));
                        break;
                    case 2: // StressX
                            normalized_contourValue = (float)((pt.sigma_x - _rslt_extremes.min_stressX) / (_rslt_extremes.max_stressX - _rslt_extremes.min_stressX));
                        scaled_contourValue = (normalized_contourValue * 2.0f) - 1.0f; // Scale to [-1, 1]
                        vertexData.Add(scaled_contourValue);
                        break;
                    case 3: // StressY
                        normalized_contourValue = (float)((pt.sigma_y - _rslt_extremes.min_stressY) / (_rslt_extremes.max_stressY - _rslt_extremes.min_stressY));
                        scaled_contourValue = (normalized_contourValue * 2.0f) - 1.0f; // Scale to [-1, 1]
                        vertexData.Add(scaled_contourValue);
                        break; 
                    case 4: // Shear stress
                        normalized_contourValue = (float)((pt.tau_xy - _rslt_extremes.min_tauXY) / (_rslt_extremes.max_tauXY - _rslt_extremes.min_tauXY));
                        scaled_contourValue = (normalized_contourValue * 2.0f) - 1.0f; // Scale to [-1, 1]
                        vertexData.Add(scaled_contourValue);
                        break;
                    case 5: // Von Mises stress
                        normalized_contourValue = (float)((pt.von_mises - _rslt_extremes.min_vonMises) / (_rslt_extremes.max_vonMises - _rslt_extremes.min_vonMises));
                        scaled_contourValue = (normalized_contourValue * 2.0f) - 1.0f; // Scale to [-1, 1]
                        vertexData.Add(scaled_contourValue);
                        break; 
                    case 6: // Principal stress 1
                        normalized_contourValue = (float)((pt.sigma_1 - _rslt_extremes.min_principalStress1) / (_rslt_extremes.max_principalStress1 - _rslt_extremes.min_principalStress1));
                        scaled_contourValue = (normalized_contourValue * 2.0f) - 1.0f; // Scale to [-1, 1]
                        vertexData.Add(scaled_contourValue);
                        break;
                    case 7: // Principal stress 2
                        normalized_contourValue = (float)((pt.sigma_2 - _rslt_extremes.min_principalStress2) / (_rslt_extremes.max_principalStress2 - _rslt_extremes.min_principalStress2));
                        scaled_contourValue = (normalized_contourValue * 2.0f) - 1.0f; // Scale to [-1, 1]
                        vertexData.Add(scaled_contourValue);
                        break;
                    case 8: // Max shear stress
                        normalized_contourValue = (float)((pt.max_shear - _rslt_extremes.min_shearStress) / (_rslt_extremes.max_shearStress - _rslt_extremes.min_shearStress));
                        scaled_contourValue = (normalized_contourValue * 2.0f) - 1.0f; // Scale to [-1, 1]
                        vertexData.Add(scaled_contourValue);
                        break;

                }

            }


            point_vbo.updateVertexBuffer(vertexData.ToArray());

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
                vertexData.Add((float)pt.x_coord);
                vertexData.Add((float)pt.y_coord);

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
                vertexData.Add((float)(pt.displ_magnitude / _rslt_extremes.max_displacement)); // normalized scalar value
                vertexData.Add((float)(pt.displ_magnitude / _rslt_extremes.max_displacement)); // Contour value for color mapping

                pointIndexData.Add(i);
            }


            // Create VAO and VBO for points
            point_vao = new VertexArray();
            point_vbo = new VertexBuffer(Math.Max(10, vertexData.Count));
            point_ibo = new IndexBuffer(Math.Max(10, pointIndexData.Count));


            VertexBufferLayout pointLayout = new VertexBufferLayout();
            pointLayout.AddFloat(2);  // x and y coordinates
            pointLayout.AddFloat(2); // displ_x and displ_y
            pointLayout.AddFloat(1); // displacement magnitude
            pointLayout.AddFloat(1); // Contour value

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
                pt1_values[0] = (float)p1.x_coord;
                pt1_values[1] = (float)p1.y_coord;
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
                pt1_values[4] = (float)(p1.displ_magnitude / _rslt_extremes.max_displacement);


                float[] pt2_values = new float[5];
                pt2_values[0] = (float)p2.x_coord;
                pt2_values[1] = (float)p2.y_coord;
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
                pt2_values[4] = (float)(p2.displ_magnitude / _rslt_extremes.max_displacement);


                float[] pt3_values = new float[5];
                pt3_values[0] = (float)p3.x_coord;
                pt3_values[1] = (float)p3.y_coord;
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
                pt3_values[4] = (float)(p3.displ_magnitude / _rslt_extremes.max_displacement);

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

            rsltmeshShader.SetFloat("rsltoption", 0);

            if(gvariables_static.result_option != 1)
            {
                rsltmeshShader.SetFloat("rsltoption", 1);
            }

            //____________________________________________________________________________________________

            rsltmeshwireframeShader.SetMatrix4("uMVP", uMVP);
            rsltmeshwireframeShader.SetFloat("geomscale", gvariables_static.geom_size);
            rsltmeshwireframeShader.SetFloat("modelpercent", model_percent);

            // rsltmeshwireframeShader.SetFloat("vertexTransparency", gvariables_static.rslt_transparency);

            Vector3 customColor = new Vector3(1.0f, 1.0f, 1.0f); // Fallback color

            // rsltmeshwireframeShader.SetVector3("wireframeColor", customColor);
            rsltmeshwireframeShader.SetFloat("wireframeAlpha", 0.5f);

            //____________________________________________________________________________________________
            // Contour data update
            rsltmeshShader.SetFloat("uNumContours", gvariables_static.contourline_level);
            rsltmeshShader.SetFloat("uLineOpacity", 0.0f);

            if (gvariables_static.is_paint_result_contourlines)
            {
                rsltmeshShader.SetFloat("uLineOpacity", 1.0f);
            }

        }

        public void update_animation(float sine_oscillation)
        {
            // Update the sine oscillation value in the shader for animation
            rsltmeshShader.SetFloat("sinevalue", sine_oscillation);

            rsltmeshwireframeShader.SetFloat("sinevalue", sine_oscillation);
            rsltmeshShader.SetFloat("uLineOpacity", 1.0f);

            if (sine_oscillation < 0.1)
            {
                rsltmeshShader.SetFloat("uLineOpacity", 0.0f);
            }

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
