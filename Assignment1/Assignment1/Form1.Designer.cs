namespace Assignment1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.UpdateLoopTimer = new System.Windows.Forms.Timer(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.DrawButton = new System.Windows.Forms.Button();
            this.EraseButton = new System.Windows.Forms.Button();
            this.BorderColorButton = new System.Windows.Forms.Button();
            this.FillColorButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.AddCircleButton = new System.Windows.Forms.Button();
            this.DrawCircleButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.NumericX = new System.Windows.Forms.NumericUpDown();
            this.NumericY = new System.Windows.Forms.NumericUpDown();
            this.NumericR = new System.Windows.Forms.NumericUpDown();
            this.BorderColorDialog = new System.Windows.Forms.ColorDialog();
            this.FillColorDialog = new System.Windows.Forms.ColorDialog();
            this.NumericWidth = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.ClearButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericWidth)).BeginInit();
            this.SuspendLayout();
            // 
            // UpdateLoopTimer
            // 
            this.UpdateLoopTimer.Enabled = true;
            this.UpdateLoopTimer.Interval = 16;
            this.UpdateLoopTimer.Tick += new System.EventHandler(this.UpdateLoopTimer_Tick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(800, 600);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // DrawButton
            // 
            this.DrawButton.Location = new System.Drawing.Point(831, 462);
            this.DrawButton.Name = "DrawButton";
            this.DrawButton.Size = new System.Drawing.Size(75, 23);
            this.DrawButton.TabIndex = 1;
            this.DrawButton.Text = "Draw all";
            this.DrawButton.UseVisualStyleBackColor = true;
            this.DrawButton.Click += new System.EventHandler(this.DrawButton_Click);
            // 
            // EraseButton
            // 
            this.EraseButton.Location = new System.Drawing.Point(831, 491);
            this.EraseButton.Name = "EraseButton";
            this.EraseButton.Size = new System.Drawing.Size(75, 23);
            this.EraseButton.TabIndex = 2;
            this.EraseButton.Text = "Erase all";
            this.EraseButton.UseVisualStyleBackColor = true;
            this.EraseButton.Click += new System.EventHandler(this.EraseButton_Click);
            // 
            // BorderColorButton
            // 
            this.BorderColorButton.BackColor = System.Drawing.Color.Black;
            this.BorderColorButton.Location = new System.Drawing.Point(831, 181);
            this.BorderColorButton.Name = "BorderColorButton";
            this.BorderColorButton.Size = new System.Drawing.Size(75, 23);
            this.BorderColorButton.TabIndex = 6;
            this.BorderColorButton.UseVisualStyleBackColor = false;
            this.BorderColorButton.Click += new System.EventHandler(this.BorderColorButton_Click);
            // 
            // FillColorButton
            // 
            this.FillColorButton.BackColor = System.Drawing.Color.Blue;
            this.FillColorButton.Location = new System.Drawing.Point(912, 181);
            this.FillColorButton.Name = "FillColorButton";
            this.FillColorButton.Size = new System.Drawing.Size(75, 23);
            this.FillColorButton.TabIndex = 7;
            this.FillColorButton.UseVisualStyleBackColor = false;
            this.FillColorButton.Click += new System.EventHandler(this.FillColorButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(912, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 15);
            this.label1.TabIndex = 8;
            this.label1.Text = "X";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(912, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 15);
            this.label2.TabIndex = 9;
            this.label2.Text = "Y";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(912, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 15);
            this.label3.TabIndex = 10;
            this.label3.Text = "Radius";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(834, 160);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 15);
            this.label4.TabIndex = 11;
            this.label4.Text = "Border color";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(927, 160);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 15);
            this.label5.TabIndex = 12;
            this.label5.Text = "Fill color";
            // 
            // AddCircleButton
            // 
            this.AddCircleButton.Location = new System.Drawing.Point(831, 225);
            this.AddCircleButton.Name = "AddCircleButton";
            this.AddCircleButton.Size = new System.Drawing.Size(75, 23);
            this.AddCircleButton.TabIndex = 13;
            this.AddCircleButton.Text = "Add";
            this.AddCircleButton.UseVisualStyleBackColor = true;
            this.AddCircleButton.Click += new System.EventHandler(this.AddCircleButton_Click);
            // 
            // DrawCircleButton
            // 
            this.DrawCircleButton.Location = new System.Drawing.Point(912, 225);
            this.DrawCircleButton.Name = "DrawCircleButton";
            this.DrawCircleButton.Size = new System.Drawing.Size(75, 23);
            this.DrawCircleButton.TabIndex = 14;
            this.DrawCircleButton.Text = "Draw";
            this.DrawCircleButton.UseVisualStyleBackColor = true;
            this.DrawCircleButton.Click += new System.EventHandler(this.DrawCircleButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(831, 207);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(157, 15);
            this.label6.TabIndex = 15;
            this.label6.Text = "------------------------------";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label7.Location = new System.Drawing.Point(831, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 25);
            this.label7.TabIndex = 16;
            this.label7.Text = "Circle";
            // 
            // NumericX
            // 
            this.NumericX.Location = new System.Drawing.Point(831, 37);
            this.NumericX.Maximum = new decimal(new int[] {
            800,
            0,
            0,
            0});
            this.NumericX.Name = "NumericX";
            this.NumericX.Size = new System.Drawing.Size(75, 23);
            this.NumericX.TabIndex = 17;
            // 
            // NumericY
            // 
            this.NumericY.Location = new System.Drawing.Point(831, 66);
            this.NumericY.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.NumericY.Name = "NumericY";
            this.NumericY.Size = new System.Drawing.Size(75, 23);
            this.NumericY.TabIndex = 18;
            // 
            // NumericR
            // 
            this.NumericR.Location = new System.Drawing.Point(831, 95);
            this.NumericR.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.NumericR.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericR.Name = "NumericR";
            this.NumericR.Size = new System.Drawing.Size(75, 23);
            this.NumericR.TabIndex = 19;
            this.NumericR.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // FillColorDialog
            // 
            this.FillColorDialog.Color = System.Drawing.Color.Blue;
            // 
            // NumericWidth
            // 
            this.NumericWidth.Location = new System.Drawing.Point(831, 124);
            this.NumericWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericWidth.Name = "NumericWidth";
            this.NumericWidth.Size = new System.Drawing.Size(75, 23);
            this.NumericWidth.TabIndex = 20;
            this.NumericWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(912, 126);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 15);
            this.label8.TabIndex = 21;
            this.label8.Text = "Border width";
            // 
            // ClearButton
            // 
            this.ClearButton.Location = new System.Drawing.Point(831, 520);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(75, 23);
            this.ClearButton.TabIndex = 22;
            this.ClearButton.Text = "Clear";
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1080, 601);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.NumericWidth);
            this.Controls.Add(this.NumericR);
            this.Controls.Add(this.NumericY);
            this.Controls.Add(this.NumericX);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.DrawCircleButton);
            this.Controls.Add(this.AddCircleButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FillColorButton);
            this.Controls.Add(this.BorderColorButton);
            this.Controls.Add(this.EraseButton);
            this.Controls.Add(this.DrawButton);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Assignment1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericWidth)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer UpdateLoopTimer;
        private PictureBox pictureBox1;
        private Button DrawButton;
        private Button EraseButton;
        private Button BorderColorButton;
        private Button FillColorButton;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Button AddCircleButton;
        private Button DrawCircleButton;
        private Label label6;
        private Label label7;
        private NumericUpDown NumericX;
        private NumericUpDown NumericY;
        private NumericUpDown NumericR;
        private ColorDialog BorderColorDialog;
        private ColorDialog FillColorDialog;
        private NumericUpDown NumericWidth;
        private Label label8;
        private Button ClearButton;
    }
}