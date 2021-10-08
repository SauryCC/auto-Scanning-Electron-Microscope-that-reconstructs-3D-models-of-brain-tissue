namespace MIcrotome_GUI
{
    partial class Form_NavigationBuild
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
            this.label_Ypos = new System.Windows.Forms.Label();
            this.label_Xpos = new System.Windows.Forms.Label();
            this.label_W = new System.Windows.Forms.Label();
            this.label_L = new System.Windows.Forms.Label();
            this.label_Pixel = new System.Windows.Forms.Label();
            this.button_Navi_start = new System.Windows.Forms.Button();
            this.button_NaviValid = new System.Windows.Forms.Button();
            this.label_validResult = new System.Windows.Forms.Label();
            this.label_area = new System.Windows.Forms.Label();
            this.label_resolu = new System.Windows.Forms.Label();
            this.button_Navi_abort = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.numericalUpDown_XCen = new System.Windows.Forms.NumericUpDown();
            this.numericalUpDown_Length = new System.Windows.Forms.NumericUpDown();
            this.numericalUpDown_Width = new System.Windows.Forms.NumericUpDown();
            this.numericalUpDown_Pixel = new System.Windows.Forms.NumericUpDown();
            this.numericalUpDown_Mag = new System.Windows.Forms.NumericUpDown();
            this.numericalUpDown_YCen = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericalUpDown_XCen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericalUpDown_Length)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericalUpDown_Width)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericalUpDown_Pixel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericalUpDown_Mag)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericalUpDown_YCen)).BeginInit();
            this.SuspendLayout();
            // 
            // label_Ypos
            // 
            this.label_Ypos.AutoSize = true;
            this.label_Ypos.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F);
            this.label_Ypos.Location = new System.Drawing.Point(129, 69);
            this.label_Ypos.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_Ypos.Name = "label_Ypos";
            this.label_Ypos.Size = new System.Drawing.Size(146, 25);
            this.label_Ypos.TabIndex = 24;
            this.label_Ypos.Text = "Center Y (um)";
            // 
            // label_Xpos
            // 
            this.label_Xpos.AutoSize = true;
            this.label_Xpos.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F);
            this.label_Xpos.Location = new System.Drawing.Point(129, 25);
            this.label_Xpos.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_Xpos.Name = "label_Xpos";
            this.label_Xpos.Size = new System.Drawing.Size(145, 25);
            this.label_Xpos.TabIndex = 22;
            this.label_Xpos.Text = "Center X (um)";
            // 
            // label_W
            // 
            this.label_W.AutoSize = true;
            this.label_W.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F);
            this.label_W.Location = new System.Drawing.Point(129, 158);
            this.label_W.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_W.Name = "label_W";
            this.label_W.Size = new System.Drawing.Size(116, 25);
            this.label_W.TabIndex = 28;
            this.label_W.Text = "Width (um)";
            // 
            // label_L
            // 
            this.label_L.AutoSize = true;
            this.label_L.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F);
            this.label_L.Location = new System.Drawing.Point(129, 114);
            this.label_L.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_L.Name = "label_L";
            this.label_L.Size = new System.Drawing.Size(127, 25);
            this.label_L.TabIndex = 26;
            this.label_L.Text = "Length (um)";
            // 
            // label_Pixel
            // 
            this.label_Pixel.AutoSize = true;
            this.label_Pixel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F);
            this.label_Pixel.Location = new System.Drawing.Point(129, 203);
            this.label_Pixel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_Pixel.Name = "label_Pixel";
            this.label_Pixel.Size = new System.Drawing.Size(129, 25);
            this.label_Pixel.TabIndex = 30;
            this.label_Pixel.Text = "nm per pixel";
            // 
            // button_Navi_start
            // 
            this.button_Navi_start.Enabled = false;
            this.button_Navi_start.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
            this.button_Navi_start.Location = new System.Drawing.Point(174, 468);
            this.button_Navi_start.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button_Navi_start.Name = "button_Navi_start";
            this.button_Navi_start.Size = new System.Drawing.Size(136, 42);
            this.button_Navi_start.TabIndex = 31;
            this.button_Navi_start.Text = "Start";
            this.button_Navi_start.UseVisualStyleBackColor = true;
            this.button_Navi_start.Click += new System.EventHandler(this.button_Navi_start_Click);
            // 
            // button_NaviValid
            // 
            this.button_NaviValid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
            this.button_NaviValid.Location = new System.Drawing.Point(174, 417);
            this.button_NaviValid.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button_NaviValid.Name = "button_NaviValid";
            this.button_NaviValid.Size = new System.Drawing.Size(136, 42);
            this.button_NaviValid.TabIndex = 32;
            this.button_NaviValid.Text = "Valid";
            this.button_NaviValid.UseVisualStyleBackColor = true;
            this.button_NaviValid.Click += new System.EventHandler(this.button_NaviValid_Click);
            // 
            // label_validResult
            // 
            this.label_validResult.AutoSize = true;
            this.label_validResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F);
            this.label_validResult.Location = new System.Drawing.Point(129, 283);
            this.label_validResult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_validResult.Name = "label_validResult";
            this.label_validResult.Size = new System.Drawing.Size(129, 25);
            this.label_validResult.TabIndex = 33;
            this.label_validResult.Text = "Image grids:";
            // 
            // label_area
            // 
            this.label_area.AutoSize = true;
            this.label_area.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F);
            this.label_area.Location = new System.Drawing.Point(129, 325);
            this.label_area.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_area.Name = "label_area";
            this.label_area.Size = new System.Drawing.Size(69, 25);
            this.label_area.TabIndex = 34;
            this.label_area.Text = "Area: ";
            // 
            // label_resolu
            // 
            this.label_resolu.AutoSize = true;
            this.label_resolu.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F);
            this.label_resolu.Location = new System.Drawing.Point(129, 366);
            this.label_resolu.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_resolu.Name = "label_resolu";
            this.label_resolu.Size = new System.Drawing.Size(165, 25);
            this.label_resolu.TabIndex = 35;
            this.label_resolu.Text = "Pixel resolution:";
            // 
            // button_Navi_abort
            // 
            this.button_Navi_abort.Enabled = false;
            this.button_Navi_abort.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
            this.button_Navi_abort.Location = new System.Drawing.Point(174, 519);
            this.button_Navi_abort.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button_Navi_abort.Name = "button_Navi_abort";
            this.button_Navi_abort.Size = new System.Drawing.Size(136, 42);
            this.button_Navi_abort.TabIndex = 36;
            this.button_Navi_abort.Text = "Abort";
            this.button_Navi_abort.UseVisualStyleBackColor = true;
            this.button_Navi_abort.Click += new System.EventHandler(this.button_Navi_abort_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F);
            this.label1.Location = new System.Drawing.Point(129, 244);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 25);
            this.label1.TabIndex = 38;
            this.label1.Text = "Magnification";
            // 
            // numericalUpDown_XCen
            // 
            this.numericalUpDown_XCen.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F);
            this.numericalUpDown_XCen.Location = new System.Drawing.Point(282, 25);
            this.numericalUpDown_XCen.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericalUpDown_XCen.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericalUpDown_XCen.Name = "numericalUpDown_XCen";
            this.numericalUpDown_XCen.Size = new System.Drawing.Size(139, 31);
            this.numericalUpDown_XCen.TabIndex = 39;
            this.numericalUpDown_XCen.Value = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            // 
            // numericalUpDown_Length
            // 
            this.numericalUpDown_Length.DecimalPlaces = 3;
            this.numericalUpDown_Length.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F);
            this.numericalUpDown_Length.Location = new System.Drawing.Point(283, 114);
            this.numericalUpDown_Length.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericalUpDown_Length.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericalUpDown_Length.Name = "numericalUpDown_Length";
            this.numericalUpDown_Length.Size = new System.Drawing.Size(139, 31);
            this.numericalUpDown_Length.TabIndex = 41;
            this.numericalUpDown_Length.Value = new decimal(new int[] {
            19304,
            0,
            0,
            131072});
            this.numericalUpDown_Length.ValueChanged += new System.EventHandler(this.numericalUpDown_Length_ValueChanged);
            // 
            // numericalUpDown_Width
            // 
            this.numericalUpDown_Width.DecimalPlaces = 3;
            this.numericalUpDown_Width.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F);
            this.numericalUpDown_Width.Location = new System.Drawing.Point(282, 158);
            this.numericalUpDown_Width.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericalUpDown_Width.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.numericalUpDown_Width.Name = "numericalUpDown_Width";
            this.numericalUpDown_Width.Size = new System.Drawing.Size(139, 31);
            this.numericalUpDown_Width.TabIndex = 42;
            this.numericalUpDown_Width.Value = new decimal(new int[] {
            135128,
            0,
            0,
            196608});
            this.numericalUpDown_Width.ValueChanged += new System.EventHandler(this.numericalUpDown_Width_ValueChanged);
            // 
            // numericalUpDown_Pixel
            // 
            this.numericalUpDown_Pixel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F);
            this.numericalUpDown_Pixel.Location = new System.Drawing.Point(283, 203);
            this.numericalUpDown_Pixel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericalUpDown_Pixel.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.numericalUpDown_Pixel.Name = "numericalUpDown_Pixel";
            this.numericalUpDown_Pixel.Size = new System.Drawing.Size(139, 31);
            this.numericalUpDown_Pixel.TabIndex = 43;
            this.numericalUpDown_Pixel.Value = new decimal(new int[] {
            127,
            0,
            0,
            0});
            this.numericalUpDown_Pixel.ValueChanged += new System.EventHandler(this.numericalUpDown_Pixel_ValueChanged);
            // 
            // numericalUpDown_Mag
            // 
            this.numericalUpDown_Mag.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F);
            this.numericalUpDown_Mag.Location = new System.Drawing.Point(283, 244);
            this.numericalUpDown_Mag.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericalUpDown_Mag.Maximum = new decimal(new int[] {
            8000000,
            0,
            0,
            0});
            this.numericalUpDown_Mag.Minimum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.numericalUpDown_Mag.Name = "numericalUpDown_Mag";
            this.numericalUpDown_Mag.Size = new System.Drawing.Size(139, 31);
            this.numericalUpDown_Mag.TabIndex = 44;
            this.numericalUpDown_Mag.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.numericalUpDown_Mag.ValueChanged += new System.EventHandler(this.numericalUpDown_Mag_ValueChanged);
            // 
            // numericalUpDown_YCen
            // 
            this.numericalUpDown_YCen.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F);
            this.numericalUpDown_YCen.Location = new System.Drawing.Point(282, 69);
            this.numericalUpDown_YCen.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericalUpDown_YCen.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericalUpDown_YCen.Name = "numericalUpDown_YCen";
            this.numericalUpDown_YCen.Size = new System.Drawing.Size(139, 31);
            this.numericalUpDown_YCen.TabIndex = 45;
            this.numericalUpDown_YCen.Value = new decimal(new int[] {
            25000,
            0,
            0,
            0});
            // 
            // Form_NavigationBuild
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 632);
            this.Controls.Add(this.numericalUpDown_YCen);
            this.Controls.Add(this.numericalUpDown_Mag);
            this.Controls.Add(this.numericalUpDown_Pixel);
            this.Controls.Add(this.numericalUpDown_Width);
            this.Controls.Add(this.numericalUpDown_Length);
            this.Controls.Add(this.numericalUpDown_XCen);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_Navi_abort);
            this.Controls.Add(this.label_resolu);
            this.Controls.Add(this.label_area);
            this.Controls.Add(this.label_validResult);
            this.Controls.Add(this.button_NaviValid);
            this.Controls.Add(this.button_Navi_start);
            this.Controls.Add(this.label_Pixel);
            this.Controls.Add(this.label_W);
            this.Controls.Add(this.label_L);
            this.Controls.Add(this.label_Ypos);
            this.Controls.Add(this.label_Xpos);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form_NavigationBuild";
            ((System.ComponentModel.ISupportInitialize)(this.numericalUpDown_XCen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericalUpDown_Length)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericalUpDown_Width)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericalUpDown_Pixel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericalUpDown_Mag)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericalUpDown_YCen)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_Ypos;
        private System.Windows.Forms.Label label_Xpos;
        private System.Windows.Forms.Label label_W;
        private System.Windows.Forms.Label label_L;
        private System.Windows.Forms.Label label_Pixel;
        private System.Windows.Forms.Button button_Navi_start;
        private System.Windows.Forms.Button button_NaviValid;
        private System.Windows.Forms.Label label_validResult;
        private System.Windows.Forms.Label label_area;
        private System.Windows.Forms.Label label_resolu;
        private System.Windows.Forms.Button button_Navi_abort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericalUpDown_XCen;
        private System.Windows.Forms.NumericUpDown numericalUpDown_Length;
        private System.Windows.Forms.NumericUpDown numericalUpDown_Width;
        private System.Windows.Forms.NumericUpDown numericalUpDown_Pixel;
        private System.Windows.Forms.NumericUpDown numericalUpDown_Mag;
        private System.Windows.Forms.NumericUpDown numericalUpDown_YCen;
    }
}