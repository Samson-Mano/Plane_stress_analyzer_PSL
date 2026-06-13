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

    public class nodeload_data
    {
        public int load_set_id { get; set; }

        public List<Vector2> load_node_pts { get; set; }

        public List<int> load_node_ids { get; set; }

        public double load_amplitude { get; set; }

        public double load_angle { get; set; }

    }



    public class nodeload_list_store
    {
        public Dictionary<int, nodeload_data> loadMap = new Dictionary<int, nodeload_data>();
        public int load_count = 0;

        private List<int> all_loadset_ids = new List<int>();

        public nodeload_list_store()
        {
            // (Re)Initialize the data
            loadMap = new Dictionary<int, nodeload_data>();
            load_count = 0;

        }


        public void add_loads(List<int> load_node_ids, List<Vector2> load_node_pts, 
            double t_load_amplitude, double t_load_angle)
        {
            // Get an unique load set id
            int unique_loadset_id = gvariables_static.get_unique_id(all_loadset_ids);

            // Make a copy of the list
            List<int> idsCopy = new List<int>(load_node_ids);
            List<Vector2> nodePtsCopy = new List<Vector2>(load_node_pts);


            // Add the Load to the list
            nodeload_data temp_load = new nodeload_data
            {
                load_set_id = unique_loadset_id,
                load_node_ids = idsCopy,
                load_node_pts = nodePtsCopy,
                load_amplitude = t_load_amplitude,
                load_angle = t_load_angle
            };


            loadMap[unique_loadset_id] = temp_load;
            load_count++;

            // Add the load set id to list to track the unique load set id
            all_loadset_ids.Add(unique_loadset_id);

        }




        public void delete_nodeload(int load_set_id)
        {
            // Remove the load set ID from all_loadset_ids
            all_loadset_ids.Remove(load_set_id);

            // Set the load data visualization
            set_load_visualization(load_set_id, false);

            // Remove the load data based on the key (load set id)
            loadMap.Remove(load_set_id);

            // adjust the load data count
            load_count--;
        }


        private void set_load_visualization(int load_set_id, bool isAdd)
        {
            // Get the load
            nodeload_data ndloads = loadMap[load_set_id];

            if (isAdd == true)
            {
                // Add visualization for this load id


            }
            else
            {
                // Delete visualization for this load id


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
            // node load count check
            if (load_count == 0)
                return;

            // Paint the Load label
            gvariables_static.LineWidth = 3.0f;
            //  ndcnst_meshdata.paint_static_mesh_lines();
            gvariables_static.LineWidth = 1.0f;

        }



        public void update_openTK_uniforms(bool set_modelmatrix, bool set_viewmatrix, bool set_transparency,
            drawing_events graphic_events_control)
        {
            if (load_count == 0)
                return;


            //ndcnst_meshdata.update_openTK_uniforms(graphic_events_control.projectionMatrix,
            //    graphic_events_control.modelMatrix,
            //    graphic_events_control.viewMatrix,
            //    gvariables_static.geom_transparency);


        }



    }
}
