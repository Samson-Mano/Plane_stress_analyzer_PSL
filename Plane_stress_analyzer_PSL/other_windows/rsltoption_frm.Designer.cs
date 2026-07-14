namespace other_windows
{
    partial class rsltoption_frm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(rsltoption_frm));
            this.button_ok = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label_deformation_scale = new System.Windows.Forms.Label();
            this.trackBar_deformation_scale = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox_contourlevels = new System.Windows.Forms.ComboBox();
            this.checkBox_paintrsltmeshpoints = new System.Windows.Forms.CheckBox();
            this.checkBox_paintcontourlines = new System.Windows.Forms.CheckBox();
            this.checkBox_paintrsltmeshboundaries = new System.Windows.Forms.CheckBox();
            this.checkBox_paintrsltmesh = new System.Windows.Forms.CheckBox();
            this.label_status = new System.Windows.Forms.Label();
            this.button_stop = new System.Windows.Forms.Button();
            this.button_play_pause = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label_animation_speed = new System.Windows.Forms.Label();
            this.label_realtimeanim_speed = new System.Windows.Forms.Label();
            this.button_animation_speed = new System.Windows.Forms.Button();
            this.checkBox_showtransparentmesh = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_deformation_scale)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_ok
            // 
            this.button_ok.Location = new System.Drawing.Point(271, 417);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(133, 52);
            this.button_ok.TabIndex = 5;
            this.button_ok.Text = "Ok";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox_showtransparentmesh);
            this.groupBox1.Controls.Add(this.label_deformation_scale);
            this.groupBox1.Controls.Add(this.trackBar_deformation_scale);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.comboBox_contourlevels);
            this.groupBox1.Controls.Add(this.checkBox_paintrsltmeshpoints);
            this.groupBox1.Controls.Add(this.checkBox_paintcontourlines);
            this.groupBox1.Controls.Add(this.checkBox_paintrsltmeshboundaries);
            this.groupBox1.Controls.Add(this.checkBox_paintrsltmesh);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(315, 389);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Drawing Option";
            // 
            // label_deformation_scale
            // 
            this.label_deformation_scale.AutoSize = true;
            this.label_deformation_scale.Location = new System.Drawing.Point(28, 301);
            this.label_deformation_scale.Name = "label_deformation_scale";
            this.label_deformation_scale.Size = new System.Drawing.Size(45, 16);
            this.label_deformation_scale.TabIndex = 21;
            this.label_deformation_scale.Text = "label3";
            // 
            // trackBar_deformation_scale
            // 
            this.trackBar_deformation_scale.Location = new System.Drawing.Point(20, 329);
            this.trackBar_deformation_scale.Name = "trackBar_deformation_scale";
            this.trackBar_deformation_scale.Size = new System.Drawing.Size(238, 45);
            this.trackBar_deformation_scale.TabIndex = 20;
            this.trackBar_deformation_scale.Scroll += new System.EventHandler(this.trackBar_deformation_scale_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(44, 230);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 16);
            this.label1.TabIndex = 18;
            this.label1.Text = "Contour Levels: ";
            // 
            // comboBox_contourlevels
            // 
            this.comboBox_contourlevels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_contourlevels.FormattingEnabled = true;
            this.comboBox_contourlevels.Items.AddRange(new object[] {
            "5",
            "10",
            "20",
            "40",
            "80"});
            this.comboBox_contourlevels.Location = new System.Drawing.Point(166, 227);
            this.comboBox_contourlevels.Name = "comboBox_contourlevels";
            this.comboBox_contourlevels.Size = new System.Drawing.Size(128, 24);
            this.comboBox_contourlevels.TabIndex = 17;
            this.comboBox_contourlevels.SelectedIndexChanged += new System.EventHandler(this.comboBox_contourlevels_SelectedIndexChanged);
            // 
            // checkBox_paintrsltmeshpoints
            // 
            this.checkBox_paintrsltmeshpoints.AutoSize = true;
            this.checkBox_paintrsltmeshpoints.Location = new System.Drawing.Point(31, 75);
            this.checkBox_paintrsltmeshpoints.Name = "checkBox_paintrsltmeshpoints";
            this.checkBox_paintrsltmeshpoints.Size = new System.Drawing.Size(188, 20);
            this.checkBox_paintrsltmeshpoints.TabIndex = 5;
            this.checkBox_paintrsltmeshpoints.Text = "Paint Result Mesh Points";
            this.checkBox_paintrsltmeshpoints.UseVisualStyleBackColor = true;
            this.checkBox_paintrsltmeshpoints.CheckedChanged += new System.EventHandler(this.checkBox_paintrsltmeshpoints_CheckedChanged);
            // 
            // checkBox_paintcontourlines
            // 
            this.checkBox_paintcontourlines.AutoSize = true;
            this.checkBox_paintcontourlines.Location = new System.Drawing.Point(31, 192);
            this.checkBox_paintcontourlines.Name = "checkBox_paintcontourlines";
            this.checkBox_paintcontourlines.Size = new System.Drawing.Size(154, 20);
            this.checkBox_paintcontourlines.TabIndex = 4;
            this.checkBox_paintcontourlines.Text = "Paint Contour Lines";
            this.checkBox_paintcontourlines.UseVisualStyleBackColor = true;
            this.checkBox_paintcontourlines.CheckedChanged += new System.EventHandler(this.checkBox_paintcontourlines_CheckedChanged);
            // 
            // checkBox_paintrsltmeshboundaries
            // 
            this.checkBox_paintrsltmeshboundaries.AutoSize = true;
            this.checkBox_paintrsltmeshboundaries.Location = new System.Drawing.Point(31, 153);
            this.checkBox_paintrsltmeshboundaries.Name = "checkBox_paintrsltmeshboundaries";
            this.checkBox_paintrsltmeshboundaries.Size = new System.Drawing.Size(219, 20);
            this.checkBox_paintrsltmeshboundaries.TabIndex = 2;
            this.checkBox_paintrsltmeshboundaries.Text = "Paint Result Mesh Boundaries";
            this.checkBox_paintrsltmeshboundaries.UseVisualStyleBackColor = true;
            this.checkBox_paintrsltmeshboundaries.CheckedChanged += new System.EventHandler(this.checkBox_paintrsltmeshboundaries_CheckedChanged);
            // 
            // checkBox_paintrsltmesh
            // 
            this.checkBox_paintrsltmesh.AutoSize = true;
            this.checkBox_paintrsltmesh.Location = new System.Drawing.Point(31, 114);
            this.checkBox_paintrsltmesh.Name = "checkBox_paintrsltmesh";
            this.checkBox_paintrsltmesh.Size = new System.Drawing.Size(143, 20);
            this.checkBox_paintrsltmesh.TabIndex = 1;
            this.checkBox_paintrsltmesh.Text = "Paint Result Mesh";
            this.checkBox_paintrsltmesh.UseVisualStyleBackColor = true;
            this.checkBox_paintrsltmesh.CheckedChanged += new System.EventHandler(this.checkBox_paintrsltmesh_CheckedChanged);
            // 
            // label_status
            // 
            this.label_status.AutoSize = true;
            this.label_status.Location = new System.Drawing.Point(351, 172);
            this.label_status.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_status.Name = "label_status";
            this.label_status.Size = new System.Drawing.Size(53, 16);
            this.label_status.TabIndex = 39;
            this.label_status.Text = "Playing";
            // 
            // button_stop
            // 
            this.button_stop.Location = new System.Drawing.Point(354, 114);
            this.button_stop.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_stop.Name = "button_stop";
            this.button_stop.Size = new System.Drawing.Size(155, 38);
            this.button_stop.TabIndex = 38;
            this.button_stop.Text = "Stop Animation";
            this.button_stop.UseVisualStyleBackColor = true;
            this.button_stop.Click += new System.EventHandler(this.button_stop_Click);
            // 
            // button_play_pause
            // 
            this.button_play_pause.Location = new System.Drawing.Point(354, 69);
            this.button_play_pause.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_play_pause.Name = "button_play_pause";
            this.button_play_pause.Size = new System.Drawing.Size(155, 38);
            this.button_play_pause.TabIndex = 37;
            this.button_play_pause.Text = "Play Animation";
            this.button_play_pause.UseVisualStyleBackColor = true;
            this.button_play_pause.Click += new System.EventHandler(this.button_play_pause_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label_animation_speed);
            this.groupBox2.Controls.Add(this.label_realtimeanim_speed);
            this.groupBox2.Controls.Add(this.button_animation_speed);
            this.groupBox2.Location = new System.Drawing.Point(354, 218);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Size = new System.Drawing.Size(357, 183);
            this.groupBox2.TabIndex = 36;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Animation Speed: ";
            // 
            // label_animation_speed
            // 
            this.label_animation_speed.AutoSize = true;
            this.label_animation_speed.Location = new System.Drawing.Point(252, 44);
            this.label_animation_speed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_animation_speed.Name = "label_animation_speed";
            this.label_animation_speed.Size = new System.Drawing.Size(28, 16);
            this.label_animation_speed.TabIndex = 2;
            this.label_animation_speed.Text = "1.0";
            // 
            // label_realtimeanim_speed
            // 
            this.label_realtimeanim_speed.AutoSize = true;
            this.label_realtimeanim_speed.Location = new System.Drawing.Point(7, 93);
            this.label_realtimeanim_speed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_realtimeanim_speed.Name = "label_realtimeanim_speed";
            this.label_realtimeanim_speed.Size = new System.Drawing.Size(309, 16);
            this.label_realtimeanim_speed.TabIndex = 1;
            this.label_realtimeanim_speed.Text = "1 second in real time = 1 second in Animation";
            // 
            // button_animation_speed
            // 
            this.button_animation_speed.Location = new System.Drawing.Point(10, 34);
            this.button_animation_speed.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_animation_speed.Name = "button_animation_speed";
            this.button_animation_speed.Size = new System.Drawing.Size(178, 37);
            this.button_animation_speed.TabIndex = 0;
            this.button_animation_speed.Text = "Animation Speed";
            this.button_animation_speed.UseVisualStyleBackColor = true;
            this.button_animation_speed.Click += new System.EventHandler(this.button_animation_speed_Click);
            // 
            // checkBox_showtransparentmesh
            // 
            this.checkBox_showtransparentmesh.AutoSize = true;
            this.checkBox_showtransparentmesh.Location = new System.Drawing.Point(31, 36);
            this.checkBox_showtransparentmesh.Name = "checkBox_showtransparentmesh";
            this.checkBox_showtransparentmesh.Size = new System.Drawing.Size(144, 20);
            this.checkBox_showtransparentmesh.TabIndex = 22;
            this.checkBox_showtransparentmesh.Text = "Show Model Mesh";
            this.checkBox_showtransparentmesh.UseVisualStyleBackColor = true;
            this.checkBox_showtransparentmesh.CheckedChanged += new System.EventHandler(this.checkBox_showtransparentmesh_CheckedChanged);
            // 
            // rsltoption_frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(724, 481);
            this.Controls.Add(this.label_status);
            this.Controls.Add(this.button_stop);
            this.Controls.Add(this.button_play_pause);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button_ok);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(740, 520);
            this.Name = "rsltoption_frm";
            this.Opacity = 0.85D;
            this.Text = "Result Options";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.rsltoption_frm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_deformation_scale)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox_paintrsltmeshpoints;
        private System.Windows.Forms.CheckBox checkBox_paintcontourlines;
        private System.Windows.Forms.CheckBox checkBox_paintrsltmeshboundaries;
        private System.Windows.Forms.CheckBox checkBox_paintrsltmesh;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox_contourlevels;
        private System.Windows.Forms.Label label_status;
        private System.Windows.Forms.Button button_stop;
        private System.Windows.Forms.Button button_play_pause;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label_animation_speed;
        private System.Windows.Forms.Label label_realtimeanim_speed;
        private System.Windows.Forms.Button button_animation_speed;
        private System.Windows.Forms.TrackBar trackBar_deformation_scale;
        private System.Windows.Forms.Label label_deformation_scale;
        private System.Windows.Forms.CheckBox checkBox_showtransparentmesh;
    }
}