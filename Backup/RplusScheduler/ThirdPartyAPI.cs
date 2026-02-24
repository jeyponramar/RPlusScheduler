using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WebComponent;

namespace RplusScheduler
{
    public class ThirdPartyAPI
    {
        public void ProcessAPI(EnumAPIType apiType)
        {
            if (apiType == EnumAPIType.INDIA_MART)
            {
                if (AppConstantsWinform.IsMultitenant)
                {
                    string query = "";
                    AppConstantsWinform.ConnectionString = AppConstantsWinform.RPlusMasterConnectionString;
                    query = "select * from tbl_company where company_subdomain<>'rpluscrm_master_v8' and company_isactive=1 and company_isindiamartapienabled=1 and company_ismultitenant=1";
                    DataTable dttbl = DbTableWinform.ExecuteSelectQuery(query);
                    for (int i = 0; i < dttbl.Rows.Count; i++)
                    {
                        string subdomain = GlobalUtilitiesWinform.ConvertToString(dttbl.Rows[i]["company_subdomain"]);
                        string indiamartApiKey = GlobalUtilitiesWinform.ConvertToString(dttbl.Rows[i]["company_indiamartapikey"]);
                        AppConstants.WinformSubdomain = subdomain;
                        AppConstantsWinform.ConnectionString = AppConstantsWinform.GetChildConnectionString(subdomain);
                        ErrorLog.WriteLog("ProcessAPI(INDIA_MART) started for " + subdomain);
                        //AppConstants.IndiaMart_API_URL = "";// AppConstants.IndiaMart_API_PRIMARY_URL.Replace("$API_KEY", indiamartApiKey);
                        //ErrorLog.WriteLog(AppConstants.IndiaMart_API_URL);
                        ProcessIndiaMartAPI(subdomain, indiamartApiKey);
                        ErrorLog.WriteLog("ProcessAPI(INDIA_MART) completed for " + subdomain);
                    }
                }
                else
                {
                    ProcessIndiaMartAPI("", "");
                }
            }
            else if (apiType == EnumAPIType.TRADE_INDIA)
            {
                if (AppConstantsWinform.IsMultitenant)
                {
                    string query = "";
                    AppConstantsWinform.ConnectionString = AppConstantsWinform.RPlusMasterConnectionString;
                    query = "select * from tbl_company where company_subdomain<>'rpluscrm_master_v8' and company_isactive=1 and company_istradeindiaapienabled=1 and company_ismultitenant=1";
                    DataTable dttbl = DbTableWinform.ExecuteSelectQuery(query);
                    for (int i = 0; i < dttbl.Rows.Count; i++)
                    {
                        string subdomain = GlobalUtilitiesWinform.ConvertToString(dttbl.Rows[i]["company_subdomain"]);
                        string tradeindiaApiKey = "9831f38321165d3c22a4f7fb65bab1eb";// GlobalUtilitiesWinform.ConvertToString(dttbl.Rows[i]["company_tradeindiaapikey"]);
                        string userid = "1764716";// GlobalUtilitiesWinform.ConvertToString(dttbl.Rows[i]["company_tradeindiaapiuserid"]);
                        string profileid = "2203120";// GlobalUtilitiesWinform.ConvertToString(dttbl.Rows[i]["company_tradeindiaapiprofileid"]);
                        AppConstants.WinformSubdomain = subdomain;
                        AppConstantsWinform.ConnectionString = AppConstantsWinform.GetChildConnectionString(subdomain);
                        ErrorLog.WriteLog("ProcessAPI(TRADE_INDIA) started for " + subdomain);
                        string apiKey = "userid=" + userid + "&profile_id=" + profileid + "&key=" + tradeindiaApiKey;
                        ProcessTradeIndiaAPI(subdomain, apiKey);
                        ErrorLog.WriteLog("ProcessAPI(TRADE_INDIA) completed for " + subdomain);
                    }
                }
                else
                {
                    ProcessTradeIndiaAPI("", "");
                }
            }
        }
        private void ProcessIndiaMartAPI(string subdomain, string apiKey)
        {
            //string json = System.IO.File.ReadAllText(@"G:\Ram\Projects\Windows\RplusScheduler\RplusScheduler\india_mart.json");
            IndiaMartAPI obj = new IndiaMartAPI();
            obj.Process(subdomain, apiKey);
        }
        private void ProcessTradeIndiaAPI(string subdomain, string apiKey)
        {
            TradeIndiaAPI obj = new TradeIndiaAPI();
            obj.Process(subdomain, apiKey);
        }
    }
}
