using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebComponent;
using System.Data;

namespace RplusScheduler
{
    public class WinformCommon
    {
        public static string GetChildConnectionString(int companyId)
        {
            string subdomain = GetCompanySubdomain(companyId);
            return AppConstantsWinform.GetChildConnectionString(subdomain);
        }
        public static string GetCompanySubdomain(int companyId)
        {
            AppConstantsWinform.ConnectionString = AppConstantsWinform.RPlusMasterConnectionString;
            AppConstants.WinformSubdomain = "";
            string query = "";
            query = "select company_subdomain from tbl_company where company_companyid=" + companyId;
            DataRow dr = DbTable.ExecuteSelectRow(query);
            return GlobalUtilities.ConvertToString(dr["company_subdomain"]);
        }
    }
}
