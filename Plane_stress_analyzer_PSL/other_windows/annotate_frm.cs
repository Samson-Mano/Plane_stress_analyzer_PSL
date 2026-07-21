using Plane_stress_analyzer_PSL;
using Plane_stress_analyzer_PSL.src.global_variables;
using Plane_stress_analyzer_PSL.src.model_store;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace other_windows
{
    public partial class annotate_frm : Form
    {
        private modeldata_store modeldata;

        public annotate_frm(ref modeldata_store modeldata)
        {
            InitializeComponent();

            this.modeldata = modeldata;

        }

        private void annotate_frm_Load(object sender, EventArgs e)
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



        private void button_clearall_Click(object sender, EventArgs e)
        {
            dataGridView_ResultNodeList.Rows.Clear();
            modeldata.rslt_data.clear_selected_result_points();

        }


        public void updateSelectedResultPointsDataGridView()
        {
            // refresh the Result Node list data grid view
            dataGridView_ResultNodeList.Rows.Clear();

            foreach (string point_str in modeldata.rslt_data.get_selected_result_points_string())
            {
                string[] row = point_str.Split(',');

                dataGridView_ResultNodeList.Rows.Add(row);
            }

        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void annotate_frm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Control the flag
           // modeldata.rslt_data.clear_selected_result_points();
            modeldata.isAnnotateResultInProgress = false;

            // Call the main form
            if (this.Owner is main_frm mainForm)
            {
                mainForm.callFrom_annotate_frm();
            }

        }
    }
}
