namespace RplusScheduler
{
    partial class frmRunCommonScript
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
            this.btnrunscript = new System.Windows.Forms.Button();
            this.txtscriptfolderpath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtscript = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txterror = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txterrordatabase = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.lblstatus = new System.Windows.Forms.Label();
            this.btncreatetablecolumns = new System.Windows.Forms.Button();
            this.txtsubdomains = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtimportdatamodules = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnimportmasterdata = new System.Windows.Forms.Button();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.btnsynccompanycolumns = new System.Windows.Forms.Button();
            this.chkincludemasterdb = new System.Windows.Forms.CheckBox();
            this.btnDatabaseBackup = new System.Windows.Forms.Button();
            this.btnSyncSettings = new System.Windows.Forms.Button();
            this.chkSkipError = new System.Windows.Forms.CheckBox();
            this.btnSelectCount = new System.Windows.Forms.Button();
            this.txtresult = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tabControl2.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnrunscript
            // 
            this.btnrunscript.Location = new System.Drawing.Point(219, 338);
            this.btnrunscript.Name = "btnrunscript";
            this.btnrunscript.Size = new System.Drawing.Size(134, 36);
            this.btnrunscript.TabIndex = 7;
            this.btnrunscript.Text = "Run Script";
            this.btnrunscript.UseVisualStyleBackColor = true;
            this.btnrunscript.Click += new System.EventHandler(this.btnrunscript_Click);
            // 
            // txtscriptfolderpath
            // 
            this.txtscriptfolderpath.Location = new System.Drawing.Point(219, 81);
            this.txtscriptfolderpath.Name = "txtscriptfolderpath";
            this.txtscriptfolderpath.Size = new System.Drawing.Size(307, 20);
            this.txtscriptfolderpath.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(62, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Script folder path";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(334, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Run common script";
            // 
            // txtscript
            // 
            this.txtscript.Location = new System.Drawing.Point(219, 134);
            this.txtscript.Multiline = true;
            this.txtscript.Name = "txtscript";
            this.txtscript.Size = new System.Drawing.Size(307, 112);
            this.txtscript.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(62, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Script";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(396, 109);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "OR";
            // 
            // txterror
            // 
            this.txterror.Location = new System.Drawing.Point(594, 81);
            this.txterror.Multiline = true;
            this.txterror.Name = "txterror";
            this.txterror.Size = new System.Drawing.Size(348, 91);
            this.txterror.TabIndex = 12;
            this.txterror.TextChanged += new System.EventHandler(this.txterror_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(591, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Error";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(591, 35);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Error Database";
            // 
            // txterrordatabase
            // 
            this.txterrordatabase.Location = new System.Drawing.Point(703, 32);
            this.txterrordatabase.Name = "txterrordatabase";
            this.txterrordatabase.Size = new System.Drawing.Size(232, 20);
            this.txterrordatabase.TabIndex = 14;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(596, 204);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Completed : ";
            // 
            // lblstatus
            // 
            this.lblstatus.AutoSize = true;
            this.lblstatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblstatus.ForeColor = System.Drawing.Color.Green;
            this.lblstatus.Location = new System.Drawing.Point(681, 197);
            this.lblstatus.Name = "lblstatus";
            this.lblstatus.Size = new System.Drawing.Size(34, 20);
            this.lblstatus.TabIndex = 16;
            this.lblstatus.Text = "0/0";
            // 
            // btncreatetablecolumns
            // 
            this.btncreatetablecolumns.Location = new System.Drawing.Point(392, 338);
            this.btncreatetablecolumns.Name = "btncreatetablecolumns";
            this.btncreatetablecolumns.Size = new System.Drawing.Size(134, 36);
            this.btncreatetablecolumns.TabIndex = 17;
            this.btncreatetablecolumns.Text = "Create Table Columns";
            this.btncreatetablecolumns.UseVisualStyleBackColor = true;
            this.btncreatetablecolumns.Click += new System.EventHandler(this.btncreatetablecolumns_Click);
            // 
            // txtsubdomains
            // 
            this.txtsubdomains.Location = new System.Drawing.Point(219, 266);
            this.txtsubdomains.Name = "txtsubdomains";
            this.txtsubdomains.Size = new System.Drawing.Size(307, 20);
            this.txtsubdomains.TabIndex = 19;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(62, 266);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(112, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "Database subdomains";
            // 
            // txtimportdatamodules
            // 
            this.txtimportdatamodules.Location = new System.Drawing.Point(121, 36);
            this.txtimportdatamodules.Name = "txtimportdatamodules";
            this.txtimportdatamodules.Size = new System.Drawing.Size(256, 20);
            this.txtimportdatamodules.TabIndex = 21;
            this.txtimportdatamodules.Text = "setting";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(26, 39);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(79, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "Import Modules";
            // 
            // btnimportmasterdata
            // 
            this.btnimportmasterdata.Location = new System.Drawing.Point(121, 110);
            this.btnimportmasterdata.Name = "btnimportmasterdata";
            this.btnimportmasterdata.Size = new System.Drawing.Size(134, 36);
            this.btnimportmasterdata.TabIndex = 22;
            this.btnimportmasterdata.Text = "Import Master Data";
            this.btnimportmasterdata.UseVisualStyleBackColor = true;
            this.btnimportmasterdata.Click += new System.EventHandler(this.btnimportmasterdata_Click);
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage5);
            this.tabControl2.Location = new System.Drawing.Point(594, 241);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(400, 217);
            this.tabControl2.TabIndex = 25;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.txtimportdatamodules);
            this.tabPage5.Controls.Add(this.label9);
            this.tabPage5.Controls.Add(this.btnimportmasterdata);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(392, 191);
            this.tabPage5.TabIndex = 0;
            this.tabPage5.Text = "Import Master Data";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // btnsynccompanycolumns
            // 
            this.btnsynccompanycolumns.Location = new System.Drawing.Point(219, 380);
            this.btnsynccompanycolumns.Name = "btnsynccompanycolumns";
            this.btnsynccompanycolumns.Size = new System.Drawing.Size(134, 36);
            this.btnsynccompanycolumns.TabIndex = 26;
            this.btnsynccompanycolumns.Text = "Sync Company Columns";
            this.btnsynccompanycolumns.UseVisualStyleBackColor = true;
            this.btnsynccompanycolumns.Click += new System.EventHandler(this.btnsynccompanycolumns_Click);
            // 
            // chkincludemasterdb
            // 
            this.chkincludemasterdb.AutoSize = true;
            this.chkincludemasterdb.Location = new System.Drawing.Point(219, 297);
            this.chkincludemasterdb.Name = "chkincludemasterdb";
            this.chkincludemasterdb.Size = new System.Drawing.Size(114, 17);
            this.chkincludemasterdb.TabIndex = 27;
            this.chkincludemasterdb.Text = "Include Master DB";
            this.chkincludemasterdb.UseVisualStyleBackColor = true;
            // 
            // btnDatabaseBackup
            // 
            this.btnDatabaseBackup.Location = new System.Drawing.Point(392, 380);
            this.btnDatabaseBackup.Name = "btnDatabaseBackup";
            this.btnDatabaseBackup.Size = new System.Drawing.Size(134, 36);
            this.btnDatabaseBackup.TabIndex = 28;
            this.btnDatabaseBackup.Text = "Backup Database";
            this.btnDatabaseBackup.UseVisualStyleBackColor = true;
            this.btnDatabaseBackup.Click += new System.EventHandler(this.btnDatabaseBackup_Click);
            // 
            // btnSyncSettings
            // 
            this.btnSyncSettings.Location = new System.Drawing.Point(219, 422);
            this.btnSyncSettings.Name = "btnSyncSettings";
            this.btnSyncSettings.Size = new System.Drawing.Size(134, 36);
            this.btnSyncSettings.TabIndex = 29;
            this.btnSyncSettings.Text = "Sync All Settings";
            this.btnSyncSettings.UseVisualStyleBackColor = true;
            this.btnSyncSettings.Click += new System.EventHandler(this.btnSyncSettings_Click);
            // 
            // chkSkipError
            // 
            this.chkSkipError.AutoSize = true;
            this.chkSkipError.Location = new System.Drawing.Point(361, 292);
            this.chkSkipError.Name = "chkSkipError";
            this.chkSkipError.Size = new System.Drawing.Size(72, 17);
            this.chkSkipError.TabIndex = 30;
            this.chkSkipError.Text = "Skip Error";
            this.chkSkipError.UseVisualStyleBackColor = true;
            // 
            // btnSelectCount
            // 
            this.btnSelectCount.Location = new System.Drawing.Point(392, 422);
            this.btnSelectCount.Name = "btnSelectCount";
            this.btnSelectCount.Size = new System.Drawing.Size(134, 36);
            this.btnSelectCount.TabIndex = 31;
            this.btnSelectCount.Text = "Select Count";
            this.btnSelectCount.UseVisualStyleBackColor = true;
            this.btnSelectCount.Click += new System.EventHandler(this.btnSelectCount_Click_1);
            // 
            // txtresult
            // 
            this.txtresult.Location = new System.Drawing.Point(948, 35);
            this.txtresult.Multiline = true;
            this.txtresult.Name = "txtresult";
            this.txtresult.Size = new System.Drawing.Size(319, 91);
            this.txtresult.TabIndex = 33;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(945, 19);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(37, 13);
            this.label10.TabIndex = 32;
            this.label10.Text = "Result";
            // 
            // frmRunCommonScript
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1279, 506);
            this.Controls.Add(this.txtresult);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.btnSelectCount);
            this.Controls.Add(this.chkSkipError);
            this.Controls.Add(this.btnSyncSettings);
            this.Controls.Add(this.btnDatabaseBackup);
            this.Controls.Add(this.chkincludemasterdb);
            this.Controls.Add(this.btnsynccompanycolumns);
            this.Controls.Add(this.tabControl2);
            this.Controls.Add(this.txtsubdomains);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btncreatetablecolumns);
            this.Controls.Add(this.lblstatus);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txterrordatabase);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txterror);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtscript);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnrunscript);
            this.Controls.Add(this.txtscriptfolderpath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "frmRunCommonScript";
            this.Text = "frmRunCommonScript";
            this.Load += new System.EventHandler(this.frmRunCommonScript_Load);
            this.tabControl2.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnrunscript;
        private System.Windows.Forms.TextBox txtscriptfolderpath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtscript;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txterror;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txterrordatabase;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblstatus;
        private System.Windows.Forms.Button btncreatetablecolumns;
        private System.Windows.Forms.TextBox txtsubdomains;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtimportdatamodules;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnimportmasterdata;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Button btnsynccompanycolumns;
        private System.Windows.Forms.CheckBox chkincludemasterdb;
        private System.Windows.Forms.Button btnDatabaseBackup;
        private System.Windows.Forms.Button btnSyncSettings;
        private System.Windows.Forms.CheckBox chkSkipError;
        private System.Windows.Forms.Button btnSelectCount;
        private System.Windows.Forms.TextBox txtresult;
        private System.Windows.Forms.Label label10;

    }
}