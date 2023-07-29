namespace LDT___Engine_Development
{
    partial class Main
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
            components = new System.ComponentModel.Container();
            timer_main = new System.Windows.Forms.Timer(components);
            lb_display = new Label();
            pb_display = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pb_display).BeginInit();
            SuspendLayout();
            // 
            // timer_main
            // 
            timer_main.Interval = 10;
            timer_main.Tick += timer_main_Tick;
            // 
            // lb_display
            // 
            lb_display.AutoSize = true;
            lb_display.Location = new Point(657, 75);
            lb_display.Name = "lb_display";
            lb_display.Size = new Size(59, 15);
            lb_display.TabIndex = 0;
            lb_display.Text = "lb_display";
            lb_display.Visible = false;
            // 
            // pb_display
            // 
            pb_display.Dock = DockStyle.Fill;
            pb_display.Location = new Point(0, 0);
            pb_display.Name = "pb_display";
            pb_display.Size = new Size(1184, 761);
            pb_display.TabIndex = 1;
            pb_display.TabStop = false;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1184, 761);
            Controls.Add(lb_display);
            Controls.Add(pb_display);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "Main";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Limes Development Team - Engine";
            Load += Main_Load;
            KeyDown += Main_KeyDown;
            KeyPress += Main_KeyPress;
            KeyUp += Main_KeyUp;
            ((System.ComponentModel.ISupportInitialize)pb_display).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Timer timer_main;
        private Label lb_display;
        private PictureBox pb_display;
    }
}