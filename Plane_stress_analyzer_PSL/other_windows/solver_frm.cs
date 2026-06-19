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
    public partial class solver_frm : Form
    {

        private modeldata_store modeldata;

        public solver_frm(ref modeldata_store modeldata)
        {
            InitializeComponent();

            this.modeldata = modeldata;
        }

        private void solver_frm_Load(object sender, EventArgs e)
        {
            comboBox_solvertype.SelectedIndex = Properties.Settings.Default.Sett_solver_type;
            comboBox_HRefinement.SelectedIndex = Properties.Settings.Default.Sett_Hrefine;
            comboBox_polynomialrefinement.SelectedIndex = Properties.Settings.Default.Sett_Prefine;

        }

        private void button_solve_Click(object sender, EventArgs e)
        {
            // Set the default



        }

        private void save_defaults()
        {
            Properties.Settings.Default.Sett_solver_type = comboBox_solvertype.SelectedIndex;
            Properties.Settings.Default.Sett_Hrefine = comboBox_HRefinement.SelectedIndex;
            Properties.Settings.Default.Sett_Prefine = comboBox_polynomialrefinement.SelectedIndex;

            Properties.Settings.Default.Save();
        }


    }
}
