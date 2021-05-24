namespace TgenRemoter
{
    partial class Controller
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Controller));
            this.Tick = new System.Windows.Forms.Timer(this.components);
            this.ScreenSharePictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.ScreenSharePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // Tick
            // 
            this.Tick.Enabled = true;
            this.Tick.Interval = 33;
            this.Tick.Tick += new System.EventHandler(this.Tick_Tick);
            // 
            // ScreenSharePictureBox
            // 
            this.ScreenSharePictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ScreenSharePictureBox.Location = new System.Drawing.Point(12, 12);
            this.ScreenSharePictureBox.Name = "ScreenSharePictureBox";
            this.ScreenSharePictureBox.Size = new System.Drawing.Size(250, 250);
            this.ScreenSharePictureBox.TabIndex = 0;
            this.ScreenSharePictureBox.TabStop = false;
            this.ScreenSharePictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ScreenSharePictureBox_MouseClick);
            // 
            // Controller
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ScreenSharePictureBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Controller";
            this.Text = "Controller";
            this.Load += new System.EventHandler(this.Controller_Load);
            this.SizeChanged += new System.EventHandler(this.Controller_SizeChanged);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Controller_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Controller_DragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Controller_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.ScreenSharePictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer Tick;
        private System.Windows.Forms.PictureBox ScreenSharePictureBox;
    }
}