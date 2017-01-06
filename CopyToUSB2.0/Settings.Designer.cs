namespace CopyToUSB2._0
{
    partial class Settings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.label1 = new System.Windows.Forms.Label();
            this.srcPath = new System.Windows.Forms.TextBox();
            this.usbLabel = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.startUp = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.label1.MaximumSize = new System.Drawing.Size(348, 35);
            this.label1.MinimumSize = new System.Drawing.Size(348, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(348, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "Please Provide the Source location of the files and the label of the   USB Flash " +
    "Drive where these files are to be copied !";
            // 
            // srcPath
            // 
            this.srcPath.Location = new System.Drawing.Point(95, 47);
            this.srcPath.Name = "srcPath";
            this.srcPath.Size = new System.Drawing.Size(257, 20);
            this.srcPath.TabIndex = 1;
            // 
            // usbLabel
            // 
            this.usbLabel.Location = new System.Drawing.Point(95, 73);
            this.usbLabel.Name = "usbLabel";
            this.usbLabel.Size = new System.Drawing.Size(257, 20);
            this.usbLabel.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Source Path :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "USB Label :";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(277, 102);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // startUp
            // 
            this.startUp.AutoSize = true;
            this.startUp.Location = new System.Drawing.Point(15, 106);
            this.startUp.Name = "startUp";
            this.startUp.Size = new System.Drawing.Size(216, 17);
            this.startUp.TabIndex = 6;
            this.startUp.Text = "Start the application when windows start";
            this.startUp.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 137);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(229, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Note: Removal Disk is generally unamed drives";
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 162);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.startUp);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.usbLabel);
            this.Controls.Add(this.srcPath);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(380, 200);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(380, 200);
            this.Name = "Settings";
            this.Text = "Settings CopyToUSB2.0";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox srcPath;
        private System.Windows.Forms.TextBox usbLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox startUp;
        private System.Windows.Forms.Label label4;
    }
}