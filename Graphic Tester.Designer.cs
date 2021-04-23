namespace TgenRemoter
{
    partial class Graphic_Tester
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
            this.screenSharePictureBox = new System.Windows.Forms.PictureBox();
            this.Tick = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.screenSharePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // screenSharePictureBox
            // 
            this.screenSharePictureBox.BackgroundImage = global::TgenRemoter.Properties.Resources.caricature_beginning_of_the_end;
            this.screenSharePictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.screenSharePictureBox.Location = new System.Drawing.Point(12, 3);
            this.screenSharePictureBox.Name = "screenSharePictureBox";
            this.screenSharePictureBox.Size = new System.Drawing.Size(250, 250);
            this.screenSharePictureBox.TabIndex = 0;
            this.screenSharePictureBox.TabStop = false;
            // 
            // Tick
            // 
            this.Tick.Enabled = true;
            // 
            // Graphic_Tester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.screenSharePictureBox);
            this.Name = "Graphic_Tester";
            this.Text = "Graphic_Tester";
            this.Load += new System.EventHandler(this.Graphic_Tester_Load);
            ((System.ComponentModel.ISupportInitialize)(this.screenSharePictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox screenSharePictureBox;
        private System.Windows.Forms.Timer Tick;
    }
}