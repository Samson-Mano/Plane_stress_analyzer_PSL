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
    public partial class constraint_frm : Form
    {
        private modeldata_store modeldata;

        public constraint_frm(ref modeldata_store modeldata)
        {
            InitializeComponent();

            this.modeldata = modeldata;

            comboBox_constrainttype.SelectedIndex = 0;
            textBox_constraintangle.Text = "0";
        }

        private void constraint_frm_Load(object sender, EventArgs e)
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

        private void button_applyconstraint_Click(object sender, EventArgs e)
        {

            if (modeldata.fe_data.selected_node_ids.Count == 0)
                return;



            // Safely Retrieve the Constraint Angle
            string constrantanglestring = textBox_constraintangle.Text;

            if (!double.TryParse(constrantanglestring, out double constraint_angle))
            {
                // MessageBox.Show("Invalid constraint Angle.");
                return;
            }

            if(constraint_angle < 0.0 && constraint_angle > 360.0)
            {
                return;
            }


            // Get the node locations
            List<Vector2> constraint_node_pts = new List<Vector2>();

            foreach (int nd_id in modeldata.fe_data.selected_node_ids)
            {
                node_store nd = modeldata.fe_data.fe_nodes.nodeMap[nd_id];

                constraint_node_pts.Add(new Vector2((float)nd.node_pt_x_coord,
                    (float)nd.node_pt_y_coord));
            }


            // Add the constraint
            modeldata.fe_data.fe_constraints.add_nodeconstraint(modeldata.fe_data.selected_node_ids,
                constraint_node_pts, comboBox_constrainttype.SelectedIndex, constraint_angle);


            // Clear the selected node ids
            modeldata.fe_data.clear_selected_nodes();

            update_selected_node_list();

            // Call the main form
            if (this.Owner is main_frm mainForm)
            {
                mainForm.CallFrom_constraint_frm();
            }

            // Update the data grid view
            update_dataGridView();

        }

        private void button_deleteconstraint_Click(object sender, EventArgs e)
        {
            if (dataGridView_ConstraintList.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView_ConstraintList.SelectedRows[0];

                // Safely Retrieve the Constraint ID
                string idString = selectedRow.Cells["Column1_constraintid"].Value?.ToString();

                if (!int.TryParse(idString, out int constraint_id))
                {
                    // MessageBox.Show("Invalid constraint ID.");
                    return;
                }


                // Delete the selected constraint
                modeldata.fe_data.fe_constraints.delete_nodeconstraint(constraint_id);

                update_dataGridView();

                // Clear the selected node ids
                modeldata.fe_data.clear_selected_nodes();

                update_selected_node_list();

                // Call the main form
                if (this.Owner is main_frm mainForm)
                {
                    mainForm.CallFrom_constraint_frm();
                }

            }
        }


        public void update_dataGridView()
        {

            // refresh the Constraint list data grid view
            dataGridView_ConstraintList.Rows.Clear();


            foreach (var cnst_m in modeldata.fe_data.fe_constraints.cnstMap)
            {
                nodecnst_data cnst = cnst_m.Value;

                // Convert node IDs list to a short string, e.g. "1, 2, 3 ..."
                string nodeIdsPreview;
                int previewCount = 15; // how many IDs to show
                if (cnst.constraint_node_ids.Count > previewCount)
                {
                    nodeIdsPreview = string.Join(", ", cnst.constraint_node_ids.Take(previewCount)) + " ...";
                }
                else
                {
                    nodeIdsPreview = string.Join(", ", cnst.constraint_node_ids);
                }

                string str_constraintType = "";
                if(cnst.constraint_type == 0)
                {
                    str_constraintType = "Pinned";
                }    
                else if (cnst.constraint_type == 1)
                {
                    str_constraintType = "Roller";
                }

                dataGridView_ConstraintList.Rows.Add(
                    cnst.cnst_set_id,
                    nodeIdsPreview,   // show some of constraint nodes as string here
                    str_constraintType,
                    cnst.constraint_angle
                    );

            }

            if (dataGridView_ConstraintList.Rows.Count > 0)
            {
                // Move the index to the last index
                int lastIndex = dataGridView_ConstraintList.Rows.Count - 1;
                dataGridView_ConstraintList.ClearSelection();
                dataGridView_ConstraintList.Rows[lastIndex].Selected = true;

            }

            dataGridView_ConstraintList.Invalidate();

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

        private void constraint_frm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Control the flag
            modeldata.isConstraintUpdateInProgress = false;

        }
    }
}
