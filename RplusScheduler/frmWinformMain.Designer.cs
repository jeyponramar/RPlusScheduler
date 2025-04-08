namespace RplusScheduler
{
    partial class frmWinformMain
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
            this.btnruncommonscript = new System.Windows.Forms.Button();
            this.btnclientconfig = new System.Windows.Forms.Button();
            this.ddlcompany = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnruncommonscript
            // 
            this.btnruncommonscript.Location = new System.Drawing.Point(25, 368);
            this.btnruncommonscript.Name = "btnruncommonscript";
            this.btnruncommonscript.Size = new System.Drawing.Size(232, 65);
            this.btnruncommonscript.TabIndex = 1;
            this.btnruncommonscript.Text = "Run Common Script";
            this.btnruncommonscript.UseVisualStyleBackColor = true;
            this.btnruncommonscript.Click += new System.EventHandler(this.btnruncommonscript_Click);
            // 
            // btnclientconfig
            // 
            this.btnclientconfig.Location = new System.Drawing.Point(25, 71);
            this.btnclientconfig.Name = "btnclientconfig";
            this.btnclientconfig.Size = new System.Drawing.Size(232, 65);
            this.btnclientconfig.TabIndex = 2;
            this.btnclientconfig.Text = "Client Config";
            this.btnclientconfig.UseVisualStyleBackColor = true;
            this.btnclientconfig.Click += new System.EventHandler(this.btnclientconfig_Click);
            // 
            // ddlcompany
            // 
            this.ddlcompany.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlcompany.FormattingEnabled = true;
            this.ddlcompany.Location = new System.Drawing.Point(25, 29);
            this.ddlcompany.Name = "ddlcompany";
            this.ddlcompany.Size = new System.Drawing.Size(232, 21);
            this.ddlcompany.TabIndex = 3;
            // 
            // frmWinformMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 496);
            this.Controls.Add(this.ddlcompany);
            this.Controls.Add(this.btnclientconfig);
            this.Controls.Add(this.btnruncommonscript);
            this.Name = "frmWinformMain";
            this.Text = "frmWinformMain";
            this.Load += new System.EventHandler(this.frmWinformMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnruncommonscript;
        private System.Windows.Forms.Button btnclientconfig;
        private System.Windows.Forms.ComboBox ddlcompany;
    }
}