using OpenTK;
using Plane_stress_analyzer_PSL.Resources;
using Plane_stress_analyzer_PSL.src.events_handler;
using Plane_stress_analyzer_PSL.src.global_variables;
using Plane_stress_analyzer_PSL.src.opentk_control.opentk_buffer;
using Plane_stress_analyzer_PSL.src.opentk_control.shader_compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plane_stress_analyzer_PSL.src.model_store.fe_objects
{

    public class nodecnst_data
    {
        public int cnst_set_id { get; set; } // constraint id

        public List<Vector2> constraint_node_pts { get; set; }

        public List<int> constraint_node_ids { get; set; }

        public int constraint_type { get; set; }// Constraint Type = 0 & 1

        public double constraint_angle { get; set; } // Constraint Angle

    }



    public class nodecnst_list_store
    {
        public Dictionary<int, nodecnst_data> cnstMap = new Dictionary<int, nodecnst_data>();
        public int cnst_set_count = 0;

        private List<int> all_constraintset_ids = new List<int>();


        // Constraint visualization
        private Shader constraintShader;
        private Texture constraintTexture_Pin;
        private Texture constraintTexture_Roller;

        // Vertex Buffer object and Vertex Array object 
        private VertexBuffer constraint_vbo;
        private VertexArray constraint_vao;
        private IndexBuffer constraint_ibo;


        public nodecnst_list_store()
        {
            // (Re)Initialize the data
            cnstMap = new Dictionary<int, nodecnst_data>();
            cnst_set_count = 0;

            InitializeShader();
            InitializeBuffers();
        }


        public void add_nodeconstraint(List<int> constraint_node_ids, List<Vector2> constraint_node_pts,
            int t_constraint_type, double t_constraint_angle)
        {
            // Get an unique constraint set id
            int unique_constraintset_id = gvariables_static.get_unique_id(all_constraintset_ids);

            // Make a copy of the list
            List<int> idsCopy = new List<int>(constraint_node_ids);
            List<Vector2> nodePtsCopy = new List<Vector2>(constraint_node_pts);

            // Add the constraint to the particular node
            nodecnst_data temp_cnst = new nodecnst_data
            {
                cnst_set_id = unique_constraintset_id,
                constraint_node_pts = nodePtsCopy,
                constraint_node_ids = idsCopy,
                constraint_type = t_constraint_type,
                constraint_angle = t_constraint_angle
            };

            // Insert the constraint to nodes
            cnstMap[unique_constraintset_id] = temp_cnst;
            cnst_set_count++;

            // Set the constraint data visualization
            set_constraint_visualization(unique_constraintset_id, true);

            // Add the constraint set id to list to track the unique constraint set id
            all_constraintset_ids.Add(unique_constraintset_id);

        }

        public void delete_nodeconstraint(int cnst_set_id)
        {
            // Remove the constraint set ID from all_constraintset_ids
            all_constraintset_ids.Remove(cnst_set_id);

            // Set the constraint data visualization
            set_constraint_visualization(cnst_set_id, false);

            // Remove the constraint data based on the key (constraint set id)
            cnstMap.Remove(cnst_set_id);

            // adjust the constraint data count
            cnst_set_count--;
        }


        private void set_constraint_visualization(int cnst_set_id, bool isAdd)
        {
            // Get the constraint
            nodecnst_data ndcnstraint = cnstMap[cnst_set_id];

            if (isAdd == true)
            {
                // Add visualization for this constraint set id
 

            }
            else
            {
                // Delete visualization for this constraint set id
              

            }

            //cnst_meshdata.set_shader();
            // ndcnst_meshdata.set_buffer();

        }

        public void InitializeShader()
        {
            // Initialize the Shader 
            constraintShader = new Shader(
                ShaderLibrary.get_vertex_shader(ShaderLibrary.ShaderType.ConstraintShader),
                ShaderLibrary.get_fragment_shader(ShaderLibrary.ShaderType.ConstraintShader)
                );


            System.Drawing.Bitmap pin_support = Resource_font.pic_pin_support;
            constraintTexture_Pin = new Texture();
            constraintTexture_Pin.LoadTexture(pin_support);

            System.Drawing.Bitmap roller_support = Resource_font.pic_roller_support;
            constraintTexture_Roller = new Texture();
            constraintTexture_Roller.LoadTexture(roller_support);

            // Set texture uniform variables
            constraintShader.SetInt("u_Textures[0]", 0);
            constraintShader.SetInt("u_Textures[1]", 1);

        }


        public void InitializeBuffers()
        {
            // Initialize the Buffer
            constraint_vao = new VertexArray();
            constraint_vbo = new VertexBuffer(10);
            constraint_ibo = new IndexBuffer(10);
            
            VertexBufferLayout constraintLayout = new VertexBufferLayout();
            constraintLayout.AddFloat(2);
            constraintLayout.AddFloat(2);
            constraintLayout.AddFloat(2);
            constraintLayout.AddFloat(1);

            constraint_vao.Add_vertexBuffer(constraint_vbo, constraintLayout);
        }


        public void paint_node_constraint()
        {
            // node constraint count check
            if (cnst_set_count == 0)
                return;

            constraintShader.Bind();

            // Activate textures (pin and roller support)
            constraintTexture_Pin.Bind(0);
            constraintTexture_Roller.Bind(1);

            // Paint the constraint label
            gvariables_static.LineWidth = 3.0f;
           //  ndcnst_meshdata.paint_static_mesh_lines();
            gvariables_static.LineWidth = 1.0f;



            constraintTexture_Pin.UnBind();
            constraintTexture_Roller.UnBind();
            constraintShader.UnBind();

        }



        public void update_openTK_uniforms(drawing_events graphic_events_control)
        {
            if (cnst_set_count == 0)
                return;

            Matrix4 uMVP = graphic_events_control.projectionMatrix *
    graphic_events_control.viewMatrix * graphic_events_control.modelMatrix;

            float zoomscale = (float)graphic_events_control.zoom_val;

            constraintShader.SetMatrix4("uMVP", uMVP);
            constraintShader.SetFloat("zoomscale", zoomscale);

        }


    }
}
