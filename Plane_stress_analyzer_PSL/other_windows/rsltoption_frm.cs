using Plane_stress_analyzer_PSL;
using Plane_stress_analyzer_PSL.src.global_variables;
using Plane_stress_analyzer_PSL.src.model_store;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace other_windows
{
    public partial class rsltoption_frm : Form
    {
        // Define enum for paint options
        private enum PaintOption
        {
            ResultMeshPoints = 1,
            ResultMesh = 2,
            ResultMeshBoundaries = 3,
            PaintContourLines = 4,
            ShowTransparentModelMesh = 5
        }

        private Panel panelPopup;
        private TextBox textPopup;
        private Button buttonPopupOK;
        private Button buttonPopupCancel;


        private modeldata_store modeldata;


        public rsltoption_frm(ref modeldata_store modeldata)
        {
            InitializeComponent();

            // Model data
            this.modeldata = modeldata;


            // Initialize checkboxes from current state
            LoadSettings();


            // === Popup Panel ===
            panelPopup = new Panel();
            panelPopup.Size = new Size(220, 70);
            panelPopup.BackColor = Form.DefaultBackColor;
            panelPopup.BorderStyle = BorderStyle.FixedSingle;
            panelPopup.Visible = false;   // Hidden by default
            panelPopup.BringToFront();
            groupBox2.Controls.Add(panelPopup);

            // === TextBox ===
            textPopup = new TextBox();
            textPopup.Size = new Size(200, 25);
            textPopup.Location = new Point(10, 10);
            panelPopup.Controls.Add(textPopup);

            // === OK Button ===
            buttonPopupOK = new Button();
            buttonPopupOK.Text = "OK";
            buttonPopupOK.Size = new Size(80, 25);
            buttonPopupOK.Location = new Point(20, 40);
            buttonPopupOK.Click += ButtonPopupOK_Click;
            panelPopup.Controls.Add(buttonPopupOK);

            // === Cancel Button ===
            buttonPopupCancel = new Button();
            buttonPopupCancel.Text = "Cancel";
            buttonPopupCancel.Size = new Size(80, 25);
            buttonPopupCancel.Location = new Point(120, 40);
            buttonPopupCancel.Click += ButtonPopupCancel_Click;
            panelPopup.Controls.Add(buttonPopupCancel);

        }


        private void LoadSettings()
        {

            checkBox_showtransparentmesh.Checked = gvariables_static.is_show_transparent_model_mesh;
            checkBox_paintrsltmeshpoints.Checked = gvariables_static.is_paint_resultmeshpoints;
            checkBox_paintrsltmesh.Checked = gvariables_static.is_paint_resultmesh;
            checkBox_paintrsltmeshboundaries.Checked = gvariables_static.is_paint_resultmesh_boundaries;
            checkBox_paintcontourlines.Checked = gvariables_static.is_paint_result_contourlines;

            // Contour levels selection
            // 0 = 5, 1 = 10, 2 = 20, 3 = 40, 4 = 80
            switch (gvariables_static.contourline_level)
            {
                case 0:
                    comboBox_contourlevels.SelectedIndex = 0; // 5
                    break;
                case 1:
                    comboBox_contourlevels.SelectedIndex = 1; // 10
                    break;
                case 2:
                    comboBox_contourlevels.SelectedIndex = 2; // 20
                    break;
                case 3:
                    comboBox_contourlevels.SelectedIndex = 3; // 40
                    break;
                case 4:
                    comboBox_contourlevels.SelectedIndex = 4; // 80
                    break;
                default:
                    comboBox_contourlevels.SelectedIndex = 1; // Default to 10 if out of range
                    break;
            }


            // Contour limit range
            textBox_contourmax.Text = gvariables_static.contourLevel_rangeMax.ToString(CultureInfo.InvariantCulture);
            textBox_contourmin.Text = gvariables_static.contourLevel_rangeMin.ToString(CultureInfo.InvariantCulture);


            // === Track bar control ===
            trackBar_deformation_scale.Minimum = 0;
            trackBar_deformation_scale.Maximum = 199;

            // Update the scale labels based on current global variable values
            trackBar_deformation_scale.Value = ComputeTrackBarFromScale(gvariables_static.displacement_scale);

            UpdateScale(
              trackBar_deformation_scale,
              label_deformation_scale,
              "Deformation scale",
              v => v = gvariables_static.displacement_scale
          );


            //____________________________________________________________________________________________________

            if (gvariables_static.animate_play)
            {

                // Set the status label Playing
                label_status.Text = "Playing";

            }
            else if (gvariables_static.animate_pause)
            {
                // Set the status label Paused
                label_status.Text = "Paused";
            }
            else
            {
                // Set the status label Stopped
                label_status.Text = "Stopped";

            }

            gvariables_static.resp_animation_speed = Properties.Settings.Default.Sett_resp_animation_speed;

            // Set the global variable
            double value = gvariables_static.resp_animation_speed;

            // Set label
            label_animation_speed.Text = value.ToString(CultureInfo.InvariantCulture);
            label_realtimeanim_speed.Text = $"1 second in real time = {value.ToString(CultureInfo.InvariantCulture)} second in Animation";


        }


        private void UpdateScale(TrackBar bar, Label label, string prefix, Action<double> setValue)
        {
            double value = ComputeScaleFromTrackBar(bar.Value);

            label.Text = $"{prefix} = {value:F1}";
            setValue(value);
        }


        private double ComputeScaleFromTrackBar(int value)
        {
            const int PIVOT_INTEGER = 10;

            if (value <= PIVOT_INTEGER)
            {
                // --- Fine Control (0.7, 0.8, 0.9, 1.0) --- 
                // The step is 0.1. 
                // When I=3, difference is 0. 1.0 + 0 = 1.0 
                // When I=2, difference is -1. 1.0 + (-1 * 0.1) = 0.9

                int difference = value - PIVOT_INTEGER;
                return 1.0 + (difference * 0.1);   // fine step
            }
            else
            {
                // --- Coarse Control (1.0, 2.0, 3.0, ... 8.0) --- 
                // The step is 1.0. 
                // When I=4, difference is 1. 1.0 + (1 * 1.0) = 2.0 
                // When I=10 (Max), difference is 7. 1.0 + (7 * 1.0) = 8.0

                int difference = value - PIVOT_INTEGER;
                return 1.0 + (difference * 1.0);   // coarse step
            }

        }

        private int ComputeTrackBarFromScale(double scale)
        {
            const int PIVOT_INTEGER = 10;

            // Fine range: 0.7 → 1.0   (step 0.1)
            if (scale <= 1.0)
            {
                // scale = 1.0 + (difference * 0.1)
                // difference = (scale - 1.0) / 0.1
                int difference = (int)Math.Round((scale - 1.0) / 0.1);
                return PIVOT_INTEGER + difference;
            }
            else
            {
                // Coarse range: 1.0 → 8.0   (step 1.0)
                // scale = 1.0 + (difference * 1.0)
                // difference = scale - 1.0
                int difference = (int)Math.Round(scale - 1.0);
                return PIVOT_INTEGER + difference;
            }
        }





        private void button_ok_Click(object sender, EventArgs e)
        {
            // Exit the option form
            this.Close();
        }



        private void checkBox_showtransparentmesh_CheckedChanged(object sender, EventArgs e) =>
                    SetPaintOption(PaintOption.ShowTransparentModelMesh, checkBox_showtransparentmesh.Checked);

        private void checkBox_paintrsltmeshpoints_CheckedChanged(object sender, EventArgs e) =>
                    SetPaintOption(PaintOption.ResultMeshPoints, checkBox_paintrsltmeshpoints.Checked);


        private void checkBox_paintrsltmesh_CheckedChanged(object sender, EventArgs e) =>
                    SetPaintOption(PaintOption.ResultMesh, checkBox_paintrsltmesh.Checked);


        private void checkBox_paintrsltmeshboundaries_CheckedChanged(object sender, EventArgs e) =>
                    SetPaintOption(PaintOption.ResultMeshBoundaries, checkBox_paintrsltmeshboundaries.Checked);


        private void checkBox_paintcontourlines_CheckedChanged(object sender, EventArgs e) =>
                    SetPaintOption(PaintOption.PaintContourLines, checkBox_paintcontourlines.Checked);



        private void SetPaintOption(PaintOption option, bool isChecked)
        {
            switch (option)
            {
                case PaintOption.ResultMeshPoints:
                    gvariables_static.is_paint_resultmeshpoints = isChecked;
                    break;
                case PaintOption.ResultMesh:
                    gvariables_static.is_paint_resultmesh = isChecked;
                    break;
                case PaintOption.ResultMeshBoundaries:
                    gvariables_static.is_paint_resultmesh_boundaries = isChecked;
                    break;
                case PaintOption.PaintContourLines:
                    gvariables_static.is_paint_result_contourlines = isChecked;
                    break;
                case PaintOption.ShowTransparentModelMesh:
                    gvariables_static.is_show_transparent_model_mesh = isChecked;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, null);
            }

            // Optional: Trigger immediate redraw
            // Application.DoEvents(); or raise an event

            if (this.Owner is main_frm mainForm)
            {
                mainForm.CallFrom_option_frm();
            }
        }


        private void comboBox_contourlevels_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_contourlevels.SelectedIndex)
            {
                case 0:
                    gvariables_static.contourline_level = 5; // 5
                    break;
                case 1:
                    gvariables_static.contourline_level = 10; // 10
                    break;
                case 2:
                    gvariables_static.contourline_level = 20; // 20
                    break;
                case 3:
                    gvariables_static.contourline_level = 40; // 40
                    break;
                case 4:
                    gvariables_static.contourline_level = 80; // 80
                    break;
                default:
                    gvariables_static.contourline_level = 10; // Default to 10 if out of range
                    break;
            }

            if (this.Owner is main_frm mainForm)
            {
                mainForm.CallFrom_option_frm();
            }

        }


        private void ButtonPopupOK_Click(object sender, EventArgs e)
        {
            string input = textPopup.Text;

            // Test whether the input is a valid number (positive integer or float)
            // Try to parse the input as a floating-point number
            bool isNumeric = double.TryParse(
                input,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double value
            );

            // Validate: numeric AND positive
            if (!isNumeric || value <= 0)
            {
                panelPopup.Visible = false;
                label_realtimeanim_speed.Visible = true;

                return; // Do not continue
            }

            // Set the modal animation global variable
            gvariables_static.resp_animation_speed = value;

            // Set label
            label_animation_speed.Text = value.ToString(CultureInfo.InvariantCulture);
            label_realtimeanim_speed.Text = $"1 second in real time = {value.ToString(CultureInfo.InvariantCulture)} second in Animation";

            panelPopup.Visible = false;
            label_realtimeanim_speed.Visible = true;
        }

        private void ButtonPopupCancel_Click(object sender, EventArgs e)
        {
            panelPopup.Visible = false;
            label_realtimeanim_speed.Visible = true;
        }

        private void button_play_pause_Click(object sender, EventArgs e)
        {
            if (gvariables_static.animate_play)
            {
                // Currently playing, so pause
                gvariables_static.animate_play = false;
                gvariables_static.animate_pause = true;

                modeldata.pause_animation();

                // Set the status label
                label_status.Text = "Paused";

            }
            else
            {
                // Currently paused/stopped, so play
                gvariables_static.animate_play = true;
                gvariables_static.animate_pause = false;

                // Start or resume the animation
                modeldata.start_animation();

                // Set the status label
                label_status.Text = "Playing";

            }


            if (gvariables_static.animate_stop == true)
            {
                // Retart the animation from the beginning
                gvariables_static.animate_stop = false;

                // Restart the animation
                modeldata.start_animation();

            }


        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            // Stop the animation
            gvariables_static.animate_play = false;
            gvariables_static.animate_pause = false;
            gvariables_static.animate_stop = true;

            // Reset the animation to the beginning
            modeldata.stop_animation();

            label_status.Text = "Stopped";
        }

        private void button_animation_speed_Click(object sender, EventArgs e)
        {
            // Position near the button
            panelPopup.Location = new Point(button_animation_speed.Left, button_animation_speed.Bottom);

            textPopup.Text = "";         // Clear previous input
            panelPopup.Visible = true;   // Show popup
            label_realtimeanim_speed.Visible = false;
            textPopup.Focus();           // Focus for typing
        }

        private void trackBar_deformation_scale_Scroll(object sender, EventArgs e)
        {
            UpdateScale(
                         trackBar_deformation_scale,
                         label_deformation_scale,
                         "Deformation scale",
                         v => gvariables_static.displacement_scale = v
                     );

            if (this.Owner is main_frm mainForm)
            {
                mainForm.CallFrom_option_frm();
            }

        }

        private void rsltoption_frm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Update the settings
            Properties.Settings.Default.Sett_displ_scale = gvariables_static.displacement_scale;
            Properties.Settings.Default.Sett_resp_animation_speed = gvariables_static.resp_animation_speed;

            Properties.Settings.Default.Sett_contourlevel_option = gvariables_static.contourline_level;

            Properties.Settings.Default.Save(); // Save the settings

        }


        private void button_updaterange_Click(object sender, EventArgs e)
        {
            // Update the Contour range limits
            // Validate the input values
            if (textBox_contourmax.Text == "" || textBox_contourmin.Text == "")
            {
                return;
            }

            if (float.TryParse(textBox_contourmax.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float max) &&
               float.TryParse(textBox_contourmin.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float min))
            {
                if (max > min && min >= 0.0f && max <= 1.0f)
                {
                    gvariables_static.contourLevel_rangeMax = max;
                    gvariables_static.contourLevel_rangeMin = min;

                    modeldata.switch_result_option(true);

                    // Optional: Trigger immediate redraw
                    if (this.Owner is main_frm mainForm)
                    {
                        mainForm.CallFrom_option_frm();
                    }
                }
                else
                {
                    // MessageBox.Show("Contour max must be greater than contour min.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                // MessageBox.Show("Please enter valid numeric values for contour range.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
    }
}
