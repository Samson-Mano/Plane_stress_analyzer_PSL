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

namespace Plane_stress_analyzer_PSL.other_windows
{
    public partial class matprop_frm : Form
    {
        private modeldata_store modeldata;

        public matprop_frm(ref modeldata_store modeldata)
        {
            InitializeComponent();

            this.modeldata = modeldata;
        }


        private void matprop_frm_Load(object sender, EventArgs e)
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


    }
}
