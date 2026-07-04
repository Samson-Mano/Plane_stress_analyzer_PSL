namespace Plane_stress_analyzer_PSL
{
    partial class main_frm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(main_frm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importTXTFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boundaryConditionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addLoadsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addConstraintsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.materialPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.finiteElementSolverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.resultOptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel_zoom_value = new System.Windows.Forms.ToolStripStatusLabel();
            this.glControl_main_panel = new OpenTK.GLControl();
            this.resultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displacementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stressXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stressYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tauXYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.hideResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.boundaryConditionToolStripMenuItem,
            this.solveToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importTXTFileToolStripMenuItem,
            this.importModelToolStripMenuItem,
            this.exportModelToolStripMenuItem,
            this.optionToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // importTXTFileToolStripMenuItem
            // 
            this.importTXTFileToolStripMenuItem.Name = "importTXTFileToolStripMenuItem";
            this.importTXTFileToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.importTXTFileToolStripMenuItem.Text = "Import TXT File";
            this.importTXTFileToolStripMenuItem.Click += new System.EventHandler(this.importTXTFileToolStripMenuItem_Click);
            // 
            // importModelToolStripMenuItem
            // 
            this.importModelToolStripMenuItem.Name = "importModelToolStripMenuItem";
            this.importModelToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.importModelToolStripMenuItem.Text = "Import Model";
            this.importModelToolStripMenuItem.Click += new System.EventHandler(this.importModelToolStripMenuItem_Click);
            // 
            // exportModelToolStripMenuItem
            // 
            this.exportModelToolStripMenuItem.Name = "exportModelToolStripMenuItem";
            this.exportModelToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.exportModelToolStripMenuItem.Text = "Export Model";
            this.exportModelToolStripMenuItem.Click += new System.EventHandler(this.exportModelToolStripMenuItem_Click);
            // 
            // optionToolStripMenuItem
            // 
            this.optionToolStripMenuItem.Name = "optionToolStripMenuItem";
            this.optionToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.optionToolStripMenuItem.Text = "Option";
            this.optionToolStripMenuItem.Click += new System.EventHandler(this.optionToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // boundaryConditionToolStripMenuItem
            // 
            this.boundaryConditionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addLoadsToolStripMenuItem,
            this.addConstraintsToolStripMenuItem,
            this.materialPropertiesToolStripMenuItem});
            this.boundaryConditionToolStripMenuItem.Name = "boundaryConditionToolStripMenuItem";
            this.boundaryConditionToolStripMenuItem.Size = new System.Drawing.Size(126, 20);
            this.boundaryConditionToolStripMenuItem.Text = "Boundary Condition";
            // 
            // addLoadsToolStripMenuItem
            // 
            this.addLoadsToolStripMenuItem.Name = "addLoadsToolStripMenuItem";
            this.addLoadsToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.addLoadsToolStripMenuItem.Text = "Add Loads";
            this.addLoadsToolStripMenuItem.Click += new System.EventHandler(this.addLoadsToolStripMenuItem_Click);
            // 
            // addConstraintsToolStripMenuItem
            // 
            this.addConstraintsToolStripMenuItem.Name = "addConstraintsToolStripMenuItem";
            this.addConstraintsToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.addConstraintsToolStripMenuItem.Text = "Add Constraints";
            this.addConstraintsToolStripMenuItem.Click += new System.EventHandler(this.addConstraintsToolStripMenuItem_Click);
            // 
            // materialPropertiesToolStripMenuItem
            // 
            this.materialPropertiesToolStripMenuItem.Name = "materialPropertiesToolStripMenuItem";
            this.materialPropertiesToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.materialPropertiesToolStripMenuItem.Text = "Material Properties";
            this.materialPropertiesToolStripMenuItem.Click += new System.EventHandler(this.materialPropertiesToolStripMenuItem_Click);
            // 
            // solveToolStripMenuItem
            // 
            this.solveToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.finiteElementSolverToolStripMenuItem,
            this.toolStripSeparator1,
            this.resultOptionToolStripMenuItem,
            this.resultsToolStripMenuItem});
            this.solveToolStripMenuItem.Name = "solveToolStripMenuItem";
            this.solveToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.solveToolStripMenuItem.Text = "Solve";
            // 
            // finiteElementSolverToolStripMenuItem
            // 
            this.finiteElementSolverToolStripMenuItem.Name = "finiteElementSolverToolStripMenuItem";
            this.finiteElementSolverToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.finiteElementSolverToolStripMenuItem.Text = "Finite Element Solver";
            this.finiteElementSolverToolStripMenuItem.Click += new System.EventHandler(this.finiteElementSolverToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(181, 6);
            // 
            // resultOptionToolStripMenuItem
            // 
            this.resultOptionToolStripMenuItem.Name = "resultOptionToolStripMenuItem";
            this.resultOptionToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.resultOptionToolStripMenuItem.Text = "Result Option";
            this.resultOptionToolStripMenuItem.Click += new System.EventHandler(this.resultOptionToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel_zoom_value});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel_zoom_value
            // 
            this.toolStripStatusLabel_zoom_value.Name = "toolStripStatusLabel_zoom_value";
            this.toolStripStatusLabel_zoom_value.Size = new System.Drawing.Size(73, 17);
            this.toolStripStatusLabel_zoom_value.Text = "Zoom: 100%";
            // 
            // glControl_main_panel
            // 
            this.glControl_main_panel.BackColor = System.Drawing.Color.Black;
            this.glControl_main_panel.Location = new System.Drawing.Point(163, 143);
            this.glControl_main_panel.Margin = new System.Windows.Forms.Padding(4);
            this.glControl_main_panel.Name = "glControl_main_panel";
            this.glControl_main_panel.Size = new System.Drawing.Size(258, 150);
            this.glControl_main_panel.TabIndex = 2;
            this.glControl_main_panel.VSync = false;
            this.glControl_main_panel.Load += new System.EventHandler(this.glControl_main_panel_Load);
            this.glControl_main_panel.SizeChanged += new System.EventHandler(this.glControl_main_panel_SizeChanged);
            this.glControl_main_panel.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl_main_panel_Paint);
            this.glControl_main_panel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glControl_main_panel_KeyDown);
            this.glControl_main_panel.KeyUp += new System.Windows.Forms.KeyEventHandler(this.glControl_main_panel_KeyUp);
            this.glControl_main_panel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl_main_panel_MouseDown);
            this.glControl_main_panel.MouseEnter += new System.EventHandler(this.glControl_main_panel_MouseEnter);
            this.glControl_main_panel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl_main_panel_MouseMove);
            this.glControl_main_panel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glControl_main_panel_MouseUp);
            this.glControl_main_panel.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glControl_main_panel_MouseWheel);
            // 
            // resultsToolStripMenuItem
            // 
            this.resultsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.displacementToolStripMenuItem,
            this.stressXToolStripMenuItem,
            this.stressYToolStripMenuItem,
            this.tauXYToolStripMenuItem,
            this.toolStripSeparator2,
            this.hideResultsToolStripMenuItem});
            this.resultsToolStripMenuItem.Name = "resultsToolStripMenuItem";
            this.resultsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.resultsToolStripMenuItem.Text = "Results";
            // 
            // displacementToolStripMenuItem
            // 
            this.displacementToolStripMenuItem.Name = "displacementToolStripMenuItem";
            this.displacementToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.displacementToolStripMenuItem.Text = "Displacement";
            this.displacementToolStripMenuItem.Click += new System.EventHandler(this.displacementToolStripMenuItem_Click);
            // 
            // stressXToolStripMenuItem
            // 
            this.stressXToolStripMenuItem.Name = "stressXToolStripMenuItem";
            this.stressXToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.stressXToolStripMenuItem.Text = "Stress X";
            this.stressXToolStripMenuItem.Click += new System.EventHandler(this.stressXToolStripMenuItem_Click);
            // 
            // stressYToolStripMenuItem
            // 
            this.stressYToolStripMenuItem.Name = "stressYToolStripMenuItem";
            this.stressYToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.stressYToolStripMenuItem.Text = "Stress Y";
            this.stressYToolStripMenuItem.Click += new System.EventHandler(this.stressYToolStripMenuItem_Click);
            // 
            // tauXYToolStripMenuItem
            // 
            this.tauXYToolStripMenuItem.Name = "tauXYToolStripMenuItem";
            this.tauXYToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.tauXYToolStripMenuItem.Text = "Tau XY";
            this.tauXYToolStripMenuItem.Click += new System.EventHandler(this.tauXYToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            // 
            // hideResultsToolStripMenuItem
            // 
            this.hideResultsToolStripMenuItem.Name = "hideResultsToolStripMenuItem";
            this.hideResultsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.hideResultsToolStripMenuItem.Text = "Hide Results";
            this.hideResultsToolStripMenuItem.Click += new System.EventHandler(this.hideResultsToolStripMenuItem_Click);
            // 
            // main_frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.glControl_main_panel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "main_frm";
            this.Text = "Plane Stress Analyzer - Principal Stress Line PSL";
            this.Load += new System.EventHandler(this.main_frm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem boundaryConditionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem solveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importTXTFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importModelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportModelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addLoadsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addConstraintsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem materialPropertiesToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_zoom_value;
        private OpenTK.GLControl glControl_main_panel;
        private System.Windows.Forms.ToolStripMenuItem finiteElementSolverToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem resultOptionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resultsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displacementToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stressXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stressYToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tauXYToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem hideResultsToolStripMenuItem;
    }
}

