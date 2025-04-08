using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using System.Collections;
using System.IO;
using System.Configuration;
using System.Threading;

/// <summary>
/// Summary description for RPlusScheduler
/// </summary>
namespace RplusScheduler
{
    public class Scheduler
    {
        public void Start()
        {
            SchedulerLog("Initiated");
            try
            {
                //string query = "delete from tbl_schedulerlog where schedulerlog_createddate<getdate()-10";
                //ExecuteQuery(query);
                if (AppConstantsWinform.IsMultitenant)
                {
                    StartMultiSite();
                }
                else
                {
                    //StartSingleSite();
                }
                SchedulerLog("Completed");
            }
            catch(Exception ex)
            {
                ErrorLog.WriteLog(ex.Message);
            }

        }
        private void StartMultiSite()
        {
            string query = @"select * from tbl_company where company_isactive=1 and isnull(company_url,'')<>'' and company_enddate>getdate() and company_ismultitenant=1";
                            //and (company_isschedule=1 or company_isonesignalnotificationenabled=1)";
            DataTable dttbl = ExecuteSelect(query, AppConstantsWinform.RPlusMasterConnectionString);
            for (int i = 0; i < dttbl.Rows.Count; i++)
            {
                string url = Convert.ToString(dttbl.Rows[i]["company_url"]);
                string subdomain = Convert.ToString(dttbl.Rows[i]["company_subdomain"]);
                if (dttbl.Rows[i]["company_isonesignalnotificationenabled"] != DBNull.Value 
                    && Convert.ToBoolean(dttbl.Rows[i]["company_isonesignalnotificationenabled"]))
                {
                    string url1 = "http://rpluscrm_master_v8.rpluscrm.in/RplusSchedulerHandler.ashx?sd=" + subdomain + "&type=onesignal";
                    BrowsePage(url1);
                    Thread.Sleep(500);
                }
                url = url + "/RplusSchedulerHandler.ashx?sd=" + subdomain;
                BrowsePage(url);
                Thread.Sleep(500);
            }
        }
        private void StartSingleSite()
        {
            string url = ConfigurationSettings.AppSettings["URL"];
            url = url + "/RplusSchedulerHandler.ashx";
            BrowsePage(url);
        }
        private void BrowsePage(string url)
        {
            try
            {
                if (!url.StartsWith("http://") && !url.StartsWith("https://")) url = "http://" + url;
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                if (responseFromServer == "Ok")
                {
                }
                else
                {
                    if (responseFromServer != "")
                    {
                        ErrorLog.WriteLog(url + ": "+ responseFromServer);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.WriteLog(url + ":" + ex.Message);
                SchedulerLog("Error");
            }
            finally
            {
                //Environment.Exit(0);
            }
        }
        public void SchedulerLog(string msg)
        {
            return;
            string query = "insert into tbl_schedulerlog(schedulerlog_message,schedulerlog_createddate) values('" + msg + "',getdate());";
            //ExecuteQuery(query);
        }
        //private void ExecuteQuery(string query)
        //{
        //    ExecuteQuery(query, "");
        //}
        private void ExecuteQuery(string query, string connectionString)
        {
            try
            {
                //if (connectionString == "") connectionString = ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.WriteLog(ex.Message + Environment.NewLine + query);
            }
        }
        //private DataTable ExecuteSelect(string query)
        //{
        //    return ExecuteSelect(query, "");
        //}
        private DataTable ExecuteSelect(string query, string connectionString)
        {
            DataTable dttbl = new DataTable();
            try
            {
                //if (connectionString == "") connectionString = ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.Fill(dttbl);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.WriteLog(ex.Message + Environment.NewLine + query);
            }
            return dttbl;
        }
        //private DataRow ExecuteSelectRow(string query)
        //{
        //    DataTable dttbl = ExecuteSelect(query);
        //    if (dttbl == null || dttbl.Rows.Count == 0) return null;
        //    return dttbl.Rows[0];
        //}
    }
}