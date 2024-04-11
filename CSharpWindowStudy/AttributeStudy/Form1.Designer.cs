namespace AttributeStudy
{
    partial class Form1
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
            this.heroListBox = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.skillListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(44, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "英雄";
            // 
            // heroListBox
            // 
            this.heroListBox.FormattingEnabled = true;
            this.heroListBox.ItemHeight = 12;
            this.heroListBox.Location = new System.Drawing.Point(44, 100);
            this.heroListBox.Name = "heroListBox";
            this.heroListBox.Size = new System.Drawing.Size(225, 100);
            this.heroListBox.TabIndex = 1;
            this.heroListBox.SelectedIndexChanged += new System.EventHandler(this.heroListBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(386, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 29);
            this.label2.TabIndex = 2;
            this.label2.Text = "技能";
            // 
            // skillListBox
            // 
            this.skillListBox.FormattingEnabled = true;
            this.skillListBox.ItemHeight = 12;
            this.skillListBox.Location = new System.Drawing.Point(386, 100);
            this.skillListBox.Name = "skillListBox";
            this.skillListBox.Size = new System.Drawing.Size(225, 100);
            this.skillListBox.TabIndex = 3;
            this.skillListBox.SelectedIndexChanged += new System.EventHandler(this.skillListBox_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.skillListBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.heroListBox);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox skillListBox;

        private System.Windows.Forms.ListBox heroListBox;

        private System.Windows.Forms.Label label1;

        #endregion
    }
}