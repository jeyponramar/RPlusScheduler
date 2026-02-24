using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RplusScheduler
{
    public partial class frmWinformMain : Form
    {
        public frmWinformMain()
        {
            InitializeComponent();
        }

        private void btnruncommonscript_Click(object sender, EventArgs e)
        {
            frmRunCommonScript frm = new frmRunCommonScript();
            frm.ShowDialog();
        }

        private void btnclientconfig_Click(object sender, EventArgs e)
        {
            frmClientConfig frm = new frmClientConfig();
            frm._companyId = GlobalUtilities.ConvertToInt(ddlcompany.SelectedValue);
            frm._companyName = ddlcompany.Text;
            frm.ShowDialog();
        }

        private void frmWinformMain_Load(object sender, EventArgs e)
        {
            BindCompanyList();
        }
        private void BindCompanyList()
        {
            string query = "";
            query = @"select * from tbl_company where company_isactive=1 and company_ismultitenant=1
                    order by company_companyname";
            DataTable dttbl = DbTableWinform.ExecuteSelectQuery(query);
            ddlcompany.DataSource = dttbl;
            ddlcompany.DisplayMember = "company_companyname";
            ddlcompany.ValueMember = "company_companyid";
        }
    }
}
