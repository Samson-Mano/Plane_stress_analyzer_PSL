namespace Plane_stress_analyzer_PSL.other_windows
{
    partial class constraint_frm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(constraint_frm));
            this.dataGridView_ConstraintList = new System.Windows.Forms.DataGridView();
            this.Column1_constraintid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2_nodeids = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3_constrainttype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4_constraintangle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.circleSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rectangleSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.button_deleteconstraint = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_constraintangle = new System.Windows.Forms.TextBox();
            this.comboBox_constrainttype = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button_applyconstraint = new System.Windows.Forms.Button();
            this.textBox_selectednodes = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ConstraintList)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView_ConstraintList
            // 
            this.dataGridView_ConstraintList.AllowUserToAddRows = false;
            this.dataGridView_ConstraintList.AllowUserToDeleteRows = false;
            this.dataGridView_ConstraintList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_ConstraintList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_ConstraintList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1_constraintid,
            this.Column2_nodeids,
            this.Column3_constrainttype,
            this.Column4_constraintangle});
            this.dataGridView_ConstraintList.Location = new System.Drawing.Point(332, 27);
            this.dataGridView_ConstraintList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dataGridView_ConstraintList.MultiSelect = false;
            this.dataGridView_ConstraintList.Name = "dataGridView_ConstraintList";
            this.dataGridView_ConstraintList.ReadOnly = true;
            this.dataGridView_ConstraintList.RowHeadersWidth = 62;
            this.dataGridView_ConstraintList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView_ConstraintList.Size = new System.Drawing.Size(439, 251);
            this.dataGridView_ConstraintList.TabIndex = 14;
            // 
            // Column1_constraintid
            // 
            this.Column1_constraintid.FillWeight = 80F;
            this.Column1_constraintid.HeaderText = "Node Constraint ID";
            this.Column1_constraintid.MinimumWidth = 8;
            this.Column1_constraintid.Name = "Column1_constraintid";
            this.Column1_constraintid.ReadOnly = true;
            this.Column1_constraintid.Width = 80;
            // 
            // Column2_nodeids
            // 
            this.Column2_nodeids.HeaderText = "Node IDs";
            this.Column2_nodeids.MinimumWidth = 8;
            this.Column2_nodeids.Name = "Column2_nodeids";
            this.Column2_nodeids.ReadOnly = true;
            this.Column2_nodeids.Width = 125;
            // 
            // Column3_constrainttype
            // 
            this.Column3_constrainttype.FillWeight = 80F;
            this.Column3_constrainttype.HeaderText = "Constraint Type";
            this.Column3_constrainttype.MinimumWidth = 8;
            this.Column3_constrainttype.Name = "Column3_constrainttype";
            this.Column3_constrainttype.ReadOnly = true;
            this.Column3_constrainttype.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column3_constrainttype.Width = 80;
            // 
            // Column4_constraintangle
            // 
            this.Column4_constraintangle.FillWeight = 80F;
            this.Column4_constraintangle.HeaderText = "Constraint Angle";
            this.Column4_constraintangle.MinimumWidth = 8;
            this.Column4_constraintangle.Name = "Column4_constraintangle";
            this.Column4_constraintangle.ReadOnly = true;
            this.Column4_constraintangle.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column4_constraintangle.Width = 80;
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
            this.menuStrip1.TabIndex = 13;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // button_deleteconstraint
            // 
            this.button_deleteconstraint.Location = new System.Drawing.Point(581, 300);
            this.button_deleteconstraint.Name = "button_deleteconstraint";
            this.button_deleteconstraint.Size = new System.Drawing.Size(130, 35);
            this.button_deleteconstraint.TabIndex = 12;
            this.button_deleteconstraint.Text = "Delete Constraint";
            this.button_deleteconstraint.UseVisualStyleBackColor = true;
            this.button_deleteconstraint.Click += new System.EventHandler(this.button_deleteconstraint_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 138);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 16);
            this.label3.TabIndex = 9;
            this.label3.Text = "Selected Nodes: ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_constraintangle);
            this.groupBox1.Controls.Add(this.comboBox_constrainttype);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(313, 105);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Nodal Constraint Data: ";
            // 
            // textBox_constraintangle
            // 
            this.textBox_constraintangle.Location = new System.Drawing.Point(160, 63);
            this.textBox_constraintangle.Name = "textBox_constraintangle";
            this.textBox_constraintangle.Size = new System.Drawing.Size(100, 23);
            this.textBox_constraintangle.TabIndex = 5;
            // 
            // comboBox_constrainttype
            // 
            this.comboBox_constrainttype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_constrainttype.FormattingEnabled = true;
            this.comboBox_constrainttype.Items.AddRange(new object[] {
            "Pinned",
            "Roller"});
            this.comboBox_constrainttype.Location = new System.Drawing.Point(160, 26);
            this.comboBox_constrainttype.Name = "comboBox_constrainttype";
            this.comboBox_constrainttype.Size = new System.Drawing.Size(121, 24);
            this.comboBox_constrainttype.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Constraint Angle: ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Constraint Type: ";
            // 
            // button_applyconstraint
            // 
            this.button_applyconstraint.Location = new System.Drawing.Point(399, 300);
            this.button_applyconstraint.Name = "button_applyconstraint";
            this.button_applyconstraint.Size = new System.Drawing.Size(134, 35);
            this.button_applyconstraint.TabIndex = 11;
            this.button_applyconstraint.Text = "Apply Constraint";
            this.button_applyconstraint.UseVisualStyleBackColor = true;
            this.button_applyconstraint.Click += new System.EventHandler(this.button_applyconstraint_Click);
            // 
            // textBox_selectednodes
            // 
            this.textBox_selectednodes.Location = new System.Drawing.Point(12, 156);
            this.textBox_selectednodes.Multiline = true;
            this.textBox_selectednodes.Name = "textBox_selectednodes";
            this.textBox_selectednodes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_selectednodes.Size = new System.Drawing.Size(313, 193);
            this.textBox_selectednodes.TabIndex = 10;
            // 
            // constraint_frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 361);
            this.Controls.Add(this.dataGridView_ConstraintList);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.button_deleteconstraint);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_applyconstraint);
            this.Controls.Add(this.textBox_selectednodes);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(800, 400);
            this.Name = "constraint_frm";
            this.Opacity = 0.85D;
            this.Text = "Nodal Constraints";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.constraint_frm_FormClosing);
            this.Load += new System.EventHandler(this.constraint_frm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ConstraintList)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView_ConstraintList;
        private System.Windows.Forms.ToolStripMenuItem circleSelectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rectangleSelectionToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Button button_deleteconstraint;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_applyconstraint;
        private System.Windows.Forms.TextBox textBox_selectednodes;
        private System.Windows.Forms.TextBox textBox_constraintangle;
        private System.Windows.Forms.ComboBox comboBox_constrainttype;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1_constraintid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2_nodeids;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3_constrainttype;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4_constraintangle;
    }
}