using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Data;
using System.Collections;
using WebComponent;

namespace RplusScheduler
{
    public class TradeIndiaAPI
    {
        private string _ApiPrimaryUrl = "https://www.tradeindia.com/utils/my_inquiry.html?$API_KEY$&from_date=$START_DATE$&to_date=$END_DATE$";
        public void Process(string subdomain, string apiKey)
        {
            ErrorLog.WriteLog("TradeIndiaAPI Started");
            if (apiKey == "")
            {
                apiKey = GlobalUtilitiesWinform.GetSetting("TRADE_INDIA_API_KEY");
            }
            string startDate = GlobalUtilitiesWinform.GetSetting("TRADE_INDIA_API_LAST_CALL_DATE");
            DateTime dtEndDate = DateTime.Now;

            if (startDate == "")
            {
                DateTime dt = DateTime.Now.AddDays(-1);
                dt = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 1);
                startDate = String.Format("{0:yyyy-MM-dd HH:mm:ss}", dt);
            }
            else
            {
                DateTime dtstart = Convert.ToDateTime(startDate);
                DateTime dtend = dtstart.AddDays(1);//greather than 24 hours not allowed for inquiries
                if (dtend > DateTime.Now)
                {
                    dtend = DateTime.Now;
                }
                dtEndDate = dtend;
                startDate = String.Format("{0:yyyy-MM-dd HH:mm:ss}", dtstart);
            }
            string endDate = String.Format("{0:yyyy-MM-dd HH:mm:ss}", dtEndDate);
            string apiUrl = _ApiPrimaryUrl.Replace("$API_KEY$", apiKey);
            apiUrl = apiUrl.Replace("$START_DATE$", startDate);
            apiUrl = apiUrl.Replace("$END_DATE$", endDate);
            string responseFromServer = "";
            //ErrorLog.WriteLog(apiUrl);
            List<TradeIndiaAPILead> lstdata = new List<TradeIndiaAPILead>();
            int count = 0;
            try
            {
                ErrorLog.WriteLog("TradeIndia API Url: " + apiUrl);
                WebRequest request = WebRequest.Create(apiUrl);
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                request.ContentType = "application/json";
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                responseFromServer = reader.ReadToEnd();

                if (responseFromServer.Length < 10)
                {
                    GlobalUtilitiesWinform.UpdateSetting("TRADE_INDIA_API_LAST_CALL_DATE", String.Format("{0:dd-MMM-yyyy hh:mm:ss tt}", dtEndDate));
                    ErrorLog.WriteLog("TradeIndiaAPI Completed with No data;" + responseFromServer + ";" + apiUrl);
                    return;
                }
                string logFolder = ErrorLog.ApplicationPath + "/log/" + AppConstants.WinformSubdomain + "/TradeIndiaResponse";
                if (!Directory.Exists(logFolder)) Directory.CreateDirectory(logFolder);
                DirectoryInfo dir = new DirectoryInfo(logFolder);
                DateTime dtMaxLogDate = DateTime.Now.AddMonths(-1);
                foreach (FileInfo file in dir.GetFiles())
                {
                    if (file.CreationTime < dtMaxLogDate)
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch (Exception e1) { }
                    }
                }
                
                File.WriteAllText(logFolder + "/" + endDate.Replace(":", "-").Replace(" ", "-") + ".txt", responseFromServer);

                bool issuccess = ProcessData(subdomain, responseFromServer);
                if (issuccess)
                {
                    dtEndDate = dtEndDate.AddHours(-8);//8hrs back to avoid loss of data
                    GlobalUtilitiesWinform.UpdateSetting("TRADE_INDIA_API_LAST_CALL_DATE", String.Format("{0:dd-MMM-yyyy hh:mm:ss tt}", dtEndDate));
                    ErrorLog.WriteLog("TradeIndiaAPI Completed");
                }
                else
                {
                    ErrorLog.WriteLog("TradeIndiaAPI Completed with ERROR");
                }
            }
            catch (Exception ex)
            {
                ErrorLog.WriteLog("TradeIndiaError:" + apiUrl + Environment.NewLine + ex.Message);
            }
        }

        public bool ProcessData(string subdomain, string responseFromServer)
        {
            List<TradeIndiaAPILead> lstdata = new List<TradeIndiaAPILead>();
            int count = 0;
            //responseFromServer = System.IO.File.ReadAllText(@"G:\Ram\Projects\Doc\ClientRequirement\Leelavati\sample_api_data.txt");
            int defaultEnquiryAssignedTo = GlobalUtilitiesWinform.ConvertToInt(GlobalUtilitiesWinform.GetSetting("Default Trade India Enquiry Assigned To"));
            string query = "";
            int campaignId = 0;
            query = "select top 1 * from tbl_campaign where replace(campaign_campaignname,' ','')='TradeIndia'";
            DataRow drcampaign = DbTableWinform.ExecuteSelectRow(query);
            if (drcampaign != null) campaignId = GlobalUtilitiesWinform.ConvertToInt(drcampaign["campaign_campaignid"]);

            try
            {
                lstdata = ConvertJSONToTradeIndiaResponse(responseFromServer);
            }
            catch (Exception ex)
            {
                ErrorLog.WriteLog("TradeIndiaAPI ERROR in JSON Parsing: " + Environment.NewLine + responseFromServer);
                return false;
            }
            ErrorLog.WriteLog("TradeIndiaAPI " + lstdata.Count + " data found");
            foreach (TradeIndiaAPILead data in lstdata)
            {
                string queryId = "";
                try
                {
                    query = "select top 1 enquiry_enquiryid from tbl_enquiry where enquiry_thirdpartyapiid=2 and enquiry_apireferenceno='" + data.rfi_id + "'";
                    DataRow drexist = DbTableWinform.ExecuteSelectRow(query);
                    if (drexist != null) continue;
                    StringBuilder apidata = new StringBuilder();
                    string qType = GlobalUtilitiesWinform.ConvertToString(data.inquiry_type);
                    string clientName = GlobalUtilitiesWinform.ConvertToString(data.sender_co);
                    string stateName = GlobalUtilitiesWinform.ConvertToString(data.sender_state);
                    string cityName = GlobalUtilitiesWinform.ConvertToString(data.sender_city);
                    string productName = GlobalUtilitiesWinform.ConvertToString(data.product_name);
                    string message = GlobalUtilitiesWinform.ConvertToString(data.message).Replace("<br>", "\n");
                    string subject = GlobalUtilitiesWinform.ConvertToString(data.subject);
                    string contactPerson = GlobalUtilitiesWinform.ConvertToString(data.sender_name);
                    string mobileNo = GlobalUtilitiesWinform.ConvertToString(data.sender_mobile).Replace("+91-", "");
                    string emailId = GlobalUtilitiesWinform.ConvertToString(data.sender_email);
                    string landlineNo = GlobalUtilitiesWinform.ConvertToString(data.landline_number);
                    string address = GlobalUtilitiesWinform.ConvertToString(data.address).Replace("<br>", "\n");
                    //if (message == "") continue;//this is just a phone call
                    queryId = data.rfi_id;
                    //if (queryId == "2179490272")
                    //{
                    //    queryId = queryId;
                    //}
                    int stateId = DbTableWinform.GetMasterId("state", "statename", stateName);
                    int cityId = DbTableWinform.GetMasterId("city", "cityname", cityName);

                    apidata.Append("Company : " + clientName + Environment.NewLine);
                    apidata.Append("State : " + stateName + Environment.NewLine);
                    apidata.Append("City : " + cityName + Environment.NewLine);
                    apidata.Append("Product : " + productName + Environment.NewLine);

                    query = "select top 1 enquiry_code from tbl_enquiry order by enquiry_enquiryid desc";
                    DataRow dr = DbTableWinform.ExecuteSelectRow(query);
                    int counter = 1;
                    if (dr != null) counter = Convert.ToInt32(Convert.ToString(dr["enquiry_code"]).Replace("E-", "")) + 1;
                    string enquiryNo = "E-" + counter;
                    int clientAssignedTo = 0;
                    int assignedTo = 0;
                    int clientId = SaveClient(queryId, clientName, contactPerson, mobileNo, emailId, landlineNo, address, stateId, cityId, out clientAssignedTo);
                    if (clientAssignedTo > 0)
                    {
                        assignedTo = clientAssignedTo;
                    }
                    else
                    {
                        assignedTo = defaultEnquiryAssignedTo;
                    }
                    Hashtable hstbl = new Hashtable();
                    hstbl.Add("apireferenceno", queryId);
                    hstbl.Add("referenceno", queryId);
                    hstbl.Add("thirdpartyapiid", 2);//Trade India
                    hstbl.Add("leadstatusid", "1");
                    hstbl.Add("campaignid", campaignId);
                    hstbl.Add("enquiryno", enquiryNo);
                    hstbl.Add("code", enquiryNo);
                    hstbl.Add("emailid", emailId);
                    hstbl.Add("contactperson", contactPerson);
                    hstbl.Add("mobileno", mobileNo);
                    hstbl.Add("subject", subject);
                    if (subdomain == "leelavati")
                    {
                        hstbl.Add("requirement", message);
                    }
                    else
                    {
                        hstbl.Add("description", message);
                    }
                    hstbl.Add("enquirydate_datetime", GetDateTime(data.generated_date, data.generated_time));//Convert.ToDateTime(data.QUERY_TIME));
                    hstbl.Add("address", address);
                    hstbl.Add("landlineno", landlineNo);
                    hstbl.Add("clientid", clientId);
                    //hstbl.Add("stateid", DbTable.GetMasterId("state", "statename", data.ENQ_STATE));
                    hstbl.Add("cityid", cityId);
                    hstbl.Add("apidata", apidata);
                    if (assignedTo > 0)
                    {
                        hstbl.Add("statusid", "3");
                        hstbl.Add("employeeid", assignedTo);
                        hstbl.Add("assigneddate_datetime", DateTime.Now);
                    }
                    else
                    {
                        hstbl.Add("statusid", "1");
                    }
                    int enquiryId = DbTableWinform.Insert(hstbl, "enquiry");
                    if (enquiryId > 0)
                    {
                        int productId = DbTableWinform.GetMasterId("product", "productname", productName);
                        if (productId > 0)
                        {
                            int quantity = GetQuantity(message);
                            if (quantity == 0) quantity = 1;
                            hstbl = new Hashtable();
                            hstbl.Add("productid", productId);
                            hstbl.Add("quantity", quantity);
                            hstbl.Add("enquiryid", enquiryId);
                            int detailId = DbTableWinform.Insert(hstbl, "enquirydetail", false);
                        }
                        hstbl = new Hashtable();
                        hstbl.Add("tasktypeid", 4);
                        hstbl.Add("description", message);
                        hstbl.Add("module", "enquiry");
                        hstbl.Add("mid", enquiryId);
                        hstbl.Add("subject", subject);
                        hstbl.Add("code", enquiryNo);
                        if (assignedTo > 0)
                        {
                            hstbl.Add("statusid", 3);
                            hstbl.Add("employeeid", assignedTo);
                            hstbl.Add("assigneddate_datetime", DateTime.Now);
                        }
                        else
                        {
                            hstbl.Add("statusid", 1);
                        }
                        int taskId = DbTableWinform.Insert(hstbl, "task");
                    }
                    else
                    {
                        ErrorLog.WriteLog("TradeIndiaAPI Error Inner Loop while creating enquiry for : " + queryId);
                    }
                    count++;
                }
                catch (Exception ex1)
                {
                    ErrorLog.WriteLog("TradeIndiaAPI Error Inner Loop : " + queryId + ":" + ex1.Message);
                    return false;
                }
            }
            return true;
        }

        private DateTime GetDateTime(string dt, string time)
        {
            try
            {
                return Convert.ToDateTime(dt + " " + time);
            }
            catch (Exception ex)
            {
                return DateTime.Now;
            }
        }
        private int SaveClient(string apiRefNo, string companyName, string contactPerson, string mobileNo, string emailId, string landlineNo,
            string address, int stateId, int cityId, out int clientAssignedTo)
        {
            clientAssignedTo = 0;
            try
            {
                string query = "";

                Hashtable hstbl = new Hashtable();
                string billingName = companyName;
                if (companyName.Trim() == "")
                {
                    query = "select * from tbl_client where client_mobileno=@mobileno";
                    hstbl.Add("mobileno", mobileNo);
                    companyName = contactPerson + " (" + mobileNo + ")";
                    billingName = contactPerson;
                }
                else
                {
                    query = "select * from tbl_client where client_customername=@customername";
                    hstbl.Add("customername", companyName);
                }
                DataRow dr = DbTableWinform.ExecuteSelectRow(query, hstbl);
                if (dr != null)
                {
                    clientAssignedTo = GlobalUtilitiesWinform.ConvertToInt(dr["client_employeeid"]);
                    return GlobalUtilitiesWinform.ConvertToInt(dr["client_clientid"]);
                }

                hstbl = new Hashtable();
                hstbl.Add("thirdpartyapiid", (int)EnumAPIType.TRADE_INDIA);
                hstbl.Add("apireferenceno", apiRefNo);
                hstbl.Add("customername", companyName);
                hstbl.Add("billingname", billingName);
                hstbl.Add("reference", "Trade India");
                hstbl.Add("isactive", 1);
                hstbl.Add("stateid", stateId);
                hstbl.Add("cityid", cityId);
                hstbl.Add("contactperson", contactPerson);
                hstbl.Add("mobileno", mobileNo);
                hstbl.Add("landlineno", landlineNo);
                hstbl.Add("emailid", emailId);
                hstbl.Add("address", address);
                hstbl.Add("billingaddress", address);
                int clientId = DbTableWinform.Insert(hstbl, "client");
                if (clientId <= 0) return 0;
                hstbl = new Hashtable();
                hstbl.Add("clientid", clientId);
                hstbl.Add("contactperson", contactPerson);
                hstbl.Add("mobileno", mobileNo);
                hstbl.Add("emailid", emailId);
                hstbl.Add("landlineno", landlineNo);
                hstbl.Add("module", "client");
                hstbl.Add("moduleid", clientId);
                hstbl.Add("ismaincontact", 1);
                hstbl.Add("contactgroupid", 1);
                int contactId = DbTableWinform.Insert(hstbl, "contacts");
                return clientId;
            }
            catch (Exception ex)
            {
                ErrorLog.WriteLog("Error in SaveClient: " + ex.Message);
                return 0;
            }
        }
        private int GetQuantity(string data)
        {
            try
            {
                if (data == "") return 0;
                int startIndex = data.IndexOf("Quantity : ");
                if (startIndex < 0) return 0;
                int endindex = data.IndexOf(" ", startIndex + 12);
                if (endindex < 0) return 0;
                string qty = data.Substring(startIndex + 11, endindex - startIndex - 11);
                return GlobalUtilitiesWinform.ConvertToInt(qty);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        private List<TradeIndiaAPILead> ConvertJSONToTradeIndiaResponse(string json)
        {
            List<TradeIndiaAPILead> data = new List<TradeIndiaAPILead>();
            data = JsonConvert.DeserializeObject<List<TradeIndiaAPILead>>(json);
            return data;
        }
    }
}
