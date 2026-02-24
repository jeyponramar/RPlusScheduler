using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebComponent;

namespace RplusScheduler
{
    public partial class frmClientConfig : Form
    {
        public int _companyId = 0;
        public string _companyName = "";
        public frmClientConfig()
        {
            InitializeComponent();
        }

        private void frmClientConfig_Load(object sender, EventArgs e)
        {
            this.Text = _companyName;
            lbltitle.Text = "Config - " + _companyName;
            string subdomain = WinformCommon.GetCompanySubdomain(_companyId);
            AppConstants.WinformSubdomain = subdomain;
            AppConstants.IsWinformMultiTenantChild = true;
            AppConstants.WinformConnectionString = AppConstantsWinform.GetChildConnectionString(subdomain);
        }

        private void btngeneratecompanycolumns_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to generate company columns", "Confirm",
                    MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            CommonR.GenerateCustomColumns(0);
            MessageBox.Show("Company columns generated successfully!");
        }

        private void btngeneratetablecolumns_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to generate the table columns", "Confirm",
                    MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            RplusMultiTenant.CreateAllTableColumns();

            MessageBox.Show("Table columns generated successfully!");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to generate DELETE and recreate company columns", "Confirm",
                    MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            string query = "truncate table tbl_companycolumns";
            DbTable.ExecuteQuery(query);

            CommonR.GenerateCustomColumns(0);
            MessageBox.Show("Company columns generated successfully!");
        }
    }
}
