using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using WebComponent;
using System.Threading;
using System.Configuration;
using System.Data.SqlClient;

namespace RplusScheduler
{
    public partial class frmRunCommonScript : Form
    {
        public frmRunCommonScript()
        {
            InitializeComponent();
        }

        private void btnruncommonscript_Click(object sender, EventArgs e)
        {
            frmRunCommonScript frm = new frmRunCommonScript();
            frm.ShowDialog();
        }
        private DataTable GetCompanies()
        {
            AppConstants.WinformConnectionString = AppConstantsWinform.RPlusMasterConnectionString;
            string query = @"select * from tbl_company where company_subdomain<>'rpluscrm_master_v8' and company_isactive=1 and company_ismultitenant=1
                            and 'RPlusCRM_' + company_subdomain+ '_V8' in(select name  from sys.databases)";
            if (txtsubdomains.Text != "")
            {
                string subdomains = CommonR.CommaSepWithSingleQuote(txtsubdomains.Text);
                query += " and company_subdomain in(" + subdomains + ")";
            }
            DataTable dttbl = DbTable.ExecuteSelect(query);
            return dttbl;
        }
        private void btnrunscript_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to run script?", "Confirm",
                        MessageBoxButtons.YesNo) != DialogResult.Yes) return;

                string folderPath = txtscriptfolderpath.Text;
                if (folderPath == "" && txtscript.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter folder path OR script");
                    return;
                }
                if (folderPath != "" && !Directory.Exists(folderPath))
                {
                    MessageBox.Show("Invalid folder path");
                    return;
                }
                if (folderPath != "" && txtscript.Text.Trim() != "")
                {
                    MessageBox.Show("You can enter folder path OR script");
                    return;
                }
                lblstatus.Text = "";
                txterror.Text = "";
                txterrordatabase.Text = "";
                txterror.Text = "";
                string script = txtscript.Text;
                AppConstantsWinform.ConnectionString = AppConstantsWinform.RPlusMasterConnectionString;
                AppConstants.WinformSubdomain = "";

                if (chkincludemasterdb.Checked)
                {
                    InsertUpdate obj1 = new InsertUpdate();
                    obj1._throwError = false;
                    if (folderPath == "")
                    {
                        bool isexecuted1 = obj1.ExecuteQuery(script);
                        if (!isexecuted1)
                        {
                            txterror.Text += obj1._error + Environment.NewLine;
                        }
                    }
                    else
                    {
                        bool isexecuted = ExecuteScriptFolder(folderPath);
                        if (!isexecuted)
                        {
                            txterrordatabase.Text += "master,";
                        }
                    }
                }

                DataTable dttbl = GetCompanies();
                for (int i = 0; i < dttbl.Rows.Count; i++)
                {
                    string subdomain = "";
                    try
                    {
                        subdomain = GlobalUtilitiesWinform.ConvertToString(dttbl.Rows[i]["company_subdomain"]);
                        AppConstants.WinformSubdomain = subdomain;
                        AppConstants.IsWinformMultiTenantChild = true;
                        AppConstants.WinformConnectionString = AppConstantsWinform.GetChildConnectionString(subdomain);
                        ErrorLog.WriteLog("Run common script started for " + subdomain);
                        bool isexecuted = false;
                        InsertUpdate obj = new InsertUpdate();
                        if (folderPath == "")
                        {
                            obj._throwError = false;
                            isexecuted = obj.ExecuteQuery(script);
                            if (!isexecuted)
                            {
                                txterror.Text += obj._error + Environment.NewLine;
                            }
                        }
                        else
                        {
                            isexecuted = ExecuteScriptFolder(folderPath);
                        }
                        if (!isexecuted)
                        {
                            txterrordatabase.Text += subdomain + ",";

                            if (obj._error.Contains("Cannot open database"))
                            {
                            }
                            else
                            {
                                if (!chkSkipError.Checked)
                                {
                                    MessageBox.Show("Error occurred!");
                                    return;
                                }
                            }
                        }
                        lblstatus.Text = (i + 1).ToString() + " / " + dttbl.Rows.Count;
                        lblstatus.Refresh();
                        //txterror.Text = txterror.Text + Environment.NewLine + subdomain;
                        txterrordatabase.Refresh();
                        ErrorLog.WriteLog("Run common script completed for " + subdomain);
                        Thread.Sleep(10);
                    }
                    catch (Exception ex)
                    {
                        txterror.Text += Environment.NewLine + subdomain + ": " + Environment.NewLine;
                    }
                }
                MessageBox.Show("Script executed successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool ExecuteScriptFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath)) return true;
            DirectoryInfo dir = new DirectoryInfo(folderPath);
           
            foreach (FileInfo file in dir.GetFiles())
            {
                bool isexecuted = ExecuteScriptFile(file.FullName);
                if (!isexecuted)
                {
                    return false;
                }
            }
            return true;
        }
        private bool ExecuteScriptFile(string filePath)
        {
            if (!File.Exists(filePath)) return true;
            string script = GlobalUtilities.ReadFile(filePath);
            InsertUpdate obj = new InsertUpdate();
            obj._throwError = false;
            bool isexecuted = true;
            isexecuted = obj.ExecuteQuery(script);
            if (!isexecuted)
            {
                string fileName = filePath.Substring(filePath.IndexOf('/') + 1);
                txterror.Text += "Error while executing script " + fileName + "<br/>" +
                                obj._error + Environment.NewLine;
                return false;
            }
            return true;
        }

        private void btncreatetablecolumns_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to generate table columns?", "Confirm",
                    MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            DataTable dttbl = GetCompanies();
            //create table columns for master db
            AppConstants.WinformConnectionString = AppConstantsWinform.RPlusMasterConnectionString;
            RplusMultiTenant.CreateAllTableColumns("", false);
            AppConstants.IsWinformMultiTenantChild = false;
            AppConstants.WinformSubdomain = "";
            for (int i = 0; i < dttbl.Rows.Count; i++)
            {
                string subdomain = GlobalUtilitiesWinform.ConvertToString(dttbl.Rows[i]["company_subdomain"]);
                AppConstants.IsWinformMultiTenantChild = true;
                AppConstants.WinformSubdomain = subdomain;
                AppConstants.WinformConnectionString = AppConstantsWinform.GetChildConnectionString(subdomain);

                ErrorLog.WriteLog("Run create table columns started for " + subdomain);
                RplusMultiTenant.CreateAllTableColumns("", true);
                lblstatus.Text = (i + 1).ToString() + " / " + dttbl.Rows.Count;
                lblstatus.Refresh();
                ErrorLog.WriteLog("Run create table columns completed for " + subdomain);
            }

            MessageBox.Show("Columns generated successfully!");
        }

        private void txterror_TextChanged(object sender, EventArgs e)
        {

        }

        private void frmRunCommonScript_Load(object sender, EventArgs e)
        {

        }

        private void btnimportmasterdata_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show("Are you sure you want to import the master data?", "Confirm",
            //        MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            //DataTable dttbl = GetCompanies();
            ////create table columns for master db
            //AppConstants.WinformConnectionString = AppConstantsWinform.RPlusMasterConnectionString;
            //AppConstants.IsWinformMultiTenantChild = false;
            //AppConstants.WinformSubdomain = "";
            //string query = "";
            //query = "select * from tbl_module where module_ismasterdatacopy=1";
            
            //for (int i = 0; i < dttbl.Rows.Count; i++)
            //{
            //    bool isCopyPrimary = GlobalUtilities.ConvertToBool(dttbl.Rows[i]["module_ismasterdatacopyprimary"]);
            //    bool isCopyIfEmpty = GlobalUtilities.ConvertToBool(dttbl.Rows[i]["module_ismasterdatacopyifempty"]);
            //    bool isCopyIfNotExists = GlobalUtilities.ConvertToBool(dttbl.Rows[i]["module_ismasterdatacopyifnotexists"]);
            //    string module = GlobalUtilities.ConvertToString(dttbl.Rows[i]["module_module"]);
            //    string subdomain = GlobalUtilitiesWinform.ConvertToString(dttbl.Rows[i]["company_subdomain"]);

            //    AppConstants.IsWinformMultiTenantChild = true;
            //    AppConstants.WinformSubdomain = subdomain;
            //    AppConstants.WinformConnectionString = AppConstantsWinform.GetChildConnectionString(subdomain);

            //    string script = GenerateInsertScript(module, isCopyPrimary, isCopyIfEmpty, isCopyIfNotExists);

            //    ErrorLog.WriteLog("Run create table columns started for " + subdomain);
            //    DbTable.ExecuteQuery(script);

            //    lblstatus.Text = (i + 1).ToString() + " / " + dttbl.Rows.Count;
            //    lblstatus.Refresh();
            //    ErrorLog.WriteLog("Run create table columns completed for " + subdomain);
            //}

            //MessageBox.Show("Columns generated successfully!");
        }
        private string GenerateInsertScript(string module, bool isCopyPrimaryOnly, bool isCopyIfEmpty, bool isCopyIfNotExists)
        {
            string query = "";

            query = @"select COLUMN_NAME,DATA_TYPE from INFORMATION_SCHEMA.COLUMNS where table_name='tbl_" + module + @"'
                  order by ORDINAL_POSITION";
            DataTable dttblcol = DbTable.ExecuteSelect(query);
            StringBuilder insertQuery = new StringBuilder();

            StringBuilder columns = new StringBuilder();

            if (isCopyIfEmpty)
            {
                insertQuery.Append("if (select count(*) from tbl_" + module + ") = 0" + Environment.NewLine);
                insertQuery.Append("BEGIN" + Environment.NewLine);
            }
            if (isCopyPrimaryOnly || isCopyIfNotExists)
            {
            }
            else
            {
                insertQuery.Append("truncate table tbl_" + module + Environment.NewLine);
            }
            insertQuery.Append("SET IDENTITY_INSERT [dbo].[tbl_" + module + "] ON " + Environment.NewLine);

            for (int j = 0; j < dttblcol.Rows.Count; j++)
            {
                string colName = GlobalUtilities.ConvertToString(dttblcol.Rows[j]["COLUMN_NAME"]);
                if (colName == "setting_settingvalue" || colName == "rplussettings_settingvalue")
                {
                    continue;
                }
                CommonR.CommaSep(columns, colName);
            }

            query = "select * from tbl_" + module;
            if (isCopyPrimaryOnly)
            {
                query += " where " + module + "_" + module + "id < 1000";
            }
            DataTable dttbldata = DbTable.ExecuteSelect(query);
            if (dttbldata.Rows.Count == 0) return "";
            for (int i = 0; i < dttbldata.Rows.Count; i++)
            {
                int id = GlobalUtilities.ConvertToInt(dttbldata.Rows[i][module + "_" + module + "id"]);
                if (isCopyIfNotExists || isCopyPrimaryOnly)
                {
                    insertQuery.Append("if not exists(select 1 from tbl_" + module + " where " + module + "_" + module + "id=" + id + ")" + Environment.NewLine);
                }
                insertQuery.Append("insert into tbl_" + module + "(" + columns.ToString() + ")" + Environment.NewLine);
                insertQuery.Append("values(" + Environment.NewLine);
                StringBuilder values = new StringBuilder();
                for (int j = 0; j < dttblcol.Rows.Count; j++)
                {
                    string colName = GlobalUtilities.ConvertToString(dttblcol.Rows[j]["COLUMN_NAME"]);
                    string dataType = GlobalUtilities.ConvertToString(dttblcol.Rows[j]["DATA_TYPE"]);
                    if (colName == "setting_settingvalue" || colName == "rplussettings_settingvalue")
                    {
                        continue;
                    }
                    string val = "";
                    if (dttbldata.Rows[i][colName] == DBNull.Value)
                    {
                        val = "NULL";
                    }
                    else
                    {
                        if (dataType == "datetime")
                        {
                            val = "'" + GlobalUtilities.ConvertToDateTimeMMM(dttbldata.Rows[i][colName]) + "'";
                        }
                        else if (dataType == "int" || dataType == "numeric")
                        {
                            val = GlobalUtilities.ConvertToString(dttbldata.Rows[i][colName]);
                        }
                        else if (dataType == "bit")
                        {
                            if (GlobalUtilities.ConvertToBool(dttbldata.Rows[i][colName]))
                            {
                                val = "1";
                            }
                            else
                            {
                                val = "0";
                            }
                        }
                        else
                        {
                            val = "'" + GlobalUtilities.ConvertToString(dttbldata.Rows[i][colName]).Replace("'", "''") + "'";
                        }
                    }
                    CommonR.CommaSep(values, val);
                }
                insertQuery.Append(values.ToString() + Environment.NewLine);
                insertQuery.Append(")" + Environment.NewLine);
            }
            insertQuery.Append("SET IDENTITY_INSERT [dbo].[tbl_" + module + "] OFF " + Environment.NewLine);

            if (isCopyIfEmpty)
            {
                insertQuery.Append("END" + Environment.NewLine);
            }

            return insertQuery.ToString();
        }

        private void btnsynccompanycolumns_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to sync data of master tables?", "Confirm",
                    MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            DataTable dttbl = GetCompanies();
            for (int i = 0; i < dttbl.Rows.Count; i++)
            {
                string subdomain = GlobalUtilitiesWinform.ConvertToString(dttbl.Rows[i]["company_subdomain"]);
                int companyId = GlobalUtilitiesWinform.ConvertToInt(dttbl.Rows[i]["company_companyid"]);
                AppConstants.IsWinformMultiTenantChild = true;
                AppConstants.WinformSubdomain = subdomain;
                AppConstants.WinformConnectionString = AppConstantsWinform.GetChildConnectionString(subdomain);
                string project = GlobalUtilitiesWinform.ConvertToString(dttbl.Rows[i]["company_project"]);
                ErrorLog.WriteLog("Run sync company columns started for " + subdomain);
                //CommonR.GenerateCustomColumns(0, true);
                RplusMultiTenant.SetupCompanyProjectModules(AppConstants.WinformConnectionString, companyId, project);
                RplusMultiTenant.SyncCompanyColumns(AppConstants.WinformConnectionString, companyId);
                lblstatus.Text = (i + 1).ToString() + " / " + dttbl.Rows.Count;
                lblstatus.Refresh();
                ErrorLog.WriteLog("Run sync company columns completed for " + subdomain);
            }

            MessageBox.Show("Company columns synced successfully!");
        }

        private void btnDatabaseBackup_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to take backup of all databases?", "Confirm",
                    MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            string dbFolderPath = ConfigurationManager.AppSettings["DatabaseFolderPath"];
            string backupFolderPath = dbFolderPath + "/Backup/";
            backupFolderPath+=DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
            if (!Directory.Exists(backupFolderPath))
                Directory.CreateDirectory(backupFolderPath);

            DataTable dttbl = GetCompanies();
            for (int i = 0; i < dttbl.Rows.Count; i++)
            {
                string subdomain = GlobalUtilitiesWinform.ConvertToString(dttbl.Rows[i]["company_subdomain"]);
                if (subdomain.ToLower() == "rpluscrm_master_v8")
                {
                    subdomain = "master";
                }
                string databaseName = "RPlusCRM_" + subdomain + "_V8";
                try
                {
                    string filePath = backupFolderPath + "/" + databaseName + ".bak";
                    if (File.Exists(filePath)) GlobalUtilities.DeleteFile(filePath);
                    string query = "BACKUP DATABASE " + databaseName + " TO DISK = '" + filePath + "'";
                    using (SqlConnection con = new SqlConnection(RPlusConnectionString.RPlusMasterConnectionString))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.CommandTimeout = 0;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    txterrordatabase.Text = databaseName;
                    txterror.Text = ex.Message;
                }
                lblstatus.Text = (i + 1).ToString() + " / " + dttbl.Rows.Count;
                lblstatus.Refresh();
            }

            MessageBox.Show("Database backup done successfully!");
        }

        private void btnSyncSettings_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to sync the settings from master db?", "Confirm",
                        MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                ErrorLog.WriteLog("Sync settings started", false);
                DataTable dttbl = GetCompanies();
                for (int i = 0; i < dttbl.Rows.Count; i++)
                {
                    string subdomain = GlobalUtilitiesWinform.ConvertToString(dttbl.Rows[i]["company_subdomain"]);
                    ErrorLog.WriteLog("Sync settings start for " + subdomain, false);
                    AppConstants.IsWinformMultiTenantChild = true;
                    AppConstants.WinformSubdomain = subdomain;
                    AppConstants.WinformConnectionString = AppConstantsWinform.GetChildConnectionString(subdomain);
                    
                    string query = @"insert into tbl_setting(setting_createddate,setting_createdby,setting_settingname,setting_settingvalue,setting_ishtml,
                            setting_rplussettinggroupid,setting_rpluscontrolsid,setting_defaultvalue,setting_dropdownvalues,setting_isclient,
                            setting_isbottomborder,setting_setting,setting_description)
                            select getdate(),1,setting_settingname,'',setting_ishtml,
                            setting_rplussettinggroupid,setting_rpluscontrolsid,setting_defaultvalue,setting_dropdownvalues,setting_isclient,
                            setting_isbottomborder,setting_setting,setting_description
                            from [RPlusCRM_Master_V8].dbo.tbl_setting s1
                            where s1.setting_settingname not in(select setting_settingname from tbl_setting)";
                    InsertUpdate obj = new InsertUpdate();
                    try
                    {
                        obj._throwError = false;
                        obj.ExecuteQuery(query);
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show(ex2.Message);
                    }

                    ErrorLog.WriteLog("Sync settings completed for " + subdomain, false);

                    ErrorLog.WriteLog("Sync email template start for " + subdomain, false);
                    query = @"
                        set identity_insert tbl_emailtemplate on;
                        insert into tbl_emailtemplate(
                            emailtemplate_emailtemplateid,emailtemplate_templatename,emailtemplate_subject,emailtemplate_createddate,emailtemplate_createdby,
                            emailtemplate_attachments,emailtemplate_message,emailtemplate_to,emailtemplate_cc,emailtemplate_moduleid,
                            emailtemplate_ismultimail,emailtemplate_isenabled,emailtemplate_statuscolumn,emailtemplate_statusvalue,
                            emailtemplate_idcolumn,emailtemplate_module,emailtemplate_isautomatic,emailtemplate_isconfirmandsend,emailtemplate_attachment,
                            emailtemplate_aftersavequery,emailtemplate_statusupdatecolumn,emailtemplate_emailtitle,emailtemplate_isusersignature,
                            emailtemplate_emailadid,emailtemplate_issendtootherassignees,emailtemplate_sendfromloggedinuser,emailtemplate_issendtoadditionalcontact,
                            emailtemplate_isrefux,emailtemplate_whatsapptemplateid,emailtemplate_issendwhatsapp,emailtemplate_pdftemplateid
                            )
                            select emailtemplate_emailtemplateid,emailtemplate_templatename,emailtemplate_subject,getdate(),1,
                            emailtemplate_attachments,emailtemplate_message,emailtemplate_to,emailtemplate_cc,emailtemplate_moduleid,
                            emailtemplate_ismultimail,emailtemplate_isenabled,emailtemplate_statuscolumn,emailtemplate_statusvalue,
                            emailtemplate_idcolumn,emailtemplate_module,emailtemplate_isautomatic,emailtemplate_isconfirmandsend,emailtemplate_attachment,
                            emailtemplate_aftersavequery,emailtemplate_statusupdatecolumn,emailtemplate_emailtitle,emailtemplate_isusersignature,
                            emailtemplate_emailadid,emailtemplate_issendtootherassignees,emailtemplate_sendfromloggedinuser,emailtemplate_issendtoadditionalcontact,
                            emailtemplate_isrefux,emailtemplate_whatsapptemplateid,emailtemplate_issendwhatsapp,emailtemplate_pdftemplateid
                            from [RPlusCRM_Master_V8].dbo.tbl_emailtemplate t1
                            where t1.emailtemplate_emailtemplateid not in(select emailtemplate_emailtemplateid from tbl_emailtemplate);
                            set identity_insert tbl_emailtemplate off;";
                    obj = new InsertUpdate();
                    obj.ExecuteQuery(query);
                    ErrorLog.WriteLog("Sync email template completed for " + subdomain, false);

                    //                ErrorLog.WriteLog("Sync WhatsApp template start for " + subdomain);
                    //                query = @"
                    //                        set identity_insert tbl_whatsapptemplate on;
                    //                        insert into tbl_whatsapptemplate(
                    //                        whatsapptemplate_whatsapptemplateid,whatsapptemplate_templatename,whatsapptemplate_whatsapptemplatecode,whatsapptemplate_moduleid,
                    //                        whatsapptemplate_frommobileno,whatsapptemplate_mobileno,whatsapptemplate_message,whatsapptemplate_webwhatsappmessage,
                    //                        whatsapptemplate_whatsappvariables,whatsapptemplate_whatsappmessagecategoryid,whatsapptemplate_attachment,
                    //                        whatsapptemplate_ismultimessage,whatsapptemplate_isautomatic,whatsapptemplate_isenabled,whatsapptemplate_statuscolumn,
                    //                        whatsapptemplate_statusvalue,whatsapptemplate_idcolumn,whatsapptemplate_issendtootherassignees,whatsapptemplate_issendtoadditionalcontact,
                    //                        whatsapptemplate_createddate,whatsapptemplate_createdby)
                    //                        select whatsapptemplate_whatsapptemplateid,whatsapptemplate_templatename,whatsapptemplate_whatsapptemplatecode,whatsapptemplate_moduleid,
                    //                        whatsapptemplate_frommobileno,whatsapptemplate_mobileno,whatsapptemplate_message,whatsapptemplate_webwhatsappmessage,
                    //                        whatsapptemplate_whatsappvariables,whatsapptemplate_whatsappmessagecategoryid,whatsapptemplate_attachment,
                    //                        whatsapptemplate_ismultimessage,whatsapptemplate_isautomatic,whatsapptemplate_isenabled,whatsapptemplate_statuscolumn,
                    //                        whatsapptemplate_statusvalue,whatsapptemplate_idcolumn,whatsapptemplate_issendtootherassignees,whatsapptemplate_issendtoadditionalcontact,
                    //                        getdate(),1
                    //                        from [RPlusCRM_Master_V8].dbo.tbl_whatsapptemplate w
                    //                        where w.whatsapptemplate_whatsapptemplateid not in(select whatsapptemplate_whatsapptemplateid from tbl_whatsapptemplate);
                    //                        
                    //                        set identity_insert tbl_whatsapptemplate off;";
                    //                obj = new InsertUpdate();
                    //                obj.ExecuteQuery(query);
                    //                ErrorLog.WriteLog("Sync WhatsApp template completed for " + subdomain);
                    lblstatus.Text = (i + 1).ToString() + " / " + dttbl.Rows.Count.ToString();
                    lblstatus.Refresh();
                }
                ErrorLog.WriteLog("Sync settings completed for all", false);
                MessageBox.Show("Sync settings completed successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSelectCount_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to run script?", "Confirm",
                        MessageBoxButtons.YesNo) != DialogResult.Yes) return;

                lblstatus.Text = "";
                txterror.Text = "";
                txterrordatabase.Text = "";
                txterror.Text = "";
                txtresult.Text = "";
                string script = txtscript.Text;
                AppConstantsWinform.ConnectionString = AppConstantsWinform.RPlusMasterConnectionString;
                AppConstants.WinformSubdomain = "";

                if (chkincludemasterdb.Checked)
                {
                    InsertUpdate obj1 = new InsertUpdate();
                    obj1._throwError = false;
                    DataRow dr = obj1.ExecuteSelectRow(script);
                    if (dr == null)
                    {
                        txterror.Text += obj1._error + Environment.NewLine;
                    }
                    else
                    {
                        if (GlobalUtilities.ConvertToInt(dr[0]) > 0)
                        {
                            txtresult.Text = "Master," + GlobalUtilities.ConvertToInt(dr[0]);
                        }
                    }
                }

                DataTable dttbl = GetCompanies();
                for (int i = 0; i < dttbl.Rows.Count; i++)
                {
                    string subdomain = "";
                    try
                    {
                        subdomain = GlobalUtilitiesWinform.ConvertToString(dttbl.Rows[i]["company_subdomain"]);
                        AppConstants.WinformSubdomain = subdomain;
                        AppConstants.IsWinformMultiTenantChild = true;
                        AppConstants.WinformConnectionString = AppConstantsWinform.GetChildConnectionString(subdomain);
                        ErrorLog.WriteLog("Run common script started for " + subdomain);
                        bool isexecuted = false;
                        InsertUpdate obj = new InsertUpdate();
                    
                        obj._throwError = false;
                        DataRow dr = obj.ExecuteSelectRow(script);
                        if (dr == null)
                        {
                            txterror.Text += obj._error + Environment.NewLine;
                        }
                        else
                        {
                            if (GlobalUtilities.ConvertToInt(dr[0]) > 0)
                            {
                                txtresult.Text += Environment.NewLine + subdomain + "," + GlobalUtilities.ConvertToInt(dr[0]);
                            }
                        }
                        if (!isexecuted)
                        {
                            txterrordatabase.Text += subdomain + ",";

                            if (obj._error.Contains("Cannot open database"))
                            {
                            }
                            else
                            {
                                if (!chkSkipError.Checked)
                                {
                                    MessageBox.Show("Error occurred!");
                                    return;
                                }
                            }
                        }
                        lblstatus.Text = (i + 1).ToString() + " / " + dttbl.Rows.Count;
                        lblstatus.Refresh();
                        txterrordatabase.Refresh();
                        txtresult.Refresh();
                        Thread.Sleep(10);
                    }
                    catch (Exception ex)
                    {
                        txterror.Text += Environment.NewLine + subdomain + ": " + Environment.NewLine;
                    }
                }
                MessageBox.Show("Script executed successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
