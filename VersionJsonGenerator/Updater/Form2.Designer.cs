namespace Updater
{
    partial class Form2
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.btnExitSetting = new System.Windows.Forms.Button();
            this.chkShowIcon = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.picIconPreview = new System.Windows.Forms.PictureBox();
            this.btnSelectIcon = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSelectBg = new System.Windows.Forms.Button();
            this.picBgPreview = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtRemoteUrl = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtGameName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picIconPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBgPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(122, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "标题";
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(204, 60);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(339, 21);
            this.txtTitle.TabIndex = 1;
            this.txtTitle.Text = "XXX数字孪生";
            // 
            // btnExitSetting
            // 
            this.btnExitSetting.BackgroundImage = global::Updater.Properties.Resources.退出;
            this.btnExitSetting.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnExitSetting.Location = new System.Drawing.Point(756, 13);
            this.btnExitSetting.Name = "btnExitSetting";
            this.btnExitSetting.Size = new System.Drawing.Size(32, 31);
            this.btnExitSetting.TabIndex = 2;
            this.btnExitSetting.UseVisualStyleBackColor = true;
            this.btnExitSetting.Click += new System.EventHandler(this.btnExitSetting_Click);
            // 
            // chkShowIcon
            // 
            this.chkShowIcon.AutoSize = true;
            this.chkShowIcon.Location = new System.Drawing.Point(201, 182);
            this.chkShowIcon.Name = "chkShowIcon";
            this.chkShowIcon.Size = new System.Drawing.Size(48, 16);
            this.chkShowIcon.TabIndex = 3;
            this.chkShowIcon.Text = "显示";
            this.chkShowIcon.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(119, 186);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "显示图标";
            // 
            // picIconPreview
            // 
            this.picIconPreview.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.picIconPreview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picIconPreview.Location = new System.Drawing.Point(375, 182);
            this.picIconPreview.Name = "picIconPreview";
            this.picIconPreview.Size = new System.Drawing.Size(152, 105);
            this.picIconPreview.TabIndex = 5;
            this.picIconPreview.TabStop = false;
            // 
            // btnSelectIcon
            // 
            this.btnSelectIcon.Location = new System.Drawing.Point(201, 230);
            this.btnSelectIcon.Name = "btnSelectIcon";
            this.btnSelectIcon.Size = new System.Drawing.Size(75, 23);
            this.btnSelectIcon.TabIndex = 6;
            this.btnSelectIcon.Text = "选择图标";
            this.btnSelectIcon.UseVisualStyleBackColor = true;
            this.btnSelectIcon.Click += new System.EventHandler(this.btnSelectIcon_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(304, 186);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "预览图标：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(304, 322);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "预览背景：";
            // 
            // btnSelectBg
            // 
            this.btnSelectBg.Location = new System.Drawing.Point(201, 318);
            this.btnSelectBg.Name = "btnSelectBg";
            this.btnSelectBg.Size = new System.Drawing.Size(75, 23);
            this.btnSelectBg.TabIndex = 10;
            this.btnSelectBg.Text = "选择背景";
            this.btnSelectBg.UseVisualStyleBackColor = true;
            this.btnSelectBg.Click += new System.EventHandler(this.btnSelectBg_Click);
            // 
            // picBgPreview
            // 
            this.picBgPreview.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.picBgPreview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picBgPreview.Location = new System.Drawing.Point(375, 318);
            this.picBgPreview.Name = "picBgPreview";
            this.picBgPreview.Size = new System.Drawing.Size(152, 105);
            this.picBgPreview.TabIndex = 9;
            this.picBgPreview.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(119, 322);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "背景设置";
            // 
            // txtRemoteUrl
            // 
            this.txtRemoteUrl.Location = new System.Drawing.Point(204, 98);
            this.txtRemoteUrl.Name = "txtRemoteUrl";
            this.txtRemoteUrl.Size = new System.Drawing.Size(339, 21);
            this.txtRemoteUrl.TabIndex = 13;
            this.txtRemoteUrl.Text = "http://192.168.1.105:8080/version.json";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(122, 101);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(23, 12);
            this.label6.TabIndex = 12;
            this.label6.Text = "Url";
            // 
            // txtGameName
            // 
            this.txtGameName.Location = new System.Drawing.Point(204, 138);
            this.txtGameName.Name = "txtGameName";
            this.txtGameName.Size = new System.Drawing.Size(339, 21);
            this.txtGameName.TabIndex = 15;
            this.txtGameName.Text = "CPK";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(74, 141);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 12);
            this.label7.TabIndex = 14;
            this.label7.Text = "跟新对象名称";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtGameName);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtRemoteUrl);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnSelectBg);
            this.Controls.Add(this.picBgPreview);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnSelectIcon);
            this.Controls.Add(this.picIconPreview);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkShowIcon);
            this.Controls.Add(this.btnExitSetting);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.label1);
            this.Name = "Form2";
            this.Text = "设置";
            ((System.ComponentModel.ISupportInitialize)(this.picIconPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBgPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Button btnExitSetting;
        private System.Windows.Forms.CheckBox chkShowIcon;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox picIconPreview;
        private System.Windows.Forms.Button btnSelectIcon;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSelectBg;
        private System.Windows.Forms.PictureBox picBgPreview;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtRemoteUrl;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtGameName;
        private System.Windows.Forms.Label label7;
    }
}