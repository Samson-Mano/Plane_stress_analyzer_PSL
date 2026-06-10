// OpenTK library
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using Plane_stress_analyzer_PSL.other_windows;
using Plane_stress_analyzer_PSL.src.global_variables;
using Plane_stress_analyzer_PSL.src.model_store;
using Plane_stress_analyzer_PSL.src.model_store.fe_objects;
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
        // main model data store
        public modeldata_store modeldata;

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


            modeldata = new modeldata_store();

            zoomToFitTimer = new Timer();
            zoomToFitTimer.Interval = 10;
            zoomToFitTimer.Tick += ZoomToFitTimer_Tick;

            Application.Idle += OnApplicationIdle;

        }


        private void main_frm_Load(object sender, EventArgs e)
        {
            // Initialize the GLControl in the Load event
            // Fill the gcontrol panel
            glControl_main_panel.BorderStyle = BorderStyle.Fixed3D;
            glControl_main_panel.Dock = DockStyle.Fill;

            // Create the main font atlas
            gvariables_static.main_font.CreateAtlas();


        }


        #region "glControl Main Panel Events"

        private void glControl_main_panel_Load(object sender, EventArgs e)
        {
            glControl_main_panel.MakeCurrent();


            // Paint the background
            Color clr_bg = gvariables_static.glcontrol_background_color;
            GL.ClearColor(((float)clr_bg.R / 255.0f),
                ((float)clr_bg.G / 255.0f),
                ((float)clr_bg.B / 255.0f),
                ((float)clr_bg.A / 255.0f));


            fpsStopwatch.Start();

            // Refresh the controller (doesnt do much.. nothing to draw)
            glControl_main_panel.Invalidate();

        }


        private void glControl_main_panel_Paint(object sender, PaintEventArgs e)
        {
            // Paint the drawing area (glControl_main)
            // Tell OpenGL to use MyGLControl
            glControl_main_panel.MakeCurrent();

            // GL.Enable(EnableCap.Multisample);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(0, BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            // Clear the background
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            modeldata.paint_model();

            // OpenTK windows are what's known as "double-buffered". In essence, the window manages two buffers.
            // One is rendered to while the other is currently displayed by the window.
            // This avoids screen tearing, a visual artifact that can happen if the buffer is modified while being displayed.
            // After drawing, call this function to swap the buffers. If you don't, it won't display what you've rendered.
            glControl_main_panel.SwapBuffers();

            // Update the zoom value
            double zm_val = modeldata.graphic_events_control.zoom_val;
            toolStripStatusLabel_zoom_value.Text = "Zoom: " + (gvariables_static.RoundOff((int)(zm_val * 100))).ToString() + "%";

            // Update FPS every second
            if (fpsStopwatch.ElapsedMilliseconds >= 1000)
            {
                fpsStopwatch.Restart();

                // SetRefreshStatus(true); // Update status bar
            }

        }

        private void glControl_main_panel_SizeChanged(object sender, EventArgs e)
        {
            // Note: SizeChanged can fire before the OpenGL context exists (e.g., during form initialization, Load etc).
            if (glControl_main_panel == null || modeldata == null)
                return;

            // Update the size of the drawing area
            modeldata.graphic_events_control.update_drawing_area_size(glControl_main_panel.Width,
                glControl_main_panel.Height);

            toolStripStatusLabel_zoom_value.Text = "Zoom: " + (gvariables_static.RoundOff((int)(1.0f * 100))).ToString() + "%";

            // Refresh the painting area
            glControl_main_panel.Invalidate();
        }

        private void glControl_main_panel_MouseEnter(object sender, EventArgs e)
        {
            // set the focus to enable zoom/ pan & zoom to fit
            glControl_main_panel.Focus();

        }

        private void glControl_main_panel_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Left button down
                modeldata.graphic_events_control.handleMouseLeftButtonClick(true, e.X, e.Y);

            }
            else if (e.Button == MouseButtons.Right)
            {
                // Right button down
                modeldata.graphic_events_control.handleMouseRightButtonClick(true, e.X, e.Y);

            }

            glControl_main_panel.Invalidate();

        }

        private void glControl_main_panel_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Mouse wheel
            modeldata.graphic_events_control.handleMouseScroll(e.Delta, e.X, e.Y);

            glControl_main_panel.Invalidate();

        }

        private void glControl_main_panel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Mouse move 
            modeldata.graphic_events_control.handleMouseMove(e.X, e.Y);

            glControl_main_panel.Invalidate();

        }

        private void glControl_main_panel_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Left button up
                modeldata.graphic_events_control.handleMouseLeftButtonClick(false, e.X, e.Y);

            }
            else if (e.Button == MouseButtons.Right)
            {
                // Right button up
                modeldata.graphic_events_control.handleMouseRightButtonClick(false, e.X, e.Y);

            }

            glControl_main_panel.Invalidate();

            // Update the Material Property Form data
            if (modeldata.isMaterialUpdateInProgress == true)
            {
               // matprop_Form.update_selected_element_list();

            }

            // Update the Load Form data
            if (modeldata.isLoadUpdateInProgress == true)
            {
                // load_Form.update_selected_node_list();

            }

            // Update the Nodal Constraint Form data
            if (modeldata.isConstraintUpdateInProgress == true)
            {
                // nodalconstraint_Form.update_selected_node_list();

            }

        }

        private void glControl_main_panel_KeyDown(object sender, KeyEventArgs e)
        {
            // Keyboard Key Down
            modeldata.graphic_events_control.handleKeyboardAction(true, e.KeyValue);

            glControl_main_panel.Invalidate();

        }


        private void glControl_main_panel_KeyUp(object sender, KeyEventArgs e)
        {
            // Keyboard Key Up
            modeldata.graphic_events_control.handleKeyboardAction(false, e.KeyValue);

            glControl_main_panel.Invalidate();

            // If zoom-to-fit started, start the timer
            if (modeldata.graphic_events_control.isZoomToFitInProgress == true)
            {
                // Start the zoomToFit timer
                if (!zoomToFitTimer.Enabled)
                    zoomToFitTimer.Start();

            }


        }


        private void ZoomToFitTimer_Tick(object sender, EventArgs e)
        {
            glControl_ZoomToFitOperation();

        }

        private void glControl_ZoomToFitOperation()
        {
            // Refresh the glControl_main_panel as the zoom to fit operation in progress
            glControl_main_panel.Invalidate();

            if (modeldata.graphic_events_control.isZoomToFitInProgress == false)
            {
                // End the zoom to fit operation
                // Stop zoom-to-fit operation once done
                zoomToFitTimer.Stop();

            }

        }


        private bool IsApplicationIdle()
        {
            Message msg;
            return !gvariables_static.PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
        }

        private void OnApplicationIdle(object sender, EventArgs e)
        {
            while (IsApplicationIdle())
            {
                // fedata.resultmeshdata.update_modal_animation();   // Update animation
                glControl_main_panel.Invalidate(); // Redraw
            }
        }

        #endregion






    }
}
