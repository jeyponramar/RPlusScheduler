using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Collections;
using System.Data;
using WebComponent;

namespace RplusScheduler
{
    public class IndiaMartAPI
    {
        private string _ApiPrimaryUrl = "https://mapi.indiamart.com/wservce/crm/crmListing/v2/?glusr_crm_key=$API_KEY$&start_time=$START_DATE$&end_time=$END_DATE$";
        public void Process(string apiKey)
        {
            ErrorLog.WriteLog("IndiaMartAPI Started");
            if (apiKey == "")
            {
                apiKey = GlobalUtilitiesWinform.GetSetting("INDIA_MART_API_KEY");
            }
            string startDate = GlobalUtilitiesWinform.GetSetting("INDIA_MART_API_LAST_CALL_DATE");
            DateTime dtEndDate = DateTime.Now;
            
            if (startDate == "")
            {
                DateTime dt = DateTime.Now.AddDays(-6);
                dt = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 1);
                startDate = String.Format("{0:dd-MMM-yyyy hh:mm:ss tt}", dt);
            }
            else
            {
                DateTime dtstart = Convert.ToDateTime(startDate);
                DateTime dtend = dtstart.AddDays(6);
                if (dtend > DateTime.Now)
                {
                    dtend = DateTime.Now;
                }
                dtEndDate = dtend;
            }
            string endDate = String.Format("{0:dd-MMM-yyyy hh:mm:ss tt}", dtEndDate);
            string apiUrl = _ApiPrimaryUrl.Replace("$API_KEY$",apiKey);
            apiUrl = apiUrl.Replace("$START_DATE$", startDate);
            apiUrl = apiUrl.Replace("$END_DATE$", endDate);
            string responseFromServer = "";
            List<IndiaMartAPILead> lstdata = new List<IndiaMartAPILead>();
            int count = 0;
            try
            {
                ErrorLog.WriteLog("IndiaMart API Url: " + apiUrl); 
                WebRequest request = WebRequest.Create(apiUrl);
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                request.ContentType = "application/json";
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                responseFromServer = reader.ReadToEnd();

                if (responseFromServer.Contains("Error_Message"))
                {
                    if (responseFromServer.Contains("There are no leads"))
                    {
                        GlobalUtilitiesWinform.UpdateSetting("INDIA_MART_API_LAST_CALL_DATE", String.Format("{0:dd-MMM-yyyy hh:mm:ss tt}", dtEndDate));
                        ErrorLog.WriteLog("IndiaMartAPI Completed with No data");
                        return;
                    }
                    ErrorLog.WriteLog("IndiaMartError:" + apiUrl + Environment.NewLine + responseFromServer);
                    return;
                }
                string logFolder = ErrorLog.ApplicationPath + "/log/" + AppConstants.WinformSubdomain + "/IndiaMartResponse";
                if (!Directory.Exists(logFolder)) Directory.CreateDirectory(logFolder);
                DirectoryInfo dir = new DirectoryInfo(logFolder);
                DateTime dtMaxLogDate = DateTime.Now.AddMonths(-1);
                foreach(FileInfo file in dir.GetFiles())
                {
                    if (file.CreationTime < dtMaxLogDate)
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch(Exception e1){}
                    }
                }
                File.WriteAllText(logFolder + "/" + endDate.Replace(":", "-").Replace(" ", "-") + ".txt", responseFromServer);

                //responseFromServer = System.IO.File.ReadAllText(@"G:\Ram\Projects\Doc\ClientRequirement\Leelavati\sample_api_data.txt");
                int defaultEnquiryAssignedTo = GlobalUtilitiesWinform.ConvertToInt(GlobalUtilitiesWinform.GetSetting("Default India Mart Enquiry Assigned To"));
                string query = "";
                int campaignId = 0;
                query = "select top 1 * from tbl_campaign where replace(campaign_campaignname,' ','')='indiamart'";
                DataRow drcampaign = DbTableWinform.ExecuteSelectRow(query);
                if (drcampaign != null) campaignId = GlobalUtilitiesWinform.ConvertToInt(drcampaign["campaign_campaignid"]);

                lstdata = ConvertJSONToIndiaMartResponse(responseFromServer);
                ErrorLog.WriteLog("IndiaMartAPI " + lstdata.Count + " data found");
                foreach (IndiaMartAPILead data in lstdata)
                {
                    string queryId = "";
                    try
                    {
                        query = "select top 1 enquiry_enquiryid from tbl_enquiry where enquiry_thirdpartyapiid=1 and enquiry_apireferenceno='" + data.UNIQUE_QUERY_ID + "'";
                        DataRow drexist = DbTableWinform.ExecuteSelectRow(query);
                        if (drexist != null) continue;
                        StringBuilder apidata = new StringBuilder();
                        string qType = GlobalUtilitiesWinform.ConvertToString(data.QUERY_TYPE);
                        string clientName = GlobalUtilitiesWinform.ConvertToString(data.SENDER_COMPANY);
                        string stateName = GlobalUtilitiesWinform.ConvertToString(data.SENDER_STATE);
                        string cityName = GlobalUtilitiesWinform.ConvertToString(data.SENDER_CITY);
                        string productName = GlobalUtilitiesWinform.ConvertToString(data.QUERY_PRODUCT_NAME);
                        string message = GlobalUtilitiesWinform.ConvertToString(data.QUERY_MESSAGE).Replace("<br>", "\n");
                        string subject = GlobalUtilitiesWinform.ConvertToString(data.SUBJECT);
                        string contactPerson = GlobalUtilitiesWinform.ConvertToString(data.SENDER_NAME);
                        string mobileNo = GlobalUtilitiesWinform.ConvertToString(data.SENDER_MOBILE).Replace("+91-", "");
                        string emailId = GlobalUtilitiesWinform.ConvertToString(data.SENDER_EMAIL);
                        string landlineNo = GlobalUtilitiesWinform.ConvertToString(data.SENDER_PHONE);
                        string address = GlobalUtilitiesWinform.ConvertToString(data.SENDER_ADDRESS).Replace("<br>", "\n");
                        if (message == "") continue;//this is just a phone call
                        queryId = data.UNIQUE_QUERY_ID;
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
                        hstbl.Add("thirdpartyapiid", 1);//India Mart
                        hstbl.Add("leadstatusid", "1");
                        hstbl.Add("campaignid", campaignId);
                        hstbl.Add("enquiryno", enquiryNo);
                        hstbl.Add("code", enquiryNo);
                        hstbl.Add("emailid", emailId);
                        hstbl.Add("contactperson", contactPerson);
                        hstbl.Add("mobileno", mobileNo);
                        hstbl.Add("subject", subject);
                        hstbl.Add("description", message);
                        hstbl.Add("enquirydate_datetime", Convert.ToDateTime(data.QUERY_TIME));
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
                                hstbl.Add("statusid", 3);//assigned
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
                            ErrorLog.WriteLog("IndiaMartAPI Error Inner Loop while creating enquiry for : " + queryId);
                        }
                        count++;
                    }
                    catch (Exception ex1)
                    {
                        ErrorLog.WriteLog("IndiaMartAPI Error Inner Loop : " + queryId + ":" + ex1.Message);
                    }
                }
                GlobalUtilitiesWinform.UpdateSetting("INDIA_MART_API_LAST_CALL_DATE", String.Format("{0:dd-MMM-yyyy hh:mm:ss tt}", dtEndDate));
                ErrorLog.WriteLog("IndiaMartAPI Completed");
            }
            catch (Exception ex)
            {
                ErrorLog.WriteLog("IndiaMartError:" + apiUrl + Environment.NewLine + ex.Message);
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
                hstbl.Add("thirdpartyapiid", (int)EnumAPIType.INDIA_MART);
                hstbl.Add("apireferenceno", apiRefNo);
                hstbl.Add("customername", companyName);
                hstbl.Add("billingname", billingName);
                hstbl.Add("reference", "India Mart");
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
        private List<IndiaMartAPILead> ConvertJSONToIndiaMartResponse(string json)
        {
            IndiaMartAPIResponse m = JsonConvert.DeserializeObject<IndiaMartAPIResponse>(json);
            return m.RESPONSE;
        }
    }
}
