namespace other_windows
{
    partial class annotate_frm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(annotate_frm));
            this.dataGridView_ResultNodeList = new System.Windows.Forms.DataGridView();
            this.Column1_nodeId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2_displacement = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3_sigmaX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4_sigmaY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5_tauXY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6_principal1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7_principal2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8_vonmises = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9_maxshear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button_ok = new System.Windows.Forms.Button();
            this.button_clearall = new System.Windows.Forms.Button();
            this.rectangleSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.circleSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ResultNodeList)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView_ResultNodeList
            // 
            this.dataGridView_ResultNodeList.AllowUserToAddRows = false;
            this.dataGridView_ResultNodeList.AllowUserToDeleteRows = false;
            this.dataGridView_ResultNodeList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_ResultNodeList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_ResultNodeList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1_nodeId,
            this.Column2_displacement,
            this.Column3_sigmaX,
            this.Column4_sigmaY,
            this.Column5_tauXY,
            this.Column6_principal1,
            this.Column7_principal2,
            this.Column8_vonmises,
            this.Column9_maxshear});
            this.dataGridView_ResultNodeList.Location = new System.Drawing.Point(13, 39);
            this.dataGridView_ResultNodeList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dataGridView_ResultNodeList.MultiSelect = false;
            this.dataGridView_ResultNodeList.Name = "dataGridView_ResultNodeList";
            this.dataGridView_ResultNodeList.ReadOnly = true;
            this.dataGridView_ResultNodeList.RowHeadersWidth = 62;
            this.dataGridView_ResultNodeList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView_ResultNodeList.Size = new System.Drawing.Size(933, 326);
            this.dataGridView_ResultNodeList.TabIndex = 22;
            // 
            // Column1_nodeId
            // 
            this.Column1_nodeId.FillWeight = 55F;
            this.Column1_nodeId.HeaderText = "Node ID";
            this.Column1_nodeId.MinimumWidth = 8;
            this.Column1_nodeId.Name = "Column1_nodeId";
            this.Column1_nodeId.ReadOnly = true;
            this.Column1_nodeId.Width = 55;
            // 
            // Column2_displacement
            // 
            this.Column2_displacement.HeaderText = "Displacement";
            this.Column2_displacement.MinimumWidth = 8;
            this.Column2_displacement.Name = "Column2_displacement";
            this.Column2_displacement.ReadOnly = true;
            // 
            // Column3_sigmaX
            // 
            this.Column3_sigmaX.HeaderText = "Stress X";
            this.Column3_sigmaX.MinimumWidth = 8;
            this.Column3_sigmaX.Name = "Column3_sigmaX";
            this.Column3_sigmaX.ReadOnly = true;
            this.Column3_sigmaX.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // Column4_sigmaY
            // 
            this.Column4_sigmaY.HeaderText = "Stress Y";
            this.Column4_sigmaY.MinimumWidth = 8;
            this.Column4_sigmaY.Name = "Column4_sigmaY";
            this.Column4_sigmaY.ReadOnly = true;
            this.Column4_sigmaY.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // Column5_tauXY
            // 
            this.Column5_tauXY.HeaderText = "Tau XY";
            this.Column5_tauXY.Name = "Column5_tauXY";
            this.Column5_tauXY.ReadOnly = true;
            // 
            // Column6_principal1
            // 
            this.Column6_principal1.HeaderText = "Principal Stress 1";
            this.Column6_principal1.Name = "Column6_principal1";
            this.Column6_principal1.ReadOnly = true;
            // 
            // Column7_principal2
            // 
            this.Column7_principal2.HeaderText = "Principal Stress 2";
            this.Column7_principal2.Name = "Column7_principal2";
            this.Column7_principal2.ReadOnly = true;
            // 
            // Column8_vonmises
            // 
            this.Column8_vonmises.HeaderText = "Von Mises";
            this.Column8_vonmises.Name = "Column8_vonmises";
            this.Column8_vonmises.ReadOnly = true;
            // 
            // Column9_maxshear
            // 
            this.Column9_maxshear.HeaderText = "Max Shear";
            this.Column9_maxshear.Name = "Column9_maxshear";
            this.Column9_maxshear.ReadOnly = true;
            // 
            // button_ok
            // 
            this.button_ok.Location = new System.Drawing.Point(301, 378);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(135, 48);
            this.button_ok.TabIndex = 23;
            this.button_ok.Text = "Ok";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // button_clearall
            // 
            this.button_clearall.Location = new System.Drawing.Point(521, 378);
            this.button_clearall.Name = "button_clearall";
            this.button_clearall.Size = new System.Drawing.Size(135, 48);
            this.button_clearall.TabIndex = 24;
            this.button_clearall.Text = "Clear All";
            this.button_clearall.UseVisualStyleBackColor = true;
            this.button_clearall.Click += new System.EventHandler(this.button_clearall_Click);
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
            // circleSelectionToolStripMenuItem
            // 
            this.circleSelectionToolStripMenuItem.Name = "circleSelectionToolStripMenuItem";
            this.circleSelectionToolStripMenuItem.Size = new System.Drawing.Size(100, 20);
            this.circleSelectionToolStripMenuItem.Text = "Circle Selection";
            this.circleSelectionToolStripMenuItem.Click += new System.EventHandler(this.circleSelectionToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rectangleSelectionToolStripMenuItem,
            this.circleSelectionToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(959, 24);
            this.menuStrip1.TabIndex = 43;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // annotate_frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(959, 436);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.button_clearall);
            this.Controls.Add(this.button_ok);
            this.Controls.Add(this.dataGridView_ResultNodeList);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(975, 475);
            this.Name = "annotate_frm";
            this.Opacity = 0.85D;
            this.Text = "Nodal Results Annotations";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.annotate_frm_FormClosing);
            this.Load += new System.EventHandler(this.annotate_frm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ResultNodeList)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView_ResultNodeList;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1_nodeId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2_displacement;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3_sigmaX;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4_sigmaY;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5_tauXY;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6_principal1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7_principal2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8_vonmises;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9_maxshear;
        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.Button button_clearall;
        private System.Windows.Forms.ToolStripMenuItem rectangleSelectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem circleSelectionToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
    }
}