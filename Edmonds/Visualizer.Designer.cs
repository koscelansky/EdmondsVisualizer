namespace Edmonds
{
    partial class Visualizer
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
            this.components = new System.ComponentModel.Container();
            this.MainPictureBox = new System.Windows.Forms.PictureBox();
            this.MainTextBox = new System.Windows.Forms.RichTextBox();
            this.Timer = new System.Windows.Forms.Timer(this.components);
            this.Go = new System.Windows.Forms.Button();
            this.Pause = new System.Windows.Forms.NumericUpDown();
            this.TimerPause = new System.Windows.Forms.Label();
            this.InitRandom = new System.Windows.Forms.Button();
            this.Size = new System.Windows.Forms.NumericUpDown();
            this.GraphSize = new System.Windows.Forms.Label();
            this.Reset = new System.Windows.Forms.Button();
            this.ControlPanel = new System.Windows.Forms.Panel();
            this.ShowVertces = new System.Windows.Forms.CheckBox();
            this.TrivialAlgorithm = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.MainPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Pause)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Size)).BeginInit();
            this.ControlPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainPictureBox
            // 
            this.MainPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainPictureBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.MainPictureBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.MainPictureBox.Location = new System.Drawing.Point(13, 13);
            this.MainPictureBox.Margin = new System.Windows.Forms.Padding(4);
            this.MainPictureBox.Name = "MainPictureBox";
            this.MainPictureBox.Size = new System.Drawing.Size(981, 597);
            this.MainPictureBox.TabIndex = 0;
            this.MainPictureBox.TabStop = false;
            this.MainPictureBox.Click += new System.EventHandler(this.MainPictureBox_Click);
            this.MainPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.MainPictureBox_Paint);
            this.MainPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainPictureBox_MouseDown);
            // 
            // MainTextBox
            // 
            this.MainTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainTextBox.Location = new System.Drawing.Point(13, 618);
            this.MainTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.MainTextBox.Name = "MainTextBox";
            this.MainTextBox.Size = new System.Drawing.Size(610, 99);
            this.MainTextBox.TabIndex = 1;
            this.MainTextBox.Text = "";
            // 
            // Timer
            // 
            this.Timer.Interval = 1000;
            this.Timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // Go
            // 
            this.Go.Location = new System.Drawing.Point(4, 38);
            this.Go.Margin = new System.Windows.Forms.Padding(4);
            this.Go.Name = "Go";
            this.Go.Size = new System.Drawing.Size(248, 28);
            this.Go.TabIndex = 3;
            this.Go.Text = "Go";
            this.Go.UseVisualStyleBackColor = true;
            this.Go.Click += new System.EventHandler(this.Go_Click);
            // 
            // Pause
            // 
            this.Pause.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.Pause.Location = new System.Drawing.Point(309, 42);
            this.Pause.Margin = new System.Windows.Forms.Padding(4);
            this.Pause.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.Pause.Name = "Pause";
            this.Pause.Size = new System.Drawing.Size(51, 22);
            this.Pause.TabIndex = 4;
            this.Pause.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // TimerPause
            // 
            this.TimerPause.AutoSize = true;
            this.TimerPause.Location = new System.Drawing.Point(257, 44);
            this.TimerPause.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.TimerPause.Name = "TimerPause";
            this.TimerPause.Size = new System.Drawing.Size(44, 17);
            this.TimerPause.TabIndex = 5;
            this.TimerPause.Text = "Timer";
            // 
            // InitRandom
            // 
            this.InitRandom.Location = new System.Drawing.Point(152, 4);
            this.InitRandom.Margin = new System.Windows.Forms.Padding(4);
            this.InitRandom.Name = "InitRandom";
            this.InitRandom.Size = new System.Drawing.Size(100, 28);
            this.InitRandom.TabIndex = 6;
            this.InitRandom.Text = "Random";
            this.InitRandom.UseVisualStyleBackColor = true;
            this.InitRandom.Click += new System.EventHandler(this.InitRandom_Click);
            // 
            // Size
            // 
            this.Size.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.Size.Location = new System.Drawing.Point(47, 7);
            this.Size.Margin = new System.Windows.Forms.Padding(4);
            this.Size.Maximum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.Size.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.Size.Name = "Size";
            this.Size.Size = new System.Drawing.Size(89, 22);
            this.Size.TabIndex = 7;
            this.Size.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // GraphSize
            // 
            this.GraphSize.AutoSize = true;
            this.GraphSize.Location = new System.Drawing.Point(4, 10);
            this.GraphSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.GraphSize.Name = "GraphSize";
            this.GraphSize.Size = new System.Drawing.Size(35, 17);
            this.GraphSize.TabIndex = 8;
            this.GraphSize.Text = "Size";
            // 
            // Reset
            // 
            this.Reset.Location = new System.Drawing.Point(260, 4);
            this.Reset.Margin = new System.Windows.Forms.Padding(4);
            this.Reset.Name = "Reset";
            this.Reset.Size = new System.Drawing.Size(100, 28);
            this.Reset.TabIndex = 10;
            this.Reset.Text = "Reset";
            this.Reset.UseVisualStyleBackColor = true;
            this.Reset.Click += new System.EventHandler(this.Reset_Click);
            // 
            // ControlPanel
            // 
            this.ControlPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ControlPanel.Controls.Add(this.TrivialAlgorithm);
            this.ControlPanel.Controls.Add(this.ShowVertces);
            this.ControlPanel.Controls.Add(this.Reset);
            this.ControlPanel.Controls.Add(this.Go);
            this.ControlPanel.Controls.Add(this.GraphSize);
            this.ControlPanel.Controls.Add(this.Pause);
            this.ControlPanel.Controls.Add(this.Size);
            this.ControlPanel.Controls.Add(this.TimerPause);
            this.ControlPanel.Controls.Add(this.InitRandom);
            this.ControlPanel.Location = new System.Drawing.Point(630, 618);
            this.ControlPanel.Name = "ControlPanel";
            this.ControlPanel.Size = new System.Drawing.Size(364, 99);
            this.ControlPanel.TabIndex = 11;
            // 
            // ShowVertces
            // 
            this.ShowVertces.AutoSize = true;
            this.ShowVertces.Location = new System.Drawing.Point(7, 73);
            this.ShowVertces.Name = "ShowVertces";
            this.ShowVertces.Size = new System.Drawing.Size(117, 21);
            this.ShowVertces.TabIndex = 11;
            this.ShowVertces.Text = "Show vertices";
            this.ShowVertces.UseVisualStyleBackColor = true;
            // 
            // TrivialAlgorithm
            // 
            this.TrivialAlgorithm.AutoSize = true;
            this.TrivialAlgorithm.Location = new System.Drawing.Point(130, 73);
            this.TrivialAlgorithm.Name = "TrivialAlgorithm";
            this.TrivialAlgorithm.Size = new System.Drawing.Size(174, 21);
            this.TrivialAlgorithm.TabIndex = 12;
            this.TrivialAlgorithm.Text = "Include trivial algorithm";
            this.TrivialAlgorithm.UseVisualStyleBackColor = true;
            // 
            // Visualizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 721);
            this.Controls.Add(this.ControlPanel);
            this.Controls.Add(this.MainTextBox);
            this.Controls.Add(this.MainPictureBox);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Visualizer";
            this.Text = "Edmonds";
            this.Load += new System.EventHandler(this.Visualizer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MainPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Pause)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Size)).EndInit();
            this.ControlPanel.ResumeLayout(false);
            this.ControlPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox MainPictureBox;
        private System.Windows.Forms.RichTextBox MainTextBox;
        private System.Windows.Forms.Timer Timer;
        private System.Windows.Forms.Button Go;
        private System.Windows.Forms.NumericUpDown Pause;
        private System.Windows.Forms.Label TimerPause;
        private System.Windows.Forms.Button InitRandom;
        private new System.Windows.Forms.NumericUpDown Size;
        private System.Windows.Forms.Label GraphSize;
        private System.Windows.Forms.Button Reset;
        private System.Windows.Forms.Panel ControlPanel;
        private System.Windows.Forms.CheckBox ShowVertces;
        private System.Windows.Forms.CheckBox TrivialAlgorithm;
    }
}