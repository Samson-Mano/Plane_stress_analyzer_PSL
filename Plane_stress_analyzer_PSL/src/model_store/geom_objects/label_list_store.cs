// OpenTK library
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using Plane_stress_analyzer_PSL.src.events_handler;
using Plane_stress_analyzer_PSL.src.global_variables;
using Plane_stress_analyzer_PSL.src.opentk_control.opentk_buffer;
using Plane_stress_analyzer_PSL.src.opentk_control.shader_compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace Plane_stress_analyzer_PSL.src.model_store.geom_objects
{
    public class label_store
    {
        // Stores the individual label
        public int label_id { get; set; }
        public string label { get; set; }
        public Vector2 label_loc { get; set; }
        public Vector3 label_color { get; set; }
        public double label_angle { get; set; }
        public bool label_above_loc { get; set; }
        public int label_char_count { get; set; }

    }


    public class label_list_store
    {
        public Dictionary<int, label_store> labelMap { get; } = new Dictionary<int, label_store>();
        public int label_count = 0;
        public int total_char_count = 0;
        public float font_size = 12.0f;

        private VertexArray _labelVAO;
        private VertexBuffer _labelVBO;
        private IndexBuffer _labelIBO;

        public Shader _labelshader;


        public label_list_store()
        {
            // (Re)Initialize the data
            labelMap = new Dictionary<int, label_store>();
            label_count = 0;
            total_char_count = 0;


            InitializeShader();
            InitializeBuffers();

        }


        public void add_label(int label_id, string label, Vector2 label_loc, int color_id)
        {
            // Add the Label to the list
            label_store temp_label = new label_store
            {
                label_id = label_id,
                label = label,
                label_loc = label_loc,
                label_color = gvariables_static.ColorUtils.MeshGetRandomColor(color_id),
                label_angle = 0.0, // radian
                label_above_loc = true,
                label_char_count = label.Length
            };

            labelMap[label_id] = temp_label;
            label_count++;

            // Add to the total character count
            total_char_count = total_char_count + label.Length;

            update_buffer();
        }



        public void delete_label(int label_id)
        {
            // Adjust the total character count
            int label_char_count = labelMap[label_id].label_char_count;
            total_char_count = total_char_count - label_char_count;

            // Delete the label
            labelMap.Remove(label_id);
            label_count--;

            update_buffer();

        }


        private void InitializeShader()
        {

            // Create Shader
            _labelshader = new Shader(ShaderLibrary.get_vertex_shader(ShaderLibrary.ShaderType.TextShader),
                ShaderLibrary.get_fragment_shader(ShaderLibrary.ShaderType.TextShader));

        }


        private void InitializeBuffers()
        {
            // Label buffer
            _labelVAO = new VertexArray();
            _labelVBO = new VertexBuffer(100);
            _labelIBO = new IndexBuffer(100);

            var labelLayout = new VertexBufferLayout();
            labelLayout.AddFloat(2);  // Character Position
            labelLayout.AddFloat(2);  // Text location
            labelLayout.AddFloat(2);  // Texture coordinate
            labelLayout.AddFloat(3);  // Text color


            _labelVAO.Add_vertexBuffer(_labelVBO, labelLayout);

        }


        public void update_buffer()
        {
            // Set the buffer for index (6 indices to form a two triangle, quadrilateral )
            int label_indices_count = 6 * total_char_count; // 6 index per character
            int[] label_indices = new int[label_indices_count];

            int label_i_index = 0;

            // Set the label index buffers
            foreach (var lb in labelMap)
            {
                get_label_index_buffer(lb.Value, ref label_indices, ref label_i_index);
            }

            // Update the index buffer
            _labelIBO.ClearIndexBuffer();
            _labelIBO.AppendIndexBuffer(label_indices);


            // Define the vertex buffer size for a character
            // 4 vertex to form a quadrilateral ( 2 char position, 2 text location, 2 texture coord, 3 color)
            int label_vertex_count = 4 * 9 * total_char_count;
            float[] label_vertices = new float[label_vertex_count];

            int label_v_index = 0;

            // Set the label vertex buffers
            foreach (var lb in labelMap)
            {
                // Add vertex buffers
                get_label_vertex_buffer(lb.Value, ref label_vertices, ref label_v_index);
            }

            int label_vertex_size = label_vertex_count * sizeof(float); // Size of the label vertex buffer

            // Update the vertex buffer
            _labelVBO.ClearVertexBuffer();
            _labelVBO.AppendVertexBuffer(label_vertices);
            
        }



        public void clear_labels()
        {
            // Clear the data
            labelMap.Clear();
            label_count = 0;
            total_char_count = 0;

        }


        public void update_openTK_uniforms(drawing_events graphic_events_control)
        {
            // Update the openGl uniform matrices

                // Set the model matrix
                _labelshader.SetMatrix4("modelMatrix", graphic_events_control.modelMatrix);

                // Set the projection matrix
                _labelshader.SetMatrix4("projectionMatrix", graphic_events_control.projectionMatrix);

  
                // Set the view matrix
                _labelshader.SetMatrix4("viewMatrix", graphic_events_control.viewMatrix);

  
                // Set the transparency float
                _labelshader.SetFloat("vertexTransparency", gvariables_static.geom_transparency);

        }


        public void paint_static_labels()
        {
            // Paint all the static points
            _labelshader.Bind();
            _labelVAO.Bind();
            _labelIBO.Bind();

            // Activate texture unit 0
            GL.ActiveTexture(TextureUnit.Texture0);

            // Bind the texture to the active texture unit
            GL.BindTexture(TextureTarget.Texture2D, gvariables_static.main_font.TextureID);

            // Draw the elements
            GL.DrawElements(PrimitiveType.Triangles, 6 * total_char_count, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Unbind the texture
            GL.BindTexture(TextureTarget.Texture2D, 0);


            _labelshader.UnBind();
            _labelVAO.UnBind();
            _labelIBO.UnBind();

        }


        private void get_label_vertex_buffer(label_store lb, ref float[] label_vertices, ref int label_v_index)
        {

            float font_scale = gvariables_static.get_font_scale(font_size);

            // Find the label total width and total height of the label
            float total_label_width = 0.0f;
            float total_label_height = 0.0f;

            // lb.label[i] != '\0'

            for (int i = 0; i < lb.label_char_count; ++i)
            {
                // get the atlas information
                char ch = lb.label[i];
                Character ch_data = gvariables_static.main_font.Glyphs[ch];

                total_label_width += ch_data.Advance * font_scale;
                total_label_height = Math.Max(total_label_height, ch_data.Size.Y * font_scale);
            }


            // Get the x,y location
            Vector2 loc = lb.label_loc;
            float x = loc.X - (total_label_width * 0.5f);

            // Whether paint above the location or not
            float y = 0.0f;
            if (lb.label_above_loc == true)
            {
                y = loc.Y + (total_label_height * 0.5f);
            }
            else
            {
                y = loc.Y - (total_label_height + (total_label_height * 0.5f));
            }


            Vector2 rotated_pt = new Vector2(0, 0);

            for (int i = 0; i < lb.label_char_count; ++i)
            {
                // get the atlas information
                char ch = lb.label[i];

                Character ch_data = gvariables_static.main_font.Glyphs[ch];

                float xpos = x + (ch_data.Bearing.X * font_scale);
                float ypos = y - (ch_data.Size.Y - ch_data.Bearing.Y) * font_scale;

                float w = ch_data.Size.X * font_scale;
                float h = ch_data.Size.Y * font_scale;

                float margin = 0.00022f; // This value prevents the minor overlap with the next char when rendering

                // Point 1
                // Vertices [0,0] // 0th point
                rotated_pt = gvariables_static.RotatePoint(loc, new Vector2(xpos, ypos + h), lb.label_angle);

                // Character location
                label_vertices[label_v_index + 0] = rotated_pt.X;
                label_vertices[label_v_index + 1] = rotated_pt.Y;

                // character origin
                label_vertices[label_v_index + 2] = loc.X;
                label_vertices[label_v_index + 3] = loc.Y;

                // Texture Glyph coordinate
                label_vertices[label_v_index + 4] = ch_data.TopLeft.X + margin;
                label_vertices[label_v_index + 5] = ch_data.TopLeft.Y;

                // Text color
                label_vertices[label_v_index + 6] = lb.label_color.X;
                label_vertices[label_v_index + 7] = lb.label_color.Y;
                label_vertices[label_v_index + 8] = lb.label_color.Z;

                // Iterate
                label_v_index = label_v_index + 9;

                //__________________________________________________________________________________________

                // Point 2
                // Vertices [0,1] // 1th point
                rotated_pt = gvariables_static.RotatePoint(loc, new Vector2(xpos, ypos), lb.label_angle);

                // Character location
                label_vertices[label_v_index + 0] = rotated_pt.X;
                label_vertices[label_v_index + 1] = rotated_pt.Y;

                // character origin
                label_vertices[label_v_index + 2] = loc.X;
                label_vertices[label_v_index + 3] = loc.Y;

                // Texture Glyph coordinate
                label_vertices[label_v_index + 4] = ch_data.TopLeft.X + margin;
                label_vertices[label_v_index + 5] = ch_data.BottomRight.Y;

                // Text color
                label_vertices[label_v_index + 6] = lb.label_color.X;
                label_vertices[label_v_index + 7] = lb.label_color.Y;
                label_vertices[label_v_index + 8] = lb.label_color.Z;

                // Iterate
                label_v_index = label_v_index + 9;

                //__________________________________________________________________________________________

                // Point 3
                // Vertices [1,1] // 2th point
                rotated_pt = gvariables_static.RotatePoint(loc, new Vector2(xpos + w, ypos), lb.label_angle);

                // Character location
                label_vertices[label_v_index + 0] = rotated_pt.X;
                label_vertices[label_v_index + 1] = rotated_pt.Y;

                // character origin
                label_vertices[label_v_index + 2] = loc.X;
                label_vertices[label_v_index + 3] = loc.Y;

                // Texture Glyph coordinate
                label_vertices[label_v_index + 4] = ch_data.BottomRight.X - margin;
                label_vertices[label_v_index + 5] = ch_data.BottomRight.Y;

                // Text color
                label_vertices[label_v_index + 6] = lb.label_color.X;
                label_vertices[label_v_index + 7] = lb.label_color.Y;
                label_vertices[label_v_index + 8] = lb.label_color.Z;

                // Iterate
                label_v_index = label_v_index + 9;

                //__________________________________________________________________________________________

                // Point 4
                // Vertices [1,0] // 3th point
                rotated_pt = gvariables_static.RotatePoint(loc, new Vector2(xpos + w, ypos + h), lb.label_angle);

                // Character location
                label_vertices[label_v_index + 0] = rotated_pt.X;
                label_vertices[label_v_index + 1] = rotated_pt.Y;

                // character origin
                label_vertices[label_v_index + 2] = loc.X;
                label_vertices[label_v_index + 3] = loc.Y;

                // Texture Glyph coordinate
                label_vertices[label_v_index + 4] = ch_data.BottomRight.X - margin;
                label_vertices[label_v_index + 5] = ch_data.TopLeft.Y;

                // Text color
                label_vertices[label_v_index + 6] = lb.label_color.X;
                label_vertices[label_v_index + 7] = lb.label_color.Y;
                label_vertices[label_v_index + 8] = lb.label_color.Z;

                // Iterate
                label_v_index = label_v_index + 9;

                //__________________________________________________________________________________________
                x += ch_data.Advance * font_scale;

            }

        }


        private void get_label_index_buffer(label_store lb, ref int[] label_indices, ref int label_i_index)
        {

            for (int i = 0; i < lb.label_char_count; ++i)
            {
                // Set the index buffers
                int t_id = ((label_i_index / 6) * 4);

                // Triangle 0,1,2
                label_indices[label_i_index + 0] = t_id + 0;
                label_indices[label_i_index + 1] = t_id + 1;
                label_indices[label_i_index + 2] = t_id + 2;

                // Triangle 2,3,0
                label_indices[label_i_index + 3] = t_id + 2;
                label_indices[label_i_index + 4] = t_id + 3;
                label_indices[label_i_index + 5] = t_id + 0;

                // Increment
                label_i_index = label_i_index + 6;
            }

        }

    }


}
