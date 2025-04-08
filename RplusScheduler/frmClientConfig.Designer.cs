namespace RplusScheduler
{
    partial class frmClientConfig
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
            this.lbltitle = new System.Windows.Forms.Label();
            this.btngeneratecompanycolumns = new System.Windows.Forms.Button();
            this.btngeneratetablecolumns = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbltitle
            // 
            this.lbltitle.AutoSize = true;
            this.lbltitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbltitle.Location = new System.Drawing.Point(290, 37);
            this.lbltitle.Name = "lbltitle";
            this.lbltitle.Size = new System.Drawing.Size(50, 24);
            this.lbltitle.TabIndex = 0;
            this.lbltitle.Text = "Title";
            // 
            // btngeneratecompanycolumns
            // 
            this.btngeneratecompanycolumns.Location = new System.Drawing.Point(35, 111);
            this.btngeneratecompanycolumns.Name = "btngeneratecompanycolumns";
            this.btngeneratecompanycolumns.Size = new System.Drawing.Size(207, 65);
            this.btngeneratecompanycolumns.TabIndex = 1;
            this.btngeneratecompanycolumns.Text = "Generate Company Columns";
            this.btngeneratecompanycolumns.UseVisualStyleBackColor = true;
            this.btngeneratecompanycolumns.Click += new System.EventHandler(this.btngeneratecompanycolumns_Click);
            // 
            // btngeneratetablecolumns
            // 
            this.btngeneratetablecolumns.Location = new System.Drawing.Point(592, 111);
            this.btngeneratetablecolumns.Name = "btngeneratetablecolumns";
            this.btngeneratetablecolumns.Size = new System.Drawing.Size(207, 65);
            this.btngeneratetablecolumns.TabIndex = 2;
            this.btngeneratetablecolumns.Text = "Generate Table Columns";
            this.btngeneratetablecolumns.UseVisualStyleBackColor = true;
            this.btngeneratetablecolumns.Click += new System.EventHandler(this.btngeneratetablecolumns_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(265, 111);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(207, 65);
            this.button1.TabIndex = 3;
            this.button1.Text = "DELETE and Generate Company Columns";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // frmClientConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(828, 406);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btngeneratetablecolumns);
            this.Controls.Add(this.btngeneratecompanycolumns);
            this.Controls.Add(this.lbltitle);
            this.Name = "frmClientConfig";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmClientConfig_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbltitle;
        private System.Windows.Forms.Button btngeneratecompanycolumns;
        private System.Windows.Forms.Button btngeneratetablecolumns;
        private System.Windows.Forms.Button button1;
    }
}