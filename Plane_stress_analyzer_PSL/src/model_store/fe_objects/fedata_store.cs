using OpenTK;
using Plane_stress_analyzer_PSL.src.events_handler;
using Plane_stress_analyzer_PSL.src.global_variables;
using Plane_stress_analyzer_PSL.src.model_store.geom_objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plane_stress_analyzer_PSL.src.model_store.fe_objects
{

    public class material_data
    {
        public int material_id = 0;
        public string material_name = "";

        public double youngs_modulus = 0.0; // E
        public double poissons_ratio = 0.0; // G
        public double material_density = 0.0; // Rho

        public int number_of_elements_appliedto = 0;

    }


    public class fedata_store
    {
        public int p_order = 4; // higher order p - refinement

        public node_list_store fe_nodes;
        public elementtri_list_store fe_tris;
        public elementquad_list_store fe_quads;

        public nodecnst_list_store fe_constraints;
        public nodeload_list_store fe_loads;

        public Dictionary<int, material_data> fe_materials;
        public List<int> materialids;
        // public label_list_store materiallabels;

        // Drawing data
        private meshdata_store meshdrawingdata;
        private bool IsMeshDrawingDataSet = false;


        public List<int> selected_tri_ids { get; } = new List<int>();
        public List<int> selected_quad_ids { get; } = new List<int>();
        public List<int> selected_node_ids { get; } = new List<int>();


        public fedata_store()
        {
            // (Re)Initialize the data
            p_order = 4;

            fe_nodes = new node_list_store();
            fe_tris = new elementtri_list_store();
            fe_quads = new elementquad_list_store();

            fe_constraints = new nodecnst_list_store();
            fe_loads = new nodeload_list_store();

            fe_materials = new Dictionary<int, material_data>();
            materialids = new List<int>();

            IsMeshDrawingDataSet = false;
        }

        public void set_meshdrawing_data()
        {
            meshdrawingdata = new meshdata_store();

            // Add the mesh points
            foreach (var nd_m in fe_nodes.nodeMap)
            {
                node_store nd = nd_m.Value;

                meshdrawingdata.add_point(nd.node_id, (float)nd.node_pt_x_coord, (float)nd.node_pt_y_coord);

            }

            // Add the mesh tris
            foreach (var tri_m in fe_tris.elementtriMap)
            {
                elementtri_store tri = tri_m.Value;

                meshdrawingdata.add_tri(tri.tri_id, tri.nodeid1, tri.nodeid2, tri.nodeid3, tri.material_id);

            }

            // Add the mesh quads
            foreach (var quad_m in fe_quads.elementquadMap)
            {
                elementquad_store quad = quad_m.Value;

                meshdrawingdata.add_quad(quad.quad_id, quad.nodeid1, quad.nodeid2, quad.nodeid3, quad.nodeid4, quad.material_id);

            }

            // Create the mesh boundaries
            meshdrawingdata.create_wireframe();

            // Create the mesh buffer
            meshdrawingdata.create_buffer_data();


            IsMeshDrawingDataSet = true;
        }


        public void paint_model()
        {
            if (!IsMeshDrawingDataSet)
                return;

            meshdrawingdata.paint_mesh();
            meshdrawingdata.paint_mesh_wireframe();
            meshdrawingdata.paint_mesh_points();

            meshdrawingdata.paint_selected_mesh_points();

        }



        public void update_openTK_uniforms(drawing_events graphic_events_control)
        {
            if (!IsMeshDrawingDataSet)
                return;

            meshdrawingdata.update_openTK_uniforms(graphic_events_control);


        }


        public void select_nodes(Vector2 corner_pt1, Vector2 corner_pt2, bool isRightButton, drawing_events graphic_events_control)
        {
            // Select the nodes for load or constraint update
            List<int> selected_node_ids = new List<int>();

            // Loop through all node in nodeMap
            foreach (var nd_m in fe_nodes.nodeMap)
            {
                node_store nd = nd_m.Value;

                //______________________________
                Vector4 node_pt = graphic_events_control.projectionMatrix * graphic_events_control.viewMatrix
                    * graphic_events_control.modelMatrix * new Vector4((float)nd.node_pt_x_coord, 
                    (float)nd.node_pt_y_coord, 0.0f, 1.0f);


                // Check whether the point inside a rectangle
                if (gvariables_static.isPointSelected(corner_pt1, corner_pt2, new Vector2(node_pt.X, node_pt.Y)) == true)
                {
                    selected_node_ids.Add(nd.node_id);

                }

            }

            if (selected_node_ids.Count > 0)
            {
                add_selected_nodes(selected_node_ids, isRightButton);
            }

        }


        private void add_selected_nodes(List<int> selected_node_ids, bool IsRemove)
        {
            bool is_selection_changed = false;

            if (IsRemove == false)
            {
                // Add to the selected node list
                foreach (int nd_id in selected_node_ids)
                {
                    // Check whether the node is already in the list
                    if (!this.selected_node_ids.Contains(nd_id))
                    {
                        // Add to selected nodes
                        this.selected_node_ids.Add(nd_id);

                        // Selection changed flag
                        is_selection_changed = true;
                    }

                }
            }
            else
            {
                // Remove from the selected node list
                foreach (int nd_id in selected_node_ids)
                {
                    // Remove the node which is found in the list
                    this.selected_node_ids.Remove(nd_id);

                    // Selection changed flag
                    is_selection_changed = true;
                }

            }


            if (is_selection_changed == true)
            {
                // Add the selected nodes
                meshdrawingdata.add_selected_points(this.selected_node_ids);
            }
            //
        }


        public void clear_selected_nodes()
        {
            this.selected_node_ids.Clear();
            meshdrawingdata.clear_selected_points();

        }


    }
}
