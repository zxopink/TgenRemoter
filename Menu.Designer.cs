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
            this.publicIpLabel = new System.Windows.Forms.Label();
            this.partnerIp = new System.Windows.Forms.TextBox();
            this.Connect = new System.Windows.Forms.Button();
            this.copyPublicIpButton = new System.Windows.Forms.Button();
            this.copyLocalIpButton = new System.Windows.Forms.Button();
            this.localIpLabel = new System.Windows.Forms.Label();
            this.portLabel = new System.Windows.Forms.Label();
            this.serverListenCheckBox = new System.Windows.Forms.CheckBox();
            this.CreditsLabel = new System.Windows.Forms.Label();
            this.CheckFileTransformation = new System.Windows.Forms.CheckBox();
            this.ButtonFilePath = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // publicIpLabel
            // 
            this.publicIpLabel.AutoSize = true;
            this.publicIpLabel.Location = new System.Drawing.Point(147, 65);
            this.publicIpLabel.Name = "publicIpLabel";
            this.publicIpLabel.Size = new System.Drawing.Size(49, 13);
            this.publicIpLabel.TabIndex = 0;
            this.publicIpLabel.Text = "Public IP";
            // 
            // partnerIp
            // 
            this.partnerIp.Location = new System.Drawing.Point(520, 70);
            this.partnerIp.Name = "partnerIp";
            this.partnerIp.Size = new System.Drawing.Size(100, 20);
            this.partnerIp.TabIndex = 1;
            // 
            // Connect
            // 
            this.Connect.Location = new System.Drawing.Point(533, 96);
            this.Connect.Name = "Connect";
            this.Connect.Size = new System.Drawing.Size(75, 23);
            this.Connect.TabIndex = 2;
            this.Connect.Text = "Connect";
            this.Connect.UseVisualStyleBackColor = true;
            this.Connect.Click += new System.EventHandler(this.Connect_Click);
            // 
            // copyPublicIpButton
            // 
            this.copyPublicIpButton.Location = new System.Drawing.Point(289, 60);
            this.copyPublicIpButton.Name = "copyPublicIpButton";
            this.copyPublicIpButton.Size = new System.Drawing.Size(75, 23);
            this.copyPublicIpButton.TabIndex = 3;
            this.copyPublicIpButton.Text = "Copy";
            this.copyPublicIpButton.UseVisualStyleBackColor = true;
            this.copyPublicIpButton.Click += new System.EventHandler(this.copyPublicIpButton_Click);
            // 
            // copyLocalIpButton
            // 
            this.copyLocalIpButton.Location = new System.Drawing.Point(289, 96);
            this.copyLocalIpButton.Name = "copyLocalIpButton";
            this.copyLocalIpButton.Size = new System.Drawing.Size(75, 23);
            this.copyLocalIpButton.TabIndex = 4;
            this.copyLocalIpButton.Text = "Copy";
            this.copyLocalIpButton.UseVisualStyleBackColor = true;
            this.copyLocalIpButton.Click += new System.EventHandler(this.copyLocalIpButton_Click);
            // 
            // localIpLabel
            // 
            this.localIpLabel.AutoSize = true;
            this.localIpLabel.Location = new System.Drawing.Point(147, 106);
            this.localIpLabel.Name = "localIpLabel";
            this.localIpLabel.Size = new System.Drawing.Size(42, 13);
            this.localIpLabel.TabIndex = 5;
            this.localIpLabel.Text = "local IP";
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(223, 148);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(26, 13);
            this.portLabel.TabIndex = 6;
            this.portLabel.Text = "Port";
            // 
            // serverListenCheckBox
            // 
            this.serverListenCheckBox.AutoSize = true;
            this.serverListenCheckBox.Location = new System.Drawing.Point(150, 28);
            this.serverListenCheckBox.Name = "serverListenCheckBox";
            this.serverListenCheckBox.Size = new System.Drawing.Size(92, 17);
            this.serverListenCheckBox.TabIndex = 7;
            this.serverListenCheckBox.Text = "Listen (server)";
            this.serverListenCheckBox.UseVisualStyleBackColor = true;
            this.serverListenCheckBox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
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
            // ButtonFilePath
            // 
            this.ButtonFilePath.Location = new System.Drawing.Point(16, 205);
            this.ButtonFilePath.Name = "ButtonFilePath";
            this.ButtonFilePath.Size = new System.Drawing.Size(110, 23);
            this.ButtonFilePath.TabIndex = 11;
            this.ButtonFilePath.Text = "Folder Target";
            this.ButtonFilePath.UseVisualStyleBackColor = true;
            this.ButtonFilePath.Click += new System.EventHandler(this.ButtonFilePath_Click);
            // 
            // Menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ButtonFilePath);
            this.Controls.Add(this.CheckFileTransformation);
            this.Controls.Add(this.CreditsLabel);
            this.Controls.Add(this.serverListenCheckBox);
            this.Controls.Add(this.portLabel);
            this.Controls.Add(this.localIpLabel);
            this.Controls.Add(this.copyLocalIpButton);
            this.Controls.Add(this.copyPublicIpButton);
            this.Controls.Add(this.Connect);
            this.Controls.Add(this.partnerIp);
            this.Controls.Add(this.publicIpLabel);
            this.Name = "Menu";
            this.Text = "Menu";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label publicIpLabel;
        private System.Windows.Forms.TextBox partnerIp;
        private System.Windows.Forms.Button Connect;
        private System.Windows.Forms.Button copyPublicIpButton;
        private System.Windows.Forms.Button copyLocalIpButton;
        private System.Windows.Forms.Label localIpLabel;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.CheckBox serverListenCheckBox;
        private System.Windows.Forms.Label CreditsLabel;
        private System.Windows.Forms.CheckBox CheckFileTransformation;
        private System.Windows.Forms.Button ButtonFilePath;
    }
}

