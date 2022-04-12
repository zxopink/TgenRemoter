namespace TgenRemoter
{
    partial class Menu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Menu));
            this.partnerIp = new System.Windows.Forms.TextBox();
            this.Connect = new System.Windows.Forms.Button();
            this.CreditsLabel = new System.Windows.Forms.Label();
            this.CheckFileTransformation = new System.Windows.Forms.CheckBox();
            this.FilePathBtn = new System.Windows.Forms.Button();
            this.PassLabel = new System.Windows.Forms.Label();
            this.CopyPassBtn = new System.Windows.Forms.Button();
            this.FilesInfoBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.inConnCheckBox = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.NetTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // partnerIp
            // 
            this.partnerIp.Location = new System.Drawing.Point(210, 60);
            this.partnerIp.Name = "partnerIp";
            this.partnerIp.Size = new System.Drawing.Size(100, 20);
            this.partnerIp.TabIndex = 1;
            // 
            // Connect
            // 
            this.Connect.Location = new System.Drawing.Point(223, 86);
            this.Connect.Name = "Connect";
            this.Connect.Size = new System.Drawing.Size(75, 23);
            this.Connect.TabIndex = 2;
            this.Connect.Text = "Connect";
            this.Connect.UseVisualStyleBackColor = true;
            this.Connect.Click += new System.EventHandler(this.Connect_Click);
            // 
            // CreditsLabel
            // 
            this.CreditsLabel.AutoSize = true;
            this.CreditsLabel.Location = new System.Drawing.Point(13, 13);
            this.CreditsLabel.Name = "CreditsLabel";
            this.CreditsLabel.Size = new System.Drawing.Size(101, 13);
            this.CreditsLabel.TabIndex = 9;
            this.CreditsLabel.Text = "Made by Yoav Haik";
            // 
            // CheckFileTransformation
            // 
            this.CheckFileTransformation.AutoSize = true;
            this.CheckFileTransformation.Checked = true;
            this.CheckFileTransformation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckFileTransformation.Location = new System.Drawing.Point(16, 181);
            this.CheckFileTransformation.Name = "CheckFileTransformation";
            this.CheckFileTransformation.Size = new System.Drawing.Size(115, 17);
            this.CheckFileTransformation.TabIndex = 10;
            this.CheckFileTransformation.Text = "Files Tranformation";
            this.CheckFileTransformation.UseVisualStyleBackColor = true;
            this.CheckFileTransformation.CheckedChanged += new System.EventHandler(this.CheckFileTransformation_CheckedChanged);
            // 
            // FilePathBtn
            // 
            this.FilePathBtn.Location = new System.Drawing.Point(16, 205);
            this.FilePathBtn.Name = "FilePathBtn";
            this.FilePathBtn.Size = new System.Drawing.Size(110, 23);
            this.FilePathBtn.TabIndex = 11;
            this.FilePathBtn.Text = "Folder Target";
            this.FilePathBtn.UseVisualStyleBackColor = true;
            this.FilePathBtn.Click += new System.EventHandler(this.ButtonFilePath_Click);
            // 
            // PassLabel
            // 
            this.PassLabel.AutoSize = true;
            this.PassLabel.Location = new System.Drawing.Point(12, 65);
            this.PassLabel.Name = "PassLabel";
            this.PassLabel.Size = new System.Drawing.Size(35, 13);
            this.PassLabel.TabIndex = 12;
            this.PassLabel.Text = "Code:";
            // 
            // CopyPassBtn
            // 
            this.CopyPassBtn.Location = new System.Drawing.Point(80, 59);
            this.CopyPassBtn.Name = "CopyPassBtn";
            this.CopyPassBtn.Size = new System.Drawing.Size(75, 23);
            this.CopyPassBtn.TabIndex = 13;
            this.CopyPassBtn.Text = "Copy";
            this.CopyPassBtn.UseVisualStyleBackColor = true;
            this.CopyPassBtn.Click += new System.EventHandler(this.CopyPassBtn_Click);
            // 
            // FilesInfoBtn
            // 
            this.FilesInfoBtn.Location = new System.Drawing.Point(16, 234);
            this.FilesInfoBtn.Name = "FilesInfoBtn";
            this.FilesInfoBtn.Size = new System.Drawing.Size(75, 23);
            this.FilesInfoBtn.TabIndex = 14;
            this.FilesInfoBtn.Text = "Note";
            this.FilesInfoBtn.UseVisualStyleBackColor = true;
            this.FilesInfoBtn.Click += new System.EventHandler(this.FilesInfoBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(207, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Partner\'s code:";
            // 
            // inConnCheckBox
            // 
            this.inConnCheckBox.AutoSize = true;
            this.inConnCheckBox.Checked = true;
            this.inConnCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.inConnCheckBox.Location = new System.Drawing.Point(16, 92);
            this.inConnCheckBox.Name = "inConnCheckBox";
            this.inConnCheckBox.Size = new System.Drawing.Size(157, 17);
            this.inConnCheckBox.TabIndex = 16;
            this.inConnCheckBox.Text = "Allow incoming connections";
            this.inConnCheckBox.UseVisualStyleBackColor = true;
            this.inConnCheckBox.Visible = false;
            // 
            // toolTip
            // 
            this.toolTip.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip_Popup);
            // 
            // NetTimer
            // 
            this.NetTimer.Enabled = true;
            this.NetTimer.Tick += new System.EventHandler(this.NetTimer_Tick);
            // 
            // Menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(337, 274);
            this.Controls.Add(this.inConnCheckBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FilesInfoBtn);
            this.Controls.Add(this.CopyPassBtn);
            this.Controls.Add(this.PassLabel);
            this.Controls.Add(this.FilePathBtn);
            this.Controls.Add(this.CheckFileTransformation);
            this.Controls.Add(this.CreditsLabel);
            this.Controls.Add(this.Connect);
            this.Controls.Add(this.partnerIp);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Menu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Menu";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox partnerIp;
        private System.Windows.Forms.Button Connect;
        private System.Windows.Forms.Label CreditsLabel;
        private System.Windows.Forms.CheckBox CheckFileTransformation;
        private System.Windows.Forms.Button FilePathBtn;
        private System.Windows.Forms.Label PassLabel;
        private System.Windows.Forms.Button CopyPassBtn;
        private System.Windows.Forms.Button FilesInfoBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox inConnCheckBox;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Timer NetTimer;
    }
}

