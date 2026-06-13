using OpenTK;
using Plane_stress_analyzer_PSL.src.global_variables;
using Plane_stress_analyzer_PSL.src.model_store;
using Plane_stress_analyzer_PSL.src.model_store.fe_objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plane_stress_analyzer_PSL.other_windows
{
    public partial class load_frm : Form
    {
        private modeldata_store modeldata;


        public load_frm(ref modeldata_store modeldata)
        {
            InitializeComponent();

            this.modeldata = modeldata;

            textBox_loadamplitude.Text = "10.0";
            textBox_loadangle.Text = "0";
        }

        private void load_frm_Load(object sender, EventArgs e)
        {
            // Initialize selection state from global variable
            SetSelectionMode(gvariables_static.is_RectangleSelection);
        }



        private void rectangleSelectionToolStripMenuItem_Click(object sender, EventArgs e) => SetSelectionMode(true);

        private void circleSelectionToolStripMenuItem_Click(object sender, EventArgs e) => SetSelectionMode(false);


        private void SetSelectionMode(bool isRectangle)
        {

            gvariables_static.is_RectangleSelection = isRectangle;

            rectangleSelectionToolStripMenuItem.Checked = isRectangle;
            circleSelectionToolStripMenuItem.Checked = !isRectangle;


            rectangleSelectionToolStripMenuItem.BackColor = isRectangle ? Color.LightBlue : SystemColors.Control;
            circleSelectionToolStripMenuItem.BackColor = !isRectangle ? Color.LightBlue : SystemColors.Control;

        }

        private void button_applyload_Click(object sender, EventArgs e)
        {
            if (modeldata.fe_data.selected_node_ids.Count == 0)
                return;

            // Safely Retrieve the Load Amplitude
            string loadamplstring = textBox_loadamplitude.Text;

            if (!double.TryParse(loadamplstring, out double load_ampl))
            {
                // MessageBox.Show("Invalid load Amplitude.");
                return;
            }

            // Safely Retrieve the Load Angle
            string loadanglestring = textBox_loadangle.Text;

            if (!double.TryParse(loadanglestring, out double load_angle))
            {
                // MessageBox.Show("Invalid load Angle.");
                return;
            }

            if (load_angle < 0.0 && load_angle > 360.0)
            {
                return;
            }


            // Get the node locations
            List<Vector2> load_node_pts = new List<Vector2>();

            foreach (int nd_id in modeldata.fe_data.selected_node_ids)
            {
                node_store nd = modeldata.fe_data.fe_nodes.nodeMap[nd_id];

                load_node_pts.Add(new Vector2((float)nd.node_pt_x_coord,
                    (float)nd.node_pt_y_coord));
            }


            // Add the load
            modeldata.fe_data.fe_loads.add_loads(modeldata.fe_data.selected_node_ids,
                load_node_pts, load_ampl, load_angle);


            // Clear the selected node ids
            modeldata.fe_data.clear_selected_nodes();

            update_selected_node_list();

            // Call the main form
            if (this.Owner is main_frm mainForm)
            {
                mainForm.CallFrom_load_frm();
            }

            // Update the data grid view
            update_dataGridView();

        }

        private void button_deleteload_Click(object sender, EventArgs e)
        {
            if (dataGridView_LoadList.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView_LoadList.SelectedRows[0];

                // Safely Retrieve the Load Set ID
                string idString = selectedRow.Cells["Column1_loadsetid"].Value?.ToString();

                if (!int.TryParse(idString, out int loadset_id))
                {
                    // MessageBox.Show("Invalid load set ID.");
                    return;
                }


                // Delete the selected constraint
                modeldata.fe_data.fe_loads.delete_nodeload(loadset_id);

                update_dataGridView();

                // Clear the selected node ids
                modeldata.fe_data.clear_selected_nodes();

                update_selected_node_list();

                // Call the main form
                if (this.Owner is main_frm mainForm)
                {
                    mainForm.CallFrom_load_frm();
                }

            }
        }


        public void update_dataGridView()
        {

            // refresh the Load list data grid view
            dataGridView_LoadList.Rows.Clear();


            foreach (var load_m in modeldata.fe_data.fe_loads.loadMap)
            {
                nodeload_data load = load_m.Value;

                // Convert node IDs list to a short string, e.g. "1, 2, 3 ..."
                string nodeIdsPreview;
                int previewCount = 15; // how many IDs to show
                if (load.load_node_ids.Count > previewCount)
                {
                    nodeIdsPreview = string.Join(", ", load.load_node_ids.Take(previewCount)) + " ...";
                }
                else
                {
                    nodeIdsPreview = string.Join(", ", load.load_node_ids);
                }


                dataGridView_LoadList.Rows.Add(
                    load.load_set_id,
                    nodeIdsPreview,   // show some of load nodes as string here
                    load.load_amplitude,
                    load.load_angle
                    );

            }

            if (dataGridView_LoadList.Rows.Count > 0)
            {
                // Move the index to the last index
                int lastIndex = dataGridView_LoadList.Rows.Count - 1;
                dataGridView_LoadList.ClearSelection();
                dataGridView_LoadList.Rows[lastIndex].Selected = true;

            }

            dataGridView_LoadList.Invalidate();

        }


        public void update_selected_node_list()
        {
            // Clear the text box
            textBox_selectednodes.Clear();

            List<int> all_selected_nodes = new List<int>();

            all_selected_nodes.AddRange(modeldata.fe_data.selected_node_ids);

            textBox_selectednodes.Text = string.Join(", ", all_selected_nodes);

            textBox_selectednodes.Invalidate();

        }

        private void load_frm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Control the flag
            modeldata.isLoadUpdateInProgress = false;

        }

    }
}
