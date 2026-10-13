namespace VersionJsonGenerator
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.btnBrowseFolder = new System.Windows.Forms.Button();
            this.txtRootPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtServerBaseUrl = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtOldVersion = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtNewVersion = new System.Windows.Forms.TextBox();
            this.btnLoadOld = new System.Windows.Forms.Button();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnStartHttpServer = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnStopService = new System.Windows.Forms.Button();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "版本管理器";
            // 
            // btnBrowseFolder
            // 
            this.btnBrowseFolder.Location = new System.Drawing.Point(615, 64);
            this.btnBrowseFolder.Name = "btnBrowseFolder";
            this.btnBrowseFolder.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseFolder.TabIndex = 1;
            this.btnBrowseFolder.Text = "选择";
            this.btnBrowseFolder.UseVisualStyleBackColor = true;
            this.btnBrowseFolder.Click += new System.EventHandler(this.btnBrowseFolder_Click);
            // 
            // txtRootPath
            // 
            this.txtRootPath.Location = new System.Drawing.Point(158, 64);
            this.txtRootPath.Name = "txtRootPath";
            this.txtRootPath.Size = new System.Drawing.Size(434, 21);
            this.txtRootPath.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(45, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "软件发布目录";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(29, 120);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "服务器基础地址";
            // 
            // txtServerBaseUrl
            // 
            this.txtServerBaseUrl.Location = new System.Drawing.Point(158, 115);
            this.txtServerBaseUrl.Name = "txtServerBaseUrl";
            this.txtServerBaseUrl.Size = new System.Drawing.Size(317, 21);
            this.txtServerBaseUrl.TabIndex = 4;
            this.txtServerBaseUrl.Text = "http://192.168.1.105:8080";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(93, 175);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 16);
            this.label4.TabIndex = 7;
            this.label4.Text = "旧版本";
            // 
            // txtOldVersion
            // 
            this.txtOldVersion.Enabled = false;
            this.txtOldVersion.Location = new System.Drawing.Point(158, 170);
            this.txtOldVersion.Name = "txtOldVersion";
            this.txtOldVersion.Size = new System.Drawing.Size(434, 21);
            this.txtOldVersion.TabIndex = 6;
            this.txtOldVersion.Text = "0.0.0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(93, 232);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 16);
            this.label5.TabIndex = 9;
            this.label5.Text = "新版本";
            // 
            // txtNewVersion
            // 
            this.txtNewVersion.Location = new System.Drawing.Point(158, 227);
            this.txtNewVersion.Name = "txtNewVersion";
            this.txtNewVersion.Size = new System.Drawing.Size(434, 21);
            this.txtNewVersion.TabIndex = 8;
            this.txtNewVersion.Text = "0.0.0";
            // 
            // btnLoadOld
            // 
            this.btnLoadOld.Location = new System.Drawing.Point(615, 168);
            this.btnLoadOld.Name = "btnLoadOld";
            this.btnLoadOld.Size = new System.Drawing.Size(75, 23);
            this.btnLoadOld.TabIndex = 10;
            this.btnLoadOld.Text = "读取";
            this.btnLoadOld.UseVisualStyleBackColor = true;
            this.btnLoadOld.Click += new System.EventHandler(this.btnLoadOld_Click);
            // 
            // btnGenerate
            // 
            this.btnGenerate.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.btnGenerate.Location = new System.Drawing.Point(263, 264);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(188, 34);
            this.btnGenerate.TabIndex = 11;
            this.btnGenerate.Text = "版本生成";
            this.btnGenerate.UseVisualStyleBackColor = false;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.txtLog.Location = new System.Drawing.Point(143, 338);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(438, 100);
            this.txtLog.TabIndex = 12;
            // 
            // btnStartHttpServer
            // 
            this.btnStartHttpServer.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.btnStartHttpServer.Location = new System.Drawing.Point(628, 357);
            this.btnStartHttpServer.Name = "btnStartHttpServer";
            this.btnStartHttpServer.Size = new System.Drawing.Size(142, 34);
            this.btnStartHttpServer.TabIndex = 13;
            this.btnStartHttpServer.Text = "启动Http服务";
            this.btnStartHttpServer.UseVisualStyleBackColor = false;
            this.btnStartHttpServer.Click += new System.EventHandler(this.btnStartHttpServer_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.BackColor = System.Drawing.SystemColors.Info;
            this.progressBar1.Location = new System.Drawing.Point(143, 316);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(438, 16);
            this.progressBar1.TabIndex = 14;
            // 
            // btnStopService
            // 
            this.btnStopService.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.btnStopService.Location = new System.Drawing.Point(628, 404);
            this.btnStopService.Name = "btnStopService";
            this.btnStopService.Size = new System.Drawing.Size(142, 34);
            this.btnStopService.TabIndex = 16;
            this.btnStopService.Text = "停止Http服务";
            this.btnStopService.UseVisualStyleBackColor = false;
            this.btnStopService.Click += new System.EventHandler(this.btnStopService_Click);
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(504, 115);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(88, 21);
            this.txtPort.TabIndex = 17;
            this.txtPort.Text = "8080";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.btnStopService);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnStartHttpServer);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.btnLoadOld);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtNewVersion);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtOldVersion);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtServerBaseUrl);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtRootPath);
            this.Controls.Add(this.btnBrowseFolder);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "版本管理器";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowseFolder;
        private System.Windows.Forms.TextBox txtRootPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtServerBaseUrl;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtOldVersion;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtNewVersion;
        private System.Windows.Forms.Button btnLoadOld;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnStartHttpServer;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnStopService;
        private System.Windows.Forms.TextBox txtPort;
    }
}

