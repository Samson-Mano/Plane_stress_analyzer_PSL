using OpenTK;
using Plane_stress_analyzer_PSL.src.events_handler;
using Plane_stress_analyzer_PSL.src.global_variables;
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

        public bool Is_Xfixed { get; set; } // X Fixed boundary condition

        public bool Is_Yfixed { get; set; } // X Fixed boundary condition

    }



    public class nodecnst_list_store
    {
        public Dictionary<int, nodecnst_data> cnstMap = new Dictionary<int, nodecnst_data>();
        public int cnst_set_count = 0;

        private List<int> all_constraintset_ids = new List<int>();
        
        
       // // Constraint visualization
        // private meshdata_store ndcnst_meshdata;


        public nodecnst_list_store()
        {
            // (Re)Initialize the data
            cnstMap = new Dictionary<int, nodecnst_data>();
            cnst_set_count = 0;

            // ndcnst_meshdata = new meshdata_store(false);

        }


        public void add_nodeconstraint(List<int> constraint_node_ids, List<Vector2> constraint_node_pts,
            bool t_Is_Xfixed, bool t_Is_Yfixed)
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
                Is_Xfixed = t_Is_Xfixed,
                Is_Yfixed = t_Is_Yfixed
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

        public void set_shader()
        {
            // Set the shader 

            // ndcnst_meshdata.set_shader();

        }


        public void paint_node_constraint()
        {
            // node constraint count check
            if (cnst_set_count == 0)
                return;

            // Paint the constraint label
            gvariables_static.LineWidth = 3.0f;
           //  ndcnst_meshdata.paint_static_mesh_lines();
            gvariables_static.LineWidth = 1.0f;

        }



        public void update_openTK_uniforms(bool set_modelmatrix, bool set_viewmatrix, bool set_transparency,
            drawing_events graphic_events_control)
        {
            if (cnst_set_count == 0)
                return;


            //ndcnst_meshdata.update_openTK_uniforms(graphic_events_control.projectionMatrix,
            //    graphic_events_control.modelMatrix,
            //    graphic_events_control.viewMatrix,
            //    gvariables_static.geom_transparency);


        }


    }
}
