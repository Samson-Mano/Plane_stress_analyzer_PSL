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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel_zoom_value = new System.Windows.Forms.ToolStripStatusLabel();
            this.glControl_main_panel = new OpenTK.GLControl();
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
            this.menuStrip1.Size = new System.Drawing.Size(1067, 28);
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
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // importTXTFileToolStripMenuItem
            // 
            this.importTXTFileToolStripMenuItem.Name = "importTXTFileToolStripMenuItem";
            this.importTXTFileToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.importTXTFileToolStripMenuItem.Text = "Import TXT File";
            // 
            // importModelToolStripMenuItem
            // 
            this.importModelToolStripMenuItem.Name = "importModelToolStripMenuItem";
            this.importModelToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.importModelToolStripMenuItem.Text = "Import Model";
            // 
            // exportModelToolStripMenuItem
            // 
            this.exportModelToolStripMenuItem.Name = "exportModelToolStripMenuItem";
            this.exportModelToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.exportModelToolStripMenuItem.Text = "Export Model";
            // 
            // optionToolStripMenuItem
            // 
            this.optionToolStripMenuItem.Name = "optionToolStripMenuItem";
            this.optionToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.optionToolStripMenuItem.Text = "Option";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // boundaryConditionToolStripMenuItem
            // 
            this.boundaryConditionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addLoadsToolStripMenuItem,
            this.addConstraintsToolStripMenuItem,
            this.materialPropertiesToolStripMenuItem});
            this.boundaryConditionToolStripMenuItem.Name = "boundaryConditionToolStripMenuItem";
            this.boundaryConditionToolStripMenuItem.Size = new System.Drawing.Size(155, 24);
            this.boundaryConditionToolStripMenuItem.Text = "Boundary Condition";
            // 
            // addLoadsToolStripMenuItem
            // 
            this.addLoadsToolStripMenuItem.Name = "addLoadsToolStripMenuItem";
            this.addLoadsToolStripMenuItem.Size = new System.Drawing.Size(218, 26);
            this.addLoadsToolStripMenuItem.Text = "Add Loads";
            // 
            // addConstraintsToolStripMenuItem
            // 
            this.addConstraintsToolStripMenuItem.Name = "addConstraintsToolStripMenuItem";
            this.addConstraintsToolStripMenuItem.Size = new System.Drawing.Size(218, 26);
            this.addConstraintsToolStripMenuItem.Text = "Add Constraints";
            // 
            // materialPropertiesToolStripMenuItem
            // 
            this.materialPropertiesToolStripMenuItem.Name = "materialPropertiesToolStripMenuItem";
            this.materialPropertiesToolStripMenuItem.Size = new System.Drawing.Size(218, 26);
            this.materialPropertiesToolStripMenuItem.Text = "Material Properties";
            // 
            // solveToolStripMenuItem
            // 
            this.solveToolStripMenuItem.Name = "solveToolStripMenuItem";
            this.solveToolStripMenuItem.Size = new System.Drawing.Size(59, 24);
            this.solveToolStripMenuItem.Text = "Solve";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel_zoom_value});
            this.statusStrip1.Location = new System.Drawing.Point(0, 528);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1067, 26);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel_zoom_value
            // 
            this.toolStripStatusLabel_zoom_value.Name = "toolStripStatusLabel_zoom_value";
            this.toolStripStatusLabel_zoom_value.Size = new System.Drawing.Size(92, 20);
            this.toolStripStatusLabel_zoom_value.Text = "Zoom: 100%";
            // 
            // glControl_main_panel
            // 
            this.glControl_main_panel.BackColor = System.Drawing.Color.Black;
            this.glControl_main_panel.Location = new System.Drawing.Point(217, 176);
            this.glControl_main_panel.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.glControl_main_panel.Name = "glControl_main_panel";
            this.glControl_main_panel.Size = new System.Drawing.Size(344, 185);
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
            // main_frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 554);
            this.Controls.Add(this.glControl_main_panel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
    }
}

