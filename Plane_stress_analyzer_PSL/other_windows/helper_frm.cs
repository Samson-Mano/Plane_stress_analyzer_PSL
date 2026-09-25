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
    public partial class helper_frm : Form
    {
        public helper_frm()
        {
            InitializeComponent();

            // Ensure RichTextBox is configured before Load populates it.
            richTextBox_help.ReadOnly = true;
            richTextBox_help.WordWrap = false;   // critical for the code template
            richTextBox_help.ScrollBars = RichTextBoxScrollBars.Both;
            richTextBox_help.BorderStyle = BorderStyle.Fixed3D;
            richTextBox_help.BackColor = Color.White;

            richTextBox_help.DetectUrls = true;
            richTextBox_help.LinkClicked += richTextBox_help_LinkClicked;
        }

        private void helper_frm_Load(object sender, EventArgs e)
        {
            PopulateHelpContent();
        }


        // ---------------------------------------------------------------
        // Content population
        // ---------------------------------------------------------------
        private void PopulateHelpContent()
        {
            // Suspend layout to avoid flicker during many appends.
            richTextBox_help.SuspendLayout();
            richTextBox_help.Clear();

            // Fonts — create once, reuse, dispose at the end.
            var fTitle = new Font("Segoe UI", 16f, FontStyle.Bold);
            var fSub = new Font("Segoe UI", 10f, FontStyle.Italic);
            var fSection = new Font("Segoe UI", 12f, FontStyle.Bold);
            var fBody = new Font("Segoe UI", 10f);
            var fMono = new Font("Consolas", 10f);
            var fMonoB = new Font("Consolas", 10f, FontStyle.Bold);
            var fLink = new Font("Segoe UI", 10f, FontStyle.Underline);

            // Colors
            var cTitle = Color.FromArgb(20, 20, 20);
            var cSub = Color.FromArgb(100, 100, 100);
            var cSection = Color.FromArgb(0, 70, 140);
            var cBody = Color.FromArgb(30, 30, 30);
            var cCode = Color.FromArgb(0, 100, 0);
            var cLink = Color.FromArgb(0, 90, 180);
            var cKey = Color.FromArgb(160, 30, 30);   // for keyboard shortcuts

            // ---------------------------------------------------------------
            // Local helpers
            // ---------------------------------------------------------------
            void Append(string text, Font font, Color color)
            {
                richTextBox_help.SelectionFont = font;
                richTextBox_help.SelectionColor = color;
                richTextBox_help.AppendText(text);
            }

            void Line(string text, Font font, Color color)
            {
                Append(text + "\n", font, color);
            }

            void Blank()
            {
                Append("\n", fBody, cBody);
            }

            // ---------------------------------------------------------------
            // Title
            // ---------------------------------------------------------------
            Line("GENERAL INSTRUCTIONS", fTitle, cTitle);
            Line("How to use the software", fSub, cSub);
            Blank();

            // ---------------------------------------------------------------
            // 1) Navigation
            // ---------------------------------------------------------------
            Line("1)  NAVIGATION", fSection, cSection);
            Blank();
            Line("      Zoom In / Out ......... Ctrl + Scroll Wheel", fBody, cBody);
            Line("      Pan ................... Ctrl + Right-Click Drag", fBody, cBody);
            Line("      Zoom to Fit ........... Ctrl + F", fBody, cBody);
            Line("      Select ................ Shift + Left-Click Drag", fBody, cBody);
            Line("      Deselect .............. Shift + Right-Click Drag", fBody, cBody);
            Blank();

            // ---------------------------------------------------------------
            // 2) Importing a model
            // ---------------------------------------------------------------
            Line("2)  IMPORTING A MODEL", fSection, cSection);
            Blank();
            Line("The 2D domain is created using an external general-purpose meshing",
                 fBody, cBody);
            Line("tool or a text editor. The mesh text file (*.txt) follows the format",
                 fBody, cBody);
            Line("shown below.", fBody, cBody);
            Blank();

            // Code template block — monospace
            Line("    **", fMono, cCode);
            Line("    **   Template:  Plane Stress Analyzer", fMonoB, cCode);
            Line("    **", fMono, cCode);
            Line("    *NODE", fMono, cCode);
            Line("             1,  -100.0   ,  -100.0   ,  0.0", fMono, cCode);
            Line("             2,   100.0   ,  -100.0   ,  0.0", fMono, cCode);
            Line("             3,   100.0   ,   100.0   ,  0.0", fMono, cCode);
            Line("             4,  -100.0   ,   100.0   ,  0.0", fMono, cCode);
            Line("             ......", fMono, cCode);
            Blank();
            Line("    *ELEMENT,TYPE=S4", fMonoB, cCode);
            Line("            12,    23,    13,     7,    20", fMono, cCode);
            Line("            13,    18,    23,    20,     9", fMono, cCode);
            Line("            14,     9,    20,    24,    21", fMono, cCode);
            Line("            15,    20,     7,    14,    24", fMono, cCode);
            Line("            .......", fMono, cCode);
            Blank();
            Line("    *ELEMENT,TYPE=S3", fMonoB, cCode);
            Line("             0,     0,     1,     2", fMono, cCode);
            Line("             1,     2,     1,     3", fMono, cCode);
            Line("             3,     2,     3,     4", fMono, cCode);
            Line("             .......", fMono, cCode);
            Blank();
            Line("Follow the example format kept in the repository:", fBody, cBody);
            Line("https://github.com/Samson-Mano/Plane_stress_analyzer_PSL", fLink, cLink);
            Blank();

            // ---------------------------------------------------------------
            // 3) Adding loads and constraints
            // ---------------------------------------------------------------
            Line("3)  ADDING LOADS AND CONSTRAINTS", fSection, cSection);
            Blank();
            Line("After importing the model, apply loads or constraints on the nodes",
                 fBody, cBody);
            Line("by opening the Add Loads or Add Constraints form.",
                 fBody, cBody);
            Blank();
            Line("      Shift + Left-Click Drag ....... Select nodes", fBody, cBody);
            Line("      Shift + Right-Click Drag ...... Deselect to refine selection", fBody, cBody);
            Line("      Selection modes ............... Rectangle or Circle", fBody, cBody);
            Blank();

            // ---------------------------------------------------------------
            // 4) Creating / Updating material properties
            // ---------------------------------------------------------------
            Line("4)  CREATING / UPDATING MATERIAL PROPERTIES", fSection, cSection);
            Blank();
            Line("Mesh material properties can be updated using the Material Properties",
                 fBody, cBody);
            Line("form.", fBody, cBody);
            Blank();
            Line("      Create / Update or Delete material", fBody, cBody);
            Line("      Shift + Left-Click Drag ....... Select elements", fBody, cBody);
            Line("      Shift + Right-Click Drag ...... Deselect to refine selection", fBody, cBody);
            Line("      Selection modes ............... Rectangle or Circle", fBody, cBody);
            Blank();

            // ---------------------------------------------------------------
            // 5) Solving
            // ---------------------------------------------------------------
            Line("5)  SOLVING", fSection, cSection);
            Blank();
            Line("The finite element solver is developed in C++ and accessed through",
                 fBody, cBody);
            Line("a DLL. The solver supports h-refinement and p-refinement. The",
                 fBody, cBody);
            Line("h-refined model can be saved at the solver location.",
                 fBody, cBody);
            Blank();
            Line("Use \"Result Options\" to change the display. Adjust the contour range",
                 fBody, cBody);
            Line("to change the maximum/ minimum contour value.", fBody, cBody);
            Blank();
            Line("\"Annotate Results\" displays the nodal results.", fBody, cBody);
            Blank();
            Line("Change the result options to view different contour plots. Plane",
                 fBody, cBody);
            Line("stress lines can be plotted in two ways:", fBody, cBody);
            Blank();
            Line("      PSL Type 1 ..... Stress lines in grid mode, with the reaction",
                 fBody, cBody);
            Line("                       force vectors at the constraints.", fBody, cBody);
            Blank();
            Line("      PSL Type 2 ..... Stress lines as continuous stream lines.",
                 fBody, cBody);
            Blank();

            // ---------------------------------------------------------------
            // 6) Contact
            // ---------------------------------------------------------------
            Line("6)  CONTACT", fSection, cSection);
            Blank();
            Line("For help, contact:", fBody, cBody);
            Line("saminnx@gmail.com", fLink, cLink);
            Blank();
            Line("Or visit the repository:", fBody, cBody);
            Line("https://github.com/Samson-Mano/Plane_stress_analyzer_PSL", fLink, cLink);
            Blank();

            // ---------------------------------------------------------------
            // Footer
            // ---------------------------------------------------------------
            Line("Help version 1.0  —  last updated 2026-Sep-24", fSub, cSub);

            // Move caret to top so the user sees the beginning
            richTextBox_help.SelectionStart = 0;
            richTextBox_help.SelectionLength = 0;
            richTextBox_help.ScrollToCaret();

            richTextBox_help.ResumeLayout();
        }


        private void richTextBox_help_LinkClicked(object sender,
           LinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = e.LinkText,
                    UseShellExecute = true
                });
            }
            catch
            {
                // Ignore: no browser, or invalid URL
            }
        }



    }
}
