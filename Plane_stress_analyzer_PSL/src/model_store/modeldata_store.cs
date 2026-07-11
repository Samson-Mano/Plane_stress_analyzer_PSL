// OpenTK library
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using Plane_stress_analyzer_PSL.src.events_handler;
using Plane_stress_analyzer_PSL.src.global_variables;
using Plane_stress_analyzer_PSL.src.model_store.fe_objects;
using Plane_stress_analyzer_PSL.src.model_store.geom_objects;
using Plane_stress_analyzer_PSL.src.model_store.rslt_objects;
using src.model_store.geom_objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plane_stress_analyzer_PSL.src.model_store
{
    public class modeldata_store
    {

        // FE Data store
        public fedata_store fe_data;

        // Result data store
        public rsltdata_store rslt_data;


        // Contour bar data for results visualization
        private contourlevelbar_store contour_bar_data;


        // Drawing bound data
        public Vector3 min_bounds = new Vector3(-1);
        public Vector3 max_bounds = new Vector3(1);
        public Vector3 geom_bounds = new Vector3(2);


        public selectrectangle_store selection_rectangle; // { get; }
        public selectcircle_store selection_circle; // { get; }

        // To control the drawing events
        public drawing_events graphic_events_control { get; private set; }

        // Update of mesh properties
        public bool isConstraintUpdateInProgress = false;
        public bool isLoadUpdateInProgress = false;
        public bool isMaterialUpdateInProgress = false;

        public bool IsModelSet = false;
        public bool IsResultSet = false;


        // Animation control data
        public System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();




        public modeldata_store()
        {

            // To control the drawing graphics
            graphic_events_control = new drawing_events(this);

            // Set a default geometry bounds
            min_bounds = new Vector3(-1);
            max_bounds = new Vector3(1);
            geom_bounds = new Vector3(2);


            IsModelSet = false;
            IsResultSet = false;
        }


        public void InitializeModelGeom()
        {
            // Set the selection rectangle  & selection circle
            selection_rectangle = new selectrectangle_store();
            selection_circle = new selectcircle_store();
 
            fe_data = new fedata_store();
            rslt_data = new rsltdata_store();


            contour_bar_data = new contourlevelbar_store();

            IsResultSet = false;

        }


        public void importFile(string filePath, int type)
        {
            List<Vector3> nodePtsList = new List<Vector3>();
            IsModelSet = false;
            IsResultSet = false;

            fe_data = new fedata_store();
            rslt_data = new rsltdata_store();

            if (type == 0)
            {
                // Import type is TXT file
                string fileContent = System.IO.File.ReadAllText(filePath);
                file_events.import_txt_mesh(fileContent, ref fe_data, ref nodePtsList, ref IsModelSet);
            }
            else if (type == 1)
            {
                // Import type is BIN file
                file_events.import_binary_mesh(filePath, ref fe_data, ref nodePtsList, ref IsModelSet);
            }


            if (IsModelSet == false)
                return;

            // Set the mesh boundaries
            Vector3 geometry_center = gvariables_static.FindGeometricCenter(nodePtsList);
            Tuple<Vector3, Vector3> geom_extremes = gvariables_static.FindMinMaxXY(nodePtsList);


            // Set the geometry bounds
            this.min_bounds = geom_extremes.Item1; // Minimum bound
            this.max_bounds = geom_extremes.Item2; // Maximum bound

            this.geom_bounds = max_bounds - min_bounds;

            // update the global static value
            gvariables_static.geom_size = this.geom_bounds.Length;


            fe_data.set_meshdrawing_data();

            // Initialize contour bar data
            contour_bar_data.InitializeContourLevelBarData(graphic_events_control.window_width, graphic_events_control.window_height);

            update_openTK_uniforms();

        }


        public void exportBINFile(string filePath)
        {
            if (!IsModelSet)
                return;

            // Export the bindary mesh
            file_events.export_binary_mesh(filePath, fe_data);

        }


        public void paint_model()
        {

            if (!IsModelSet)
                return;


            fe_data.paint_model();

            if (isMaterialUpdateInProgress == true || isLoadUpdateInProgress == true || isConstraintUpdateInProgress == true)
            {
                if (gvariables_static.is_RectangleSelection == true)
                {
                    // Paint the selection rectangle
                    selection_rectangle.draw_selection_rectangle();
                }
                else
                {
                    // Paint the selection circle
                    selection_circle.draw_selection_circle();
                }
            }

            if (IsResultSet == true)
            {
                // Paint the result mesh
                if (gvariables_static.is_paint_result_displacement == true ||
                    gvariables_static.is_paint_result_stressX == true ||
                    gvariables_static.is_paint_result_stressY == true ||
                    gvariables_static.is_paint_result_tauXY == true ||
                    gvariables_static.is_paint_result_vonMises == true ||
                    gvariables_static.is_paint_result_principalStress1 == true ||
                    gvariables_static.is_paint_result_principalStress2 == true ||
                    gvariables_static.is_paint_result_maxShearStress == true ||
                    gvariables_static.is_paint_result_PSL == true )
                {

                    rslt_data.paint_results();

                    contour_bar_data.draw_contour_bar();
                }
            }

        }


        public void switch_result_option()
        {

            if (!IsModelSet)
                return;
            if (IsResultSet == false)
                return;


            int option = gvariables_static.result_option;


            // Switch the result option
            switch (option)
            {
                case 1:
                    contour_bar_data.UpdateContourLevelBarPosition(graphic_events_control.window_width,
                        graphic_events_control.window_height, 0.0f, 100.0f, "Displacement");
                    break;
                case 2:
                    contour_bar_data.UpdateContourLevelBarPosition(graphic_events_control.window_width,
                        graphic_events_control.window_height, -100.0f, 100.0f, "Stress X");
                    break;
                case 3:
                    contour_bar_data.UpdateContourLevelBarPosition(graphic_events_control.window_width,
                        graphic_events_control.window_height, -200.0f, 200.0f, "Stress Y");
                    break;
                case 4:
                    contour_bar_data.UpdateContourLevelBarPosition(graphic_events_control.window_width,
                        graphic_events_control.window_height, -300.0f, 300.0f, "Shear Stress XY");
                    break;
                case 5:
                    contour_bar_data.UpdateContourLevelBarPosition(graphic_events_control.window_width,
                        graphic_events_control.window_height, -400.0f, 400.0f, "Von Mises Stress");
                    break;
                case 6:
                    contour_bar_data.UpdateContourLevelBarPosition(graphic_events_control.window_width,
                        graphic_events_control.window_height, -500.0f, 500.0f, "Principal Stress 1");
                    break;
                case 7:
                    contour_bar_data.UpdateContourLevelBarPosition(graphic_events_control.window_width,
                        graphic_events_control.window_height, -600.0f, 600.0f, "Principal Stress 2"); 
                    break;
                case 8:
                    contour_bar_data.UpdateContourLevelBarPosition(graphic_events_control.window_width,
                        graphic_events_control.window_height, -700.0f, 700.0f, "Max Shear Stress");
                    break;
                case 9:
                    contour_bar_data.UpdateContourLevelBarPosition(graphic_events_control.window_width,
                        graphic_events_control.window_height, -800.0f, 800.0f, "PSL"); 
                    break;


            }
        }


        public void update_contour_bar_position(int window_width, int window_height)
        {
            if (!IsModelSet)
                return;
            if (IsResultSet == false)
                return;

            switch_result_option();

        }



        public void update_openTK_uniforms()
        {

            if (!IsModelSet)
                return;

            fe_data.update_openTK_uniforms(graphic_events_control);
            rslt_data.update_openTK_uniforms(graphic_events_control);

 
        }


        public void select_model_objects(Vector2 o_pt, Vector2 c_pt, bool isRightButton)
        {
            if (!IsModelSet) return;

            // Perform the select option
            if (isMaterialUpdateInProgress == true)
            {
                fe_data.select_mesh(o_pt, c_pt, isRightButton, graphic_events_control);

            }

            if (isLoadUpdateInProgress == true)
            {
                // Select the points for load update
                fe_data.select_nodes(o_pt, c_pt, isRightButton, graphic_events_control);

            }

            if (isConstraintUpdateInProgress == true)
            {
                // Select the points for constraint update
                fe_data.select_nodes(o_pt, c_pt, isRightButton, graphic_events_control);

            }

        }

        public void start_animation()
        {
            // Start the animation
            stopwatch.Start();

        }


        public void pause_animation()
        {
            // Pause the animation
            stopwatch.Stop();

        }


        public void stop_animation()
        {
            // Reset the animation stopwatch and time step
            stopwatch.Reset();
            stopwatch.Stop();

            // Set the animation sine value to 1.0f
            rslt_data.update_animation(1.0f);

        }


        public void update_result_animation()
        {
            if (!IsModelSet || !IsResultSet)
                return;


            // Results are stored, animate the modal results
            double elapsedRealTime = stopwatch.Elapsed.TotalSeconds;


            if (gvariables_static.animate_play == true)
            {
                // Oscillation: -1 to 1
                float oscillation = (float)Math.Sin(2.0 * Math.PI * elapsedRealTime * gvariables_static.resp_animation_speed);

                // Convert to the range of 0 to 1
                oscillation = (oscillation + 1.0f) / 2.0f;

                rslt_data.update_animation(oscillation);

            }


            //
        }


    }
}
