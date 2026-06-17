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


        public HashSet<int> selected_tri_ids { get; } = new HashSet<int>();
        public HashSet<int> selected_quad_ids { get; } = new HashSet<int>();
        public HashSet<int> selected_node_ids { get; } = new HashSet<int>();


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

            meshdrawingdata.paint_selected_mesh();

            // Paint the constraints
            fe_constraints.paint_node_constraint();

            // Paint the loads
            fe_loads.paint_node_load();

        }



        public void update_openTK_uniforms(drawing_events graphic_events_control)
        {
            if (!IsMeshDrawingDataSet)
                return;

            meshdrawingdata.update_openTK_uniforms(graphic_events_control);
            fe_constraints.update_openTK_uniforms(graphic_events_control);
            fe_loads.update_openTK_uniforms(graphic_events_control);

        }


        public void select_nodes(Vector2 corner_pt1, Vector2 corner_pt2, bool isRightButton, drawing_events graphic_events_control)
        {
            // Select the nodes for load or constraint update
            List<int> selected_node_ids = new List<int>();

            // Loop through all node in nodeMap
            foreach (node_store nd in fe_nodes.nodeMap.Values)
            {
                
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
                // Add all nodes at once
                int initialCount = this.selected_node_ids.Count;
                this.selected_node_ids.UnionWith(selected_node_ids);
                is_selection_changed = this.selected_node_ids.Count != initialCount;
            }
            else
            {
                // Remove from the selected node list
                // Remove all nodes at once
                int initialCount = this.selected_node_ids.Count;
                this.selected_node_ids.ExceptWith(selected_node_ids);
                is_selection_changed = this.selected_node_ids.Count != initialCount;
            }


            if (is_selection_changed == true)
            {
                // Add the selected nodes
                meshdrawingdata.add_selected_points(this.selected_node_ids.ToList());
            }
            //
        }


        public void clear_selected_nodes()
        {
            this.selected_node_ids.Clear();
            meshdrawingdata.clear_selected_points();

        }



        public void select_mesh(Vector2 corner_pt1, Vector2 corner_pt2, bool isRightButton, drawing_events graphic_events_control)
        {
            // Select the mesh for material properties update
            List<int> selected_tri_ids = new List<int>();
            List<int> selected_quad_ids = new List<int>();

            // Pre-compute MVP matrix
            Matrix4 mvp = graphic_events_control.projectionMatrix *
                          graphic_events_control.viewMatrix *
                          graphic_events_control.modelMatrix;

            // Select tri element for mesh
            foreach (elementtri_store tri in fe_tris.elementtriMap.Values)
            {
                Vector4 node_pt1_s = mvp *
                    new Vector4((float)fe_nodes.nodeMap[tri.nodeid1].node_pt_x_coord,
                    (float)fe_nodes.nodeMap[tri.nodeid1].node_pt_y_coord, 0.0f, 1.0f);

                Vector4 node_pt2_s = mvp *
                    new Vector4((float)fe_nodes.nodeMap[tri.nodeid2].node_pt_x_coord,
                    (float)fe_nodes.nodeMap[tri.nodeid2].node_pt_y_coord, 0.0f, 1.0f);

                Vector4 node_pt3_s = mvp *
                    new Vector4((float)fe_nodes.nodeMap[tri.nodeid3].node_pt_x_coord,
                    (float)fe_nodes.nodeMap[tri.nodeid3].node_pt_y_coord, 0.0f, 1.0f);

                Vector2 node_pt1 = new Vector2(node_pt1_s.X, node_pt1_s.Y);
                Vector2 node_pt2 = new Vector2(node_pt2_s.X, node_pt2_s.Y);
                Vector2 node_pt3 = new Vector2(node_pt3_s.X, node_pt3_s.Y);

                //Vector2 edge1_midpt = (node_pt1 + node_pt2) * 0.5f;
                //Vector2 edge2_midpt = (node_pt2 + node_pt3) * 0.5f;
                //Vector2 edge3_midpt = (node_pt3 + node_pt1) * 0.5f;

                //Vector2 tri_midpt = (node_pt1 + node_pt2 + node_pt3) * (1.0f / 3.0f);

                //if(gvariables_static.isPointSelected(corner_pt1,corner_pt2,node_pt1) == true ||
                //    gvariables_static.isPointSelected(corner_pt1, corner_pt2, node_pt2) == true ||
                //    gvariables_static.isPointSelected(corner_pt1, corner_pt2, node_pt3) == true ||
                //    gvariables_static.isPointSelected(corner_pt1, corner_pt2, edge1_midpt) == true ||
                //    gvariables_static.isPointSelected(corner_pt1, corner_pt2, edge2_midpt) == true ||
                //    gvariables_static.isPointSelected(corner_pt1, corner_pt2, edge3_midpt) == true ||
                //    gvariables_static.isPointSelected(corner_pt1, corner_pt2, tri_midpt) == true)
                //{
                //    selected_tri_ids.Add(tri.tri_id);
                //}


                if (gvariables_static.isEdgeSelected(corner_pt1, corner_pt2,node_pt1,node_pt2) == true ||
                    gvariables_static.isEdgeSelected(corner_pt1, corner_pt2,node_pt2, node_pt3) == true ||
                    gvariables_static.isEdgeSelected(corner_pt1, corner_pt2,node_pt3, node_pt1) == true)
                {
                    selected_tri_ids.Add(tri.tri_id);
                }




            }


            // Select quad element for mesh
            foreach (elementquad_store quad in fe_quads.elementquadMap.Values)
            {
                Vector4 node_pt1_s = mvp *
                    new Vector4((float)fe_nodes.nodeMap[quad.nodeid1].node_pt_x_coord,
                    (float)fe_nodes.nodeMap[quad.nodeid1].node_pt_y_coord, 0.0f, 1.0f);

                Vector4 node_pt2_s = mvp *
                    new Vector4((float)fe_nodes.nodeMap[quad.nodeid2].node_pt_x_coord,
                    (float)fe_nodes.nodeMap[quad.nodeid2].node_pt_y_coord, 0.0f, 1.0f);

                Vector4 node_pt3_s = mvp *
                    new Vector4((float)fe_nodes.nodeMap[quad.nodeid3].node_pt_x_coord,
                    (float)fe_nodes.nodeMap[quad.nodeid3].node_pt_y_coord, 0.0f, 1.0f);

                Vector4 node_pt4_s = mvp *
                    new Vector4((float)fe_nodes.nodeMap[quad.nodeid4].node_pt_x_coord,
                    (float)fe_nodes.nodeMap[quad.nodeid4].node_pt_y_coord, 0.0f, 1.0f);

                Vector2 node_pt1 = new Vector2(node_pt1_s.X, node_pt1_s.Y);
                Vector2 node_pt2 = new Vector2(node_pt2_s.X, node_pt2_s.Y);
                Vector2 node_pt3 = new Vector2(node_pt3_s.X, node_pt3_s.Y);
                Vector2 node_pt4 = new Vector2(node_pt3_s.X, node_pt3_s.Y);

                //Vector2 edge1_midpt = (node_pt1 + node_pt2) * 0.5f;
                //Vector2 edge2_midpt = (node_pt2 + node_pt3) * 0.5f;
                //Vector2 edge3_midpt = (node_pt3 + node_pt4) * 0.5f;
                //Vector2 edge4_midpt = (node_pt4 + node_pt1) * 0.5f;

                //Vector2 quad_midpt = (node_pt1 + node_pt2 + node_pt3 + node_pt4) * (1.0f / 4.0f);

                //if (gvariables_static.isPointSelected(corner_pt1, corner_pt2, node_pt1) == true ||
                //    gvariables_static.isPointSelected(corner_pt1, corner_pt2, node_pt2) == true ||
                //    gvariables_static.isPointSelected(corner_pt1, corner_pt2, node_pt3) == true ||
                //    gvariables_static.isPointSelected(corner_pt1, corner_pt2, node_pt4) == true ||
                //    gvariables_static.isPointSelected(corner_pt1, corner_pt2, edge1_midpt) == true ||
                //    gvariables_static.isPointSelected(corner_pt1, corner_pt2, edge2_midpt) == true ||
                //    gvariables_static.isPointSelected(corner_pt1, corner_pt2, edge3_midpt) == true ||
                //    gvariables_static.isPointSelected(corner_pt1, corner_pt2, edge4_midpt) == true ||
                //    gvariables_static.isPointSelected(corner_pt1, corner_pt2, quad_midpt) == true)
                //{
                //    selected_quad_ids.Add(quad.quad_id);
                //}


                if (gvariables_static.isEdgeSelected(corner_pt1, corner_pt2,node_pt1, node_pt2) == true ||
                        gvariables_static.isEdgeSelected(corner_pt1, corner_pt2, node_pt2, node_pt3) == true ||
                        gvariables_static.isEdgeSelected(corner_pt1, corner_pt2, node_pt3, node_pt4) == true ||
                        gvariables_static.isEdgeSelected(corner_pt1, corner_pt2, node_pt4, node_pt1) == true)
                {
                    selected_quad_ids.Add(quad.quad_id);
                }

            }


            if((selected_tri_ids.Count + selected_quad_ids.Count) > 0)
            {
                add_selected_mesh(selected_tri_ids, selected_quad_ids, isRightButton);
            }

        }


        private void add_selected_mesh(List<int> selected_tri_ids, List<int> selected_quad_ids, bool IsRemove)
        {
            bool is_selection_changed = false;

            if (IsRemove == false)
            {
                // Add to the selected tri or selected quad list
                // Add all tri and all quad at once
                int initialCount = this.selected_tri_ids.Count + this.selected_quad_ids.Count;
                this.selected_tri_ids.UnionWith(selected_tri_ids);
                this.selected_quad_ids.UnionWith(selected_quad_ids);

                is_selection_changed = (this.selected_tri_ids.Count + this.selected_quad_ids.Count) != initialCount;

            }
            else
            {
                // Remove from the selected node list
                // Remove all tri and all quad at once
                int initialCount = this.selected_tri_ids.Count + this.selected_quad_ids.Count;
                this.selected_tri_ids.ExceptWith(selected_tri_ids);
                this.selected_quad_ids.ExceptWith(selected_quad_ids);

                is_selection_changed = (this.selected_tri_ids.Count + this.selected_quad_ids.Count) != initialCount;

            }


            if (is_selection_changed == true)
            {
                // Add the selected meshes
                meshdrawingdata.add_selected_mesh(this.selected_tri_ids.ToList(), this.selected_quad_ids.ToList());
            }
            //
        }


        public void clear_selected_mesh()
        {
            this.selected_tri_ids.Clear();
            this.selected_quad_ids.Clear();

            meshdrawingdata.clear_selected_mesh();

        }




    }
}
