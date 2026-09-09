// OpenTK library
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using Plane_stress_analyzer_PSL.src.events_handler;
using Plane_stress_analyzer_PSL.src.global_variables;
using Plane_stress_analyzer_PSL.src.model_store.fe_objects;
using Plane_stress_analyzer_PSL.src.model_store.geom_objects;
using Plane_stress_analyzer_PSL.src.opentk_control.opentk_buffer;
using Plane_stress_analyzer_PSL.src.opentk_control.shader_compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.model_store.rslt_objects
{

    public class reactionforce_store
    {
        public int point_id;
        public double x_coord;
        public double y_coord;

        public int constraint_type; // 0 = free, 1 = pinned, 2 = roller
        public double constraint_angle;

        public double global_reaction_x;
        public double global_reaction_y;

        public double transformed_reaction_x;

        public double transformed_reaction_y;
    }



    public class reactionforce_list_store
    {

        public Dictionary<int, reactionforce_store> reactionForceMap = new Dictionary<int, reactionforce_store>();
        // public int load_set_count = 0;

        // private List<int> all_loadset_ids = new List<int>();

        // Reaction force labels
        private label_list_store reaction_force_label;

        // Reaction force visualization
        private Shader reactionForceShader;

        // Vertex Buffer object and Vertex Array object 
        private VertexBuffer reactionForce_vbo;
        private VertexArray reactionForce_vao;
        private IndexBuffer reactionForce_ibo;


        public reactionforce_list_store()
        {
            // (Re)Initialize the data
            reactionForceMap = new Dictionary<int, reactionforce_store>();
            // load_set_count = 0;

            InitializeShader();
            InitializeBuffers();

            reaction_force_label = new label_list_store();

        }


        private void InitializeShader()
        {
            // Initialize the Shader 
            reactionForceShader = new Shader(
                ShaderLibrary.get_vertex_shader(ShaderLibrary.ShaderType.ReactionForceShader),
                ShaderLibrary.get_fragment_shader(ShaderLibrary.ShaderType.ReactionForceShader)
                );

        }


        private void InitializeBuffers()
        {
            // Initialize the Buffer
            reactionForce_vao = new VertexArray();
            reactionForce_vbo = new VertexBuffer(10);
            reactionForce_ibo = new IndexBuffer(10);

            VertexBufferLayout reactionForceLayout = new VertexBufferLayout();
            reactionForceLayout.AddFloat(2);
            reactionForceLayout.AddFloat(2);
            reactionForceLayout.AddFloat(1);

            reactionForce_vao.Add_vertexBuffer(reactionForce_vbo, reactionForceLayout);

        }


        public void add_reactionForce(int point_id,
                        double x_coord,
                        double y_coord,
                        int constraint_type, // 0 = free, 1 = pinned, 2 = roller
                        double constraint_angle,
                        double reaction_x,
                        double reaction_y)
        {

            double cos_theta = Math.Cos((180.0 - constraint_angle) * Math.PI / 180.0);
            double sin_theta = Math.Sin((180.0 - constraint_angle) * Math.PI / 180.0);

            double transformed_reaction_x = reaction_x * cos_theta + reaction_y * sin_theta;
            double transformed_reaction_y = -reaction_x * sin_theta + reaction_y * cos_theta;



            // Add the Load to the list
            reactionforce_store temp_reaction = new reactionforce_store
            {
                point_id = point_id,
                x_coord = x_coord,
                y_coord = y_coord,
                constraint_type = constraint_type,
                constraint_angle = constraint_angle,
                global_reaction_x = reaction_x,
                global_reaction_y = reaction_y,
                transformed_reaction_x = transformed_reaction_x,
                transformed_reaction_y = transformed_reaction_y
            };


            reactionForceMap[point_id] = temp_reaction;

            //loadMap[unique_loadset_id] = temp_load;
            //load_set_count++;

            //// Update the load data visualization
            //update_buffer_data();

        }




        public void paint_reaction_forces()
        {
            // node reaction force count check
            if (reactionForceMap.Count == 0 || gvariables_static.is_paint_loads == false)
                return;

            reactionForceShader.Bind();

            reactionForce_vao.Bind();
            reactionForce_ibo.Bind();

            // Paint the Load Line
            GL.LineWidth(3.0f);
            GL.DrawElements(PrimitiveType.Lines, reactionForce_ibo.BufferCount, DrawElementsType.UnsignedInt, 0);
            GL.LineWidth(1.0f);

            reactionForce_vao.UnBind();
            reactionForce_ibo.UnBind();

            reactionForceShader.UnBind();

            // Paint the reaction force label
            reaction_force_label.paint_static_labels();

        }



        public void update_openTK_uniforms(drawing_events graphic_events_control)
        {
            if (reactionForceMap.Count == 0)
                return;

            Matrix4 uMVP = graphic_events_control.projectionMatrix *
                                     graphic_events_control.viewMatrix *
                                     graphic_events_control.modelMatrix;

            float zoomscale = (float)graphic_events_control.zoom_val;

            reactionForceShader.SetMatrix4("uMVP", uMVP);
            reactionForceShader.SetFloat("zoomscale", zoomscale);

            Vector4 LoadColor = new Vector4(gvariables_static.ColorUtils.get_LoadColor(),
        gvariables_static.rslt_transparency * 0.8f);


            reactionForceShader.SetVector4("vertexColor", LoadColor);

            // Update the label uniforms
            reaction_force_label.update_openTK_uniforms(uMVP, zoomscale, gvariables_static.rslt_transparency);

        }


        public void update_buffer_data()
        {
            // Transformed reaction force (to local coordinates) visualization


            //_______________________________________________________________
            // prepare the Vertex data for openGL
            List<float> reactionVertexData = new List<float>();
            List<int> reactionIndexData = new List<int>();

            // Get the load size
            float load_size = gvariables_static.get_font_scale(8.0f);


            // Get the load max
            float load_max = 0.0f;

            foreach (reactionforce_store reactionForce_data in reactionForceMap.Values)
            {
                load_max = (float)Math.Max(load_max, Math.Abs(reactionForce_data.transformed_reaction_x));
                load_max = (float)Math.Max(load_max, Math.Abs(reactionForce_data.transformed_reaction_y));
            }


            int t_id = 0;
            int label_id = 0;

            reaction_force_label.clear_labels();

            // X Reaction force
            foreach (reactionforce_store reactionForce_data in reactionForceMap.Values)
            {

                int load_sign = reactionForce_data.transformed_reaction_x > 0 ? 1 : -1;
                float ld_visualization_factor = load_sign * load_size;
                float ld_scale = (float)(reactionForce_data.transformed_reaction_x / load_max) * load_size;

                float arrowLength = -20.0f * ld_scale;
                float arrowheadSize = -4.0f * ld_visualization_factor;
                float arrowheadAngle = 10.0f;


                double constraint_angle = -reactionForce_data.constraint_angle + 180.0f; // reactionForce_data.constraint_angle;

                Vector2 startpt = gvariables_static.RotatePoint(new Vector2(0, 0), new Vector2(-0.1f * ld_visualization_factor, 0.0f), constraint_angle);
                Vector2 tailpt = gvariables_static.RotatePoint(new Vector2(0, 0), new Vector2(arrowLength, 0.0f), constraint_angle);
                Vector2 arrowheadpt1 = gvariables_static.RotatePoint(new Vector2(0, 0), new Vector2(arrowheadSize, 0.0f), constraint_angle + arrowheadAngle);
                Vector2 arrowheadpt2 = gvariables_static.RotatePoint(new Vector2(0, 0), new Vector2(arrowheadSize, 0.0f), constraint_angle - arrowheadAngle);



                Vector2 reaction_node_pt = new Vector2((float)reactionForce_data.x_coord, (float)reactionForce_data.y_coord);

                // Load arrow start point
                reactionVertexData.Add(reaction_node_pt.X + startpt.X);
                reactionVertexData.Add(reaction_node_pt.Y + startpt.Y);
                reactionVertexData.Add(reaction_node_pt.X);
                reactionVertexData.Add(reaction_node_pt.Y);
                reactionVertexData.Add(1.0f);


                // Load arrow tail point
                reactionVertexData.Add(reaction_node_pt.X + tailpt.X);
                reactionVertexData.Add(reaction_node_pt.Y + tailpt.Y);
                reactionVertexData.Add(reaction_node_pt.X);
                reactionVertexData.Add(reaction_node_pt.Y);
                reactionVertexData.Add(0.0f);


                // Load arrow pt 1
                reactionVertexData.Add(reaction_node_pt.X + arrowheadpt1.X);
                reactionVertexData.Add(reaction_node_pt.Y + arrowheadpt1.Y);
                reactionVertexData.Add(reaction_node_pt.X);
                reactionVertexData.Add(reaction_node_pt.Y);
                reactionVertexData.Add(1.0f);

                // Load arrow pt 2
                reactionVertexData.Add(reaction_node_pt.X + arrowheadpt2.X);
                reactionVertexData.Add(reaction_node_pt.Y + arrowheadpt2.Y);
                reactionVertexData.Add(reaction_node_pt.X);
                reactionVertexData.Add(reaction_node_pt.Y);
                reactionVertexData.Add(1.0f);

                // Set the node indices
                // Load Line 1
                reactionIndexData.Add(t_id + 0);
                reactionIndexData.Add(t_id + 1);

                // Arrow head line 1
                reactionIndexData.Add(t_id + 0);
                reactionIndexData.Add(t_id + 2);

                // Arrow head line 2
                reactionIndexData.Add(t_id + 0);
                reactionIndexData.Add(t_id + 3);

                t_id = t_id + 4;


                // Create the reaction force label
                // Add labels
                string label_string1 = FormatReactionForceValue((float)Math.Abs(reactionForce_data.transformed_reaction_x));

                Vector2 label_loc1 = new Vector2((float)reactionForce_data.x_coord + tailpt.X,
                    (float)reactionForce_data.y_coord + tailpt.Y);


                reaction_force_label.add_label(label_id + 0, label_string1, label_loc1, gvariables_static.ColorUtils.get_LoadColor());

                label_id++;

            }


            // Y Reaction force
            foreach (reactionforce_store reactionForce_data in reactionForceMap.Values)
            {

                int load_sign = reactionForce_data.transformed_reaction_y > 0 ? 1 : -1;
                float ld_visualization_factor = load_sign * load_size;
                float ld_scale = (float)(reactionForce_data.transformed_reaction_y / load_max) * load_size;

                float arrowLength = -20.0f * ld_scale;
                float arrowheadSize = -4.0f * ld_visualization_factor;
                float arrowheadAngle = 10.0f;


                double constraint_angle = (90.0 - reactionForce_data.constraint_angle) + 180.0f; // reactionForce_data.constraint_angle;

                Vector2 startpt = gvariables_static.RotatePoint(new Vector2(0, 0), new Vector2(-0.1f * ld_visualization_factor, 0.0f), constraint_angle);
                Vector2 tailpt = gvariables_static.RotatePoint(new Vector2(0, 0), new Vector2(arrowLength, 0.0f), constraint_angle);
                Vector2 arrowheadpt1 = gvariables_static.RotatePoint(new Vector2(0, 0), new Vector2(arrowheadSize, 0.0f), constraint_angle + arrowheadAngle);
                Vector2 arrowheadpt2 = gvariables_static.RotatePoint(new Vector2(0, 0), new Vector2(arrowheadSize, 0.0f), constraint_angle - arrowheadAngle);



                Vector2 reaction_node_pt = new Vector2((float)reactionForce_data.x_coord, (float)reactionForce_data.y_coord);

                // Load arrow start point
                reactionVertexData.Add(reaction_node_pt.X + startpt.X);
                reactionVertexData.Add(reaction_node_pt.Y + startpt.Y);
                reactionVertexData.Add(reaction_node_pt.X);
                reactionVertexData.Add(reaction_node_pt.Y);
                reactionVertexData.Add(1.0f);


                // Load arrow tail point
                reactionVertexData.Add(reaction_node_pt.X + tailpt.X);
                reactionVertexData.Add(reaction_node_pt.Y + tailpt.Y);
                reactionVertexData.Add(reaction_node_pt.X);
                reactionVertexData.Add(reaction_node_pt.Y);
                reactionVertexData.Add(0.0f);


                // Load arrow pt 1
                reactionVertexData.Add(reaction_node_pt.X + arrowheadpt1.X);
                reactionVertexData.Add(reaction_node_pt.Y + arrowheadpt1.Y);
                reactionVertexData.Add(reaction_node_pt.X);
                reactionVertexData.Add(reaction_node_pt.Y);
                reactionVertexData.Add(1.0f);


                // Load arrow pt 2
                reactionVertexData.Add(reaction_node_pt.X + arrowheadpt2.X);
                reactionVertexData.Add(reaction_node_pt.Y + arrowheadpt2.Y);
                reactionVertexData.Add(reaction_node_pt.X);
                reactionVertexData.Add(reaction_node_pt.Y);
                reactionVertexData.Add(1.0f);


                // Set the node indices
                // Load Line 1
                reactionIndexData.Add(t_id + 0);
                reactionIndexData.Add(t_id + 1);

                // Arrow head line 1
                reactionIndexData.Add(t_id + 0);
                reactionIndexData.Add(t_id + 2);

                // Arrow head line 2
                reactionIndexData.Add(t_id + 0);
                reactionIndexData.Add(t_id + 3);

                t_id = t_id + 4;


                // Create the reaction force label
                // Add labels
                string label_string1 = FormatReactionForceValue((float)Math.Abs(reactionForce_data.transformed_reaction_y));

                Vector2 label_loc1 = new Vector2((float)reactionForce_data.x_coord + tailpt.X,
                    (float)reactionForce_data.y_coord + tailpt.Y);


                reaction_force_label.add_label(label_id + 0, label_string1, label_loc1, gvariables_static.ColorUtils.get_LoadColor());

                label_id++;

            }





            // Update the label buffer
            reaction_force_label.update_buffer(gvariables_static.geom_size * 0.85f);


            // Clear and update buffers
            if (reactionVertexData.Count > 0)
            {
                // Convert to array and upload
                float[] vertexArray = reactionVertexData.ToArray();
                int[] indexArray = reactionIndexData.ToArray();

                // Clear existing data
                reactionForce_vbo.ClearVertexBuffer();
                reactionForce_ibo.ClearIndexBuffer();

                // Upload new data
                reactionForce_vbo.AppendVertexBuffer(vertexArray);
                reactionForce_ibo.AppendIndexBuffer(indexArray);

            }
            else
            {

                // Clear buffers if no data
                reactionForce_vbo.ClearVertexBuffer();
                reactionForce_ibo.ClearIndexBuffer();

            }

        }





        public void update_buffer_data11()
        {

            // Buffer data
            // Global Visualization of reaction forces



            //_______________________________________________________________
            // prepare the Vertex data for openGL
            List<float> reactionVertexData = new List<float>();
            List<int> reactionIndexData = new List<int>();

            // Get the load size
            float load_size = gvariables_static.get_font_scale(8.0f);


            // Get the load max
            float load_max = 0.0f;

            foreach (reactionforce_store reactionForce_data in reactionForceMap.Values)
            {
                load_max = (float)Math.Max(load_max, Math.Abs(reactionForce_data.global_reaction_x));
                load_max = (float)Math.Max(load_max, Math.Abs(reactionForce_data.global_reaction_y));
            }


            int t_id = 0;
            int label_id = 0;

            reaction_force_label.clear_labels();

            // X Reaction force
            foreach (reactionforce_store reactionForce_data in reactionForceMap.Values)
            {

                int load_sign = reactionForce_data.global_reaction_x > 0 ? 1 : -1;
                float ld_visualization_factor = load_sign * load_size;
                float ld_scale = (float)(reactionForce_data.global_reaction_x / load_max) * load_size;

                float arrowLength = -20.0f * ld_scale;
                float arrowheadSize = -4.0f * ld_visualization_factor;
                float arrowheadAngle = 10.0f;


                double constraint_angle = 0.0; // reactionForce_data.constraint_angle;

                Vector2 startpt = gvariables_static.RotatePoint(new Vector2(0, 0), new Vector2(-0.1f * ld_visualization_factor, 0.0f), constraint_angle);
                Vector2 tailpt = gvariables_static.RotatePoint(new Vector2(0, 0), new Vector2(arrowLength, 0.0f), constraint_angle);
                Vector2 arrowheadpt1 = gvariables_static.RotatePoint(new Vector2(0, 0), new Vector2(arrowheadSize, 0.0f), constraint_angle + arrowheadAngle);
                Vector2 arrowheadpt2 = gvariables_static.RotatePoint(new Vector2(0, 0), new Vector2(arrowheadSize, 0.0f), constraint_angle - arrowheadAngle);



                Vector2 reaction_node_pt = new Vector2((float)reactionForce_data.x_coord, (float)reactionForce_data.y_coord);

                // Load arrow start point
                reactionVertexData.Add(reaction_node_pt.X + startpt.X);
                reactionVertexData.Add(reaction_node_pt.Y + startpt.Y);
                reactionVertexData.Add(reaction_node_pt.X);
                reactionVertexData.Add(reaction_node_pt.Y); 
                reactionVertexData.Add(1.0f);


                // Load arrow tail point
                reactionVertexData.Add(reaction_node_pt.X + tailpt.X);
                reactionVertexData.Add(reaction_node_pt.Y + tailpt.Y);
                reactionVertexData.Add(reaction_node_pt.X);
                reactionVertexData.Add(reaction_node_pt.Y);
                reactionVertexData.Add(0.0f);


                // Load arrow pt 1
                reactionVertexData.Add(reaction_node_pt.X + arrowheadpt1.X);
                reactionVertexData.Add(reaction_node_pt.Y + arrowheadpt1.Y);
                reactionVertexData.Add(reaction_node_pt.X);
                reactionVertexData.Add(reaction_node_pt.Y);
                reactionVertexData.Add(1.0f);

                // Load arrow pt 2
                reactionVertexData.Add(reaction_node_pt.X + arrowheadpt2.X);
                reactionVertexData.Add(reaction_node_pt.Y + arrowheadpt2.Y);
                reactionVertexData.Add(reaction_node_pt.X);
                reactionVertexData.Add(reaction_node_pt.Y);
                reactionVertexData.Add(1.0f);

                // Set the node indices
                // Load Line 1
                reactionIndexData.Add(t_id + 0);
                reactionIndexData.Add(t_id + 1);

                // Arrow head line 1
                reactionIndexData.Add(t_id + 0);
                reactionIndexData.Add(t_id + 2);

                // Arrow head line 2
                reactionIndexData.Add(t_id + 0);
                reactionIndexData.Add(t_id + 3);

                t_id = t_id + 4;


                // Create the reaction force label
                // Add labels
                string label_string1 = FormatReactionForceValue((float)Math.Abs(reactionForce_data.global_reaction_x));

                Vector2 label_loc1 = new Vector2((float)reactionForce_data.x_coord + tailpt.X, 
                    (float)reactionForce_data.y_coord + tailpt.Y);
                

                reaction_force_label.add_label(label_id + 0, label_string1, label_loc1, gvariables_static.ColorUtils.get_LoadColor());
                
                label_id++;

            }


            // Y Reaction force
            foreach (reactionforce_store reactionForce_data in reactionForceMap.Values)
            {

                int load_sign = reactionForce_data.global_reaction_y > 0 ? 1 : -1;
                float ld_visualization_factor = load_sign * load_size;
                float ld_scale = (float)(reactionForce_data.global_reaction_y / load_max) * load_size;

                float arrowLength = -20.0f * ld_scale;
                float arrowheadSize = -4.0f * ld_visualization_factor;
                float arrowheadAngle = 10.0f;


                double constraint_angle = 90.0; // reactionForce_data.constraint_angle;

                Vector2 startpt = gvariables_static.RotatePoint(new Vector2(0, 0), new Vector2(-0.1f * ld_visualization_factor, 0.0f), constraint_angle);
                Vector2 tailpt = gvariables_static.RotatePoint(new Vector2(0, 0), new Vector2(arrowLength, 0.0f), constraint_angle);
                Vector2 arrowheadpt1 = gvariables_static.RotatePoint(new Vector2(0, 0), new Vector2(arrowheadSize, 0.0f), constraint_angle + arrowheadAngle);
                Vector2 arrowheadpt2 = gvariables_static.RotatePoint(new Vector2(0, 0), new Vector2(arrowheadSize, 0.0f), constraint_angle - arrowheadAngle);



                Vector2 reaction_node_pt = new Vector2((float)reactionForce_data.x_coord, (float)reactionForce_data.y_coord);

                // Load arrow start point
                reactionVertexData.Add(reaction_node_pt.X + startpt.X);
                reactionVertexData.Add(reaction_node_pt.Y + startpt.Y);
                reactionVertexData.Add(reaction_node_pt.X);
                reactionVertexData.Add(reaction_node_pt.Y);
                reactionVertexData.Add(1.0f);


                // Load arrow tail point
                reactionVertexData.Add(reaction_node_pt.X + tailpt.X);
                reactionVertexData.Add(reaction_node_pt.Y + tailpt.Y);
                reactionVertexData.Add(reaction_node_pt.X);
                reactionVertexData.Add(reaction_node_pt.Y);
                reactionVertexData.Add(0.0f);


                // Load arrow pt 1
                reactionVertexData.Add(reaction_node_pt.X + arrowheadpt1.X);
                reactionVertexData.Add(reaction_node_pt.Y + arrowheadpt1.Y);
                reactionVertexData.Add(reaction_node_pt.X);
                reactionVertexData.Add(reaction_node_pt.Y);
                reactionVertexData.Add(1.0f);


                // Load arrow pt 2
                reactionVertexData.Add(reaction_node_pt.X + arrowheadpt2.X);
                reactionVertexData.Add(reaction_node_pt.Y + arrowheadpt2.Y);
                reactionVertexData.Add(reaction_node_pt.X);
                reactionVertexData.Add(reaction_node_pt.Y);
                reactionVertexData.Add(1.0f);


                // Set the node indices
                // Load Line 1
                reactionIndexData.Add(t_id + 0);
                reactionIndexData.Add(t_id + 1);

                // Arrow head line 1
                reactionIndexData.Add(t_id + 0);
                reactionIndexData.Add(t_id + 2);

                // Arrow head line 2
                reactionIndexData.Add(t_id + 0);
                reactionIndexData.Add(t_id + 3);

                t_id = t_id + 4;


                // Create the reaction force label
                // Add labels
                string label_string1 = FormatReactionForceValue((float)Math.Abs(reactionForce_data.global_reaction_y));

                Vector2 label_loc1 = new Vector2((float)reactionForce_data.x_coord + tailpt.X,
                    (float)reactionForce_data.y_coord + tailpt.Y);


                reaction_force_label.add_label(label_id + 0, label_string1, label_loc1, gvariables_static.ColorUtils.get_LoadColor());

                label_id++;

            }





            // Update the label buffer
            reaction_force_label.update_buffer(gvariables_static.geom_size * 0.85f);


            // Clear and update buffers
            if (reactionVertexData.Count > 0)
            {
                // Convert to array and upload
                float[] vertexArray = reactionVertexData.ToArray();
                int[] indexArray = reactionIndexData.ToArray();

                // Clear existing data
                reactionForce_vbo.ClearVertexBuffer();
                reactionForce_ibo.ClearIndexBuffer();

                // Upload new data
                reactionForce_vbo.AppendVertexBuffer(vertexArray);
                reactionForce_ibo.AppendIndexBuffer(indexArray);

            }
            else
            {

                // Clear buffers if no data
                reactionForce_vbo.ClearVertexBuffer();
                reactionForce_ibo.ClearIndexBuffer();

            }


        }





        private string FormatReactionForceValue(float value)
        {
            // Determine precision based on value magnitude
            float absValue = Math.Abs(value);

            if(absValue < 1E-8)
                return "0";
            else if (absValue < 0.0001f)
                return value.ToString("F7");
            else if (absValue < 0.001f)
                return value.ToString("F6");
            else if (absValue < 0.01f)
                return value.ToString("F5");
            else if (absValue < 0.1f)
                return value.ToString("F4");
            else if (absValue < 1.0f)
                return value.ToString("F3");
            else if (absValue < 10.0f)
                return value.ToString("F2");
            else if (absValue < 100.0f)
                return value.ToString("F1");
            else
                return value.ToString("F0");
        }




    }
}
