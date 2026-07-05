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
            this.checkBox_paintrsltmeshpoints = new System.Windows.Forms.CheckBox();
            this.checkBox_paintcontourlines = new System.Windows.Forms.CheckBox();
            this.checkBox_paintrsltmeshboundaries = new System.Windows.Forms.CheckBox();
            this.checkBox_paintrsltmesh = new System.Windows.Forms.CheckBox();
            this.comboBox_contourlevels = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label_status = new System.Windows.Forms.Label();
            this.button_stop = new System.Windows.Forms.Button();
            this.button_play_pause = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label_animation_speed = new System.Windows.Forms.Label();
            this.label_realtimeanim_speed = new System.Windows.Forms.Label();
            this.button_animation_speed = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // button_ok
            // 
            this.button_ok.Location = new System.Drawing.Point(295, 379);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(131, 45);
            this.button_ok.TabIndex = 5;
            this.button_ok.Text = "Ok";
            this.button_ok.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.trackBar1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.comboBox_contourlevels);
            this.groupBox1.Controls.Add(this.checkBox_paintrsltmeshpoints);
            this.groupBox1.Controls.Add(this.checkBox_paintcontourlines);
            this.groupBox1.Controls.Add(this.checkBox_paintrsltmeshboundaries);
            this.groupBox1.Controls.Add(this.checkBox_paintrsltmesh);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(315, 346);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Drawing Option";
            // 
            // checkBox_paintrsltmeshpoints
            // 
            this.checkBox_paintrsltmeshpoints.AutoSize = true;
            this.checkBox_paintrsltmeshpoints.Location = new System.Drawing.Point(31, 52);
            this.checkBox_paintrsltmeshpoints.Name = "checkBox_paintrsltmeshpoints";
            this.checkBox_paintrsltmeshpoints.Size = new System.Drawing.Size(188, 20);
            this.checkBox_paintrsltmeshpoints.TabIndex = 5;
            this.checkBox_paintrsltmeshpoints.Text = "Paint Result Mesh Points";
            this.checkBox_paintrsltmeshpoints.UseVisualStyleBackColor = true;
            // 
            // checkBox_paintcontourlines
            // 
            this.checkBox_paintcontourlines.AutoSize = true;
            this.checkBox_paintcontourlines.Location = new System.Drawing.Point(31, 167);
            this.checkBox_paintcontourlines.Name = "checkBox_paintcontourlines";
            this.checkBox_paintcontourlines.Size = new System.Drawing.Size(154, 20);
            this.checkBox_paintcontourlines.TabIndex = 4;
            this.checkBox_paintcontourlines.Text = "Paint Contour Lines";
            this.checkBox_paintcontourlines.UseVisualStyleBackColor = true;
            // 
            // checkBox_paintrsltmeshboundaries
            // 
            this.checkBox_paintrsltmeshboundaries.AutoSize = true;
            this.checkBox_paintrsltmeshboundaries.Location = new System.Drawing.Point(31, 128);
            this.checkBox_paintrsltmeshboundaries.Name = "checkBox_paintrsltmeshboundaries";
            this.checkBox_paintrsltmeshboundaries.Size = new System.Drawing.Size(219, 20);
            this.checkBox_paintrsltmeshboundaries.TabIndex = 2;
            this.checkBox_paintrsltmeshboundaries.Text = "Paint Result Mesh Boundaries";
            this.checkBox_paintrsltmeshboundaries.UseVisualStyleBackColor = true;
            // 
            // checkBox_paintrsltmesh
            // 
            this.checkBox_paintrsltmesh.AutoSize = true;
            this.checkBox_paintrsltmesh.Location = new System.Drawing.Point(31, 91);
            this.checkBox_paintrsltmesh.Name = "checkBox_paintrsltmesh";
            this.checkBox_paintrsltmesh.Size = new System.Drawing.Size(143, 20);
            this.checkBox_paintrsltmesh.TabIndex = 1;
            this.checkBox_paintrsltmesh.Text = "Paint Result Mesh";
            this.checkBox_paintrsltmesh.UseVisualStyleBackColor = true;
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
            this.comboBox_contourlevels.Location = new System.Drawing.Point(166, 207);
            this.comboBox_contourlevels.Name = "comboBox_contourlevels";
            this.comboBox_contourlevels.Size = new System.Drawing.Size(128, 24);
            this.comboBox_contourlevels.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(44, 210);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 16);
            this.label1.TabIndex = 18;
            this.label1.Text = "Contour Levels: ";
            // 
            // label_status
            // 
            this.label_status.AutoSize = true;
            this.label_status.Location = new System.Drawing.Point(351, 157);
            this.label_status.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_status.Name = "label_status";
            this.label_status.Size = new System.Drawing.Size(53, 16);
            this.label_status.TabIndex = 39;
            this.label_status.Text = "Playing";
            // 
            // button_stop
            // 
            this.button_stop.Location = new System.Drawing.Point(354, 99);
            this.button_stop.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_stop.Name = "button_stop";
            this.button_stop.Size = new System.Drawing.Size(155, 38);
            this.button_stop.TabIndex = 38;
            this.button_stop.Text = "Stop Animation";
            this.button_stop.UseVisualStyleBackColor = true;
            // 
            // button_play_pause
            // 
            this.button_play_pause.Location = new System.Drawing.Point(354, 54);
            this.button_play_pause.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_play_pause.Name = "button_play_pause";
            this.button_play_pause.Size = new System.Drawing.Size(155, 38);
            this.button_play_pause.TabIndex = 37;
            this.button_play_pause.Text = "Play Animation";
            this.button_play_pause.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label_animation_speed);
            this.groupBox2.Controls.Add(this.label_realtimeanim_speed);
            this.groupBox2.Controls.Add(this.button_animation_speed);
            this.groupBox2.Location = new System.Drawing.Point(355, 199);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Size = new System.Drawing.Size(357, 135);
            this.groupBox2.TabIndex = 36;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Animation Speed: ";
            // 
            // label_animation_speed
            // 
            this.label_animation_speed.AutoSize = true;
            this.label_animation_speed.Location = new System.Drawing.Point(252, 35);
            this.label_animation_speed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_animation_speed.Name = "label_animation_speed";
            this.label_animation_speed.Size = new System.Drawing.Size(28, 16);
            this.label_animation_speed.TabIndex = 2;
            this.label_animation_speed.Text = "1.0";
            // 
            // label_realtimeanim_speed
            // 
            this.label_realtimeanim_speed.AutoSize = true;
            this.label_realtimeanim_speed.Location = new System.Drawing.Point(7, 84);
            this.label_realtimeanim_speed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_realtimeanim_speed.Name = "label_realtimeanim_speed";
            this.label_realtimeanim_speed.Size = new System.Drawing.Size(309, 16);
            this.label_realtimeanim_speed.TabIndex = 1;
            this.label_realtimeanim_speed.Text = "1 second in real time = 1 second in Animation";
            // 
            // button_animation_speed
            // 
            this.button_animation_speed.Location = new System.Drawing.Point(10, 25);
            this.button_animation_speed.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_animation_speed.Name = "button_animation_speed";
            this.button_animation_speed.Size = new System.Drawing.Size(178, 37);
            this.button_animation_speed.TabIndex = 0;
            this.button_animation_speed.Text = "Animation Speed";
            this.button_animation_speed.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 258);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(146, 16);
            this.label2.TabIndex = 19;
            this.label2.Text = "Displacement Scale: ";
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(31, 286);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(219, 45);
            this.trackBar1.TabIndex = 20;
            // 
            // rsltoption_frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(724, 436);
            this.Controls.Add(this.label_status);
            this.Controls.Add(this.button_stop);
            this.Controls.Add(this.button_play_pause);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button_ok);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimumSize = new System.Drawing.Size(740, 475);
            this.Name = "rsltoption_frm";
            this.Text = "Result Options";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar trackBar1;
    }
}