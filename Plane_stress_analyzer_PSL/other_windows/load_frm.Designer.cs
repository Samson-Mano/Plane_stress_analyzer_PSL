namespace Plane_stress_analyzer_PSL.other_windows
{
    partial class load_frm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(load_frm));
            this.dataGridView_LoadList = new System.Windows.Forms.DataGridView();
            this.circleSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rectangleSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.button_deleteload = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_loadangle = new System.Windows.Forms.TextBox();
            this.textBox_loadamplitude = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button_applyload = new System.Windows.Forms.Button();
            this.textBox_selectednodes = new System.Windows.Forms.TextBox();
            this.Column1_loadsetid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2_nodeids = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3_loadampl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4_loadangle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_LoadList)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView_LoadList
            // 
            this.dataGridView_LoadList.AllowUserToAddRows = false;
            this.dataGridView_LoadList.AllowUserToDeleteRows = false;
            this.dataGridView_LoadList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_LoadList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_LoadList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1_loadsetid,
            this.Column2_nodeids,
            this.Column3_loadampl,
            this.Column4_loadangle});
            this.dataGridView_LoadList.Location = new System.Drawing.Point(332, 33);
            this.dataGridView_LoadList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dataGridView_LoadList.MultiSelect = false;
            this.dataGridView_LoadList.Name = "dataGridView_LoadList";
            this.dataGridView_LoadList.ReadOnly = true;
            this.dataGridView_LoadList.RowHeadersWidth = 62;
            this.dataGridView_LoadList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView_LoadList.Size = new System.Drawing.Size(439, 251);
            this.dataGridView_LoadList.TabIndex = 21;
            // 
            // circleSelectionToolStripMenuItem
            // 
            this.circleSelectionToolStripMenuItem.Name = "circleSelectionToolStripMenuItem";
            this.circleSelectionToolStripMenuItem.Size = new System.Drawing.Size(100, 20);
            this.circleSelectionToolStripMenuItem.Text = "Circle Selection";
            this.circleSelectionToolStripMenuItem.Click += new System.EventHandler(this.circleSelectionToolStripMenuItem_Click);
            // 
            // rectangleSelectionToolStripMenuItem
            // 
            this.rectangleSelectionToolStripMenuItem.Checked = true;
            this.rectangleSelectionToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rectangleSelectionToolStripMenuItem.Name = "rectangleSelectionToolStripMenuItem";
            this.rectangleSelectionToolStripMenuItem.Size = new System.Drawing.Size(122, 20);
            this.rectangleSelectionToolStripMenuItem.Text = "Rectangle Selection";
            this.rectangleSelectionToolStripMenuItem.Click += new System.EventHandler(this.rectangleSelectionToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rectangleSelectionToolStripMenuItem,
            this.circleSelectionToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 20;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // button_deleteload
            // 
            this.button_deleteload.Location = new System.Drawing.Point(581, 306);
            this.button_deleteload.Name = "button_deleteload";
            this.button_deleteload.Size = new System.Drawing.Size(130, 35);
            this.button_deleteload.TabIndex = 19;
            this.button_deleteload.Text = "Delete Load";
            this.button_deleteload.UseVisualStyleBackColor = true;
            this.button_deleteload.Click += new System.EventHandler(this.button_deleteload_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 16);
            this.label3.TabIndex = 16;
            this.label3.Text = "Selected Nodes: ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_loadangle);
            this.groupBox1.Controls.Add(this.textBox_loadamplitude);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 33);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(313, 105);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Nodal Load Data: ";
            // 
            // textBox_loadangle
            // 
            this.textBox_loadangle.Location = new System.Drawing.Point(156, 61);
            this.textBox_loadangle.Name = "textBox_loadangle";
            this.textBox_loadangle.Size = new System.Drawing.Size(100, 23);
            this.textBox_loadangle.TabIndex = 5;
            // 
            // textBox_loadamplitude
            // 
            this.textBox_loadamplitude.Location = new System.Drawing.Point(156, 25);
            this.textBox_loadamplitude.Name = "textBox_loadamplitude";
            this.textBox_loadamplitude.Size = new System.Drawing.Size(100, 23);
            this.textBox_loadamplitude.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(60, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Load Angle: ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Load Amplitude: ";
            // 
            // button_applyload
            // 
            this.button_applyload.Location = new System.Drawing.Point(399, 306);
            this.button_applyload.Name = "button_applyload";
            this.button_applyload.Size = new System.Drawing.Size(134, 35);
            this.button_applyload.TabIndex = 18;
            this.button_applyload.Text = "Apply Load";
            this.button_applyload.UseVisualStyleBackColor = true;
            this.button_applyload.Click += new System.EventHandler(this.button_applyload_Click);
            // 
            // textBox_selectednodes
            // 
            this.textBox_selectednodes.Location = new System.Drawing.Point(12, 162);
            this.textBox_selectednodes.Multiline = true;
            this.textBox_selectednodes.Name = "textBox_selectednodes";
            this.textBox_selectednodes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_selectednodes.Size = new System.Drawing.Size(313, 193);
            this.textBox_selectednodes.TabIndex = 17;
            // 
            // Column1_loadsetid
            // 
            this.Column1_loadsetid.FillWeight = 80F;
            this.Column1_loadsetid.HeaderText = "Node Load ID";
            this.Column1_loadsetid.MinimumWidth = 8;
            this.Column1_loadsetid.Name = "Column1_loadsetid";
            this.Column1_loadsetid.ReadOnly = true;
            this.Column1_loadsetid.Width = 80;
            // 
            // Column2_nodeids
            // 
            this.Column2_nodeids.HeaderText = "Node IDs";
            this.Column2_nodeids.MinimumWidth = 8;
            this.Column2_nodeids.Name = "Column2_nodeids";
            this.Column2_nodeids.ReadOnly = true;
            this.Column2_nodeids.Width = 125;
            // 
            // Column3_loadampl
            // 
            this.Column3_loadampl.FillWeight = 80F;
            this.Column3_loadampl.HeaderText = "Load Amplitude";
            this.Column3_loadampl.MinimumWidth = 8;
            this.Column3_loadampl.Name = "Column3_loadampl";
            this.Column3_loadampl.ReadOnly = true;
            this.Column3_loadampl.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column3_loadampl.Width = 80;
            // 
            // Column4_loadangle
            // 
            this.Column4_loadangle.FillWeight = 80F;
            this.Column4_loadangle.HeaderText = "Load Angle";
            this.Column4_loadangle.MinimumWidth = 8;
            this.Column4_loadangle.Name = "Column4_loadangle";
            this.Column4_loadangle.ReadOnly = true;
            this.Column4_loadangle.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column4_loadangle.Width = 80;
            // 
            // load_frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 361);
            this.Controls.Add(this.dataGridView_LoadList);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.button_deleteload);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_applyload);
            this.Controls.Add(this.textBox_selectednodes);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(800, 400);
            this.Name = "load_frm";
            this.Opacity = 0.85D;
            this.Text = "Nodal Loads";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.load_frm_FormClosing);
            this.Load += new System.EventHandler(this.load_frm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_LoadList)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView_LoadList;
        private System.Windows.Forms.ToolStripMenuItem circleSelectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rectangleSelectionToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Button button_deleteload;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_applyload;
        private System.Windows.Forms.TextBox textBox_selectednodes;
        private System.Windows.Forms.TextBox textBox_loadangle;
        private System.Windows.Forms.TextBox textBox_loadamplitude;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1_loadsetid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2_nodeids;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3_loadampl;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4_loadangle;
    }
}