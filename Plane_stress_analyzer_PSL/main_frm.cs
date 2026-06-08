using Plane_stress_analyzer_PSL.other_windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plane_stress_analyzer_PSL
{
    public partial class main_frm : Form
    {
        // // main finite element data store
        // public fedata_store fedata;

        // Zoom To Fit 
        private Timer zoomToFitTimer;

        // Refreh and FPS Tracking variables
        private Timer refreshStatusResetTimer;
        private Stopwatch fpsStopwatch = new Stopwatch();


        // Forms
        private option_frm option_Form;
        private matprop_frm matprop_Form;
        private load_frm load_Form;
        private constraint_frm constraint_Form;
        private solver_frm solver_Form;


        public main_frm()
        {
            InitializeComponent();

            // Initialize the timer
            zoomToFitTimer = new Timer();
            zoomToFitTimer.Interval = 10; // ~60 FPS refresh (16 ms)
            zoomToFitTimer.Tick += ZoomToFitTimer_Tick;

            // Render timer
            Application.Idle += OnApplicationIdle;

        }






    }
}
