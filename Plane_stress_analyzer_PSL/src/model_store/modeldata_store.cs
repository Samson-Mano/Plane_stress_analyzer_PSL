using OpenTK;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Plane_stress_analyzer_PSL.src.events_handler;
using Plane_stress_analyzer_PSL.src.model_store.geom_objects;
using Plane_stress_analyzer_PSL.src.model_store.fe_objects;

namespace Plane_stress_analyzer_PSL.src.model_store
{
    public class modeldata_store
    {

        // FE Data store
        public fedata_store fe_data;


        // Drawing bound data
        public Vector3 min_bounds = new Vector3(-1);
        public Vector3 max_bounds = new Vector3(1);
        public Vector3 geom_bounds = new Vector3(2);


        public selectrectangle_store selection_rectangle { get; }
        public selectcircle_store selection_circle { get; }

        // To control the drawing events
        public drawing_events graphic_events_control { get; private set; }

        // Update of mesh properties
        public bool isConstraintUpdateInProgress = false;
        public bool isLoadUpdateInProgress = false;
        public bool isMaterialUpdateInProgress = false;


        bool IsModelSet = false;

        public modeldata_store()
        {
            // Initialize the model
            fe_data = new fedata_store();

            // To control the drawing graphics
            graphic_events_control = new drawing_events(this);

            // Set the selection rectangle  & selection circle
            selection_rectangle = new selectrectangle_store();
            selection_circle = new selectcircle_store();

            // Set a default geometry bounds
            min_bounds = new Vector3(-1);
            max_bounds = new Vector3(1);
            geom_bounds = new Vector3(2);


            IsModelSet = false;
        }


        public void paint_model()
        {
          
        }


        public void update_openTK_uniforms()
        {


        }


        public void select_model_objects(Vector2 o_pt, Vector2 c_pt, bool isRightButton)
        {
            // Perform the select option
            if (isMaterialUpdateInProgress == true)
            {
                // meshdata.select_mesh_elements(o_pt, c_pt, isRightButton, graphic_events_control);

            }

            if (isLoadUpdateInProgress == true)
            {
                // Select the points for load update
                // meshdata.select_mesh_points(o_pt, c_pt, isRightButton, graphic_events_control);

            }

            if (isConstraintUpdateInProgress == true)
            {
                // Select the points for constraint update
                // meshdata.select_mesh_points(o_pt, c_pt, isRightButton, graphic_events_control);

            }

        }







    }
}
