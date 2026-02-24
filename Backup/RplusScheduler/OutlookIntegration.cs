using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using OpenPop.Pop3;
using OpenPop.Mime;
using System.IO;
using System.Collections;
using System.Configuration;
using System.Threading;
using WebComponent;

namespace RplusScheduler
{
    public class OutlookIntegration
    {
        public void ReadEmail()
        {
            AppConstantsWinform.InitEmailIntegration();

            Pop3Client pop3Client;
            string strSubject = "";
            string emailBody = "";
            string _webAppPath = "";
            Console.WriteLine("Running...");
            pop3Client = new Pop3Client();
            pop3Client.Connect(AppConstantsWinform.PopHostName, AppConstantsWinform.PopPort, AppConstantsWinform.PopSSL);
            Console.WriteLine("Connected to " + AppConstantsWinform.PopHostName);
            pop3Client.Authenticate(AppConstantsWinform.PopUserName, AppConstantsWinform.PopPassword, AuthenticationMethod.UsernameAndPassword);

            int count = pop3Client.GetMessageCount();
            ErrorLog.WriteLog("OutlookIntegration:ReadEmail : Started : " + count);
            if (count > 0)
            {
                Console.WriteLine("Reading...");
                _webAppPath = AppConstants.WinformWebAppSAASPath;
                string query = "";
                query = @"select company_subdomain,company_emailintegrationenquiryfromemailid,company_emailintegrationcomplaintfromemailid 
                            from tbl_company where company_isactive=1 and company_isemailintegration=1 
                           and (isnull(company_emailintegrationenquiryfromemailid,'')<>'' OR isnull(company_emailintegrationcomplaintfromemailid,'')<>'')";
                DataTable dttblcompany = DbTable.ExecuteSelect(query);
                if (dttblcompany.Rows.Count == 0)
                {
                    count = 0;
                }
                bool isMultiTenant = AppConstantsWinform.IsMultitenant;
                for (int i = count; i >= 1; i--)
                {
                    try
                    {
                        OpenPop.Mime.Message message = pop3Client.GetMessage(i);
                        string strUID = pop3Client.GetMessageUid(i);

                        if (strUID == null || strUID == "")
                            return;
                        if (message.Headers.Subject != null)
                        {
                            strSubject = message.Headers.Subject;
                        }
                        if (message.Headers.From.MailAddress == null)
                        {
                            pop3Client.DeleteMessage(i);
                            continue;
                        }
                        string fromEmailId = message.Headers.From.MailAddress.Address;
                        string toEmailId = "";// message.Headers.To[0].MailAddress.Address;
                        int clientEmailTypeId = 0;
                        string connectionString = "";
                        string enquiryEmailId = ""; string complaintEmailId = "";
                        bool isvalid = false;
                        string subdomain = "";
                        if (isMultiTenant)
                        {
                            for (int j = 0; j < message.Headers.To.Count; j++)
                            {
                                toEmailId = message.Headers.To[j].MailAddress.Address;
                                for (int k = 0; k < dttblcompany.Rows.Count; k++)
                                {
                                    enquiryEmailId = GlobalUtilities.ConvertToString(dttblcompany.Rows[k]["company_emailintegrationenquiryfromemailid"]);
                                    complaintEmailId = GlobalUtilities.ConvertToString(dttblcompany.Rows[k]["company_emailintegrationcomplaintfromemailid"]);
                                    if (enquiryEmailId == toEmailId || complaintEmailId == toEmailId)
                                    {
                                        subdomain = GlobalUtilities.ConvertToString(dttblcompany.Rows[k]["company_subdomain"]);
                                        if (AppConstants.IsLive)
                                        {
                                            string subdomainDb = subdomain;
                                            if (!subdomain.ToLower().Contains("_v9"))
                                            {
                                                subdomainDb = "RPlusCRM_" + subdomain + "_V9";
                                            }
                                            connectionString = AppConstantsWinform.RPlusMasterConnectionString.Replace(AppConstantsWinform.RPlusMasterDB, subdomainDb);
                                        }
                                        else
                                        {
                                            connectionString = AppConstantsWinform.RPlusMasterConnectionString;
                                        }
                                        if (enquiryEmailId == toEmailId)
                                        {
                                            clientEmailTypeId = 1;
                                        }
                                        else
                                        {
                                            clientEmailTypeId = 2;
                                        }
                                        isvalid = true;
                                        break;
                                    }
                                }
                                if (isvalid) break;
                            }
                        }
                        else
                        {
                            enquiryEmailId = CommonR.GetSetting("Email Integration Enquiry From Email Id");
                            complaintEmailId = CommonR.GetSetting("Email Integration Complaint From Email Id");
                            for (int j = 0; j < message.Headers.To.Count; j++)
                            {
                                toEmailId = message.Headers.To[j].MailAddress.Address;
                                if (enquiryEmailId == toEmailId || complaintEmailId == toEmailId)
                                {
                                    if (enquiryEmailId == toEmailId)
                                    {
                                        clientEmailTypeId = 1;
                                    }
                                    else
                                    {
                                        clientEmailTypeId = 2;
                                    }
                                    isvalid = true;
                                    break;
                                }
                            }
                        }

                        if (isvalid)
                        {
                            isvalid = CommonR.GetSettingBool("Is Email Integration Enabled");
                        }
                        if (isvalid)
                        {
                            if (clientEmailTypeId == 1)//enquiry
                            {
                                isvalid = CommonR.GetSettingBool("Is Enquiry Email Integration Enabled");
                            }
                            else if (clientEmailTypeId == 2)
                            {
                                isvalid = CommonR.GetSettingBool("Is Complaint Email Integration Enabled");
                            }
                        }
                        if (isvalid)
                        {
                            fromEmailId = global.CheckInputData(fromEmailId);
                            query = "select top 1 * from tbl_contacts where contacts_clientid>0 and contacts_emailid='" + fromEmailId
                                    + "' order by contacts_contactsid desc";
                            DataRow drcontact = DbTable.ExecuteSelectRowWithCustomConnection(query, connectionString);

                            if (clientEmailTypeId == 2)
                            {
                                if (drcontact == null)
                                {
                                    isvalid = false;
                                }
                            }
                            if (isvalid)
                            {
                                int clientId = 0;
                                string mobileNo = "";
                                string contactPerson = message.Headers.From.DisplayName;
                                int clientEmailId = 0;
                                int clientEmailDetailId = 0;
                                string ccEmailId = "";
                                string code = "";
                                int isnew = 0;
                                //emailUniqueId = GetEmailUniqueId(emailBody);
                                code = GetCodeFromSubject(strSubject);
                                if (code != "")
                                {
                                    Hashtable hstblp = new Hashtable();
                                    hstblp.Add("code", code);
                                    query = "select clientemail_clientemailid,clientemail_code from tbl_clientemail where clientemail_code=@code";
                                    DataRow dre = DbTable.ExecuteSelectRow(query, connectionString, false, hstblp);
                                    if (dre == null)
                                    {
                                        code = "";
                                    }
                                    else
                                    {
                                        code = GlobalUtilities.ConvertToString(dre["clientemail_code"]);
                                        clientEmailId = GlobalUtilities.ConvertToInt(dre["clientemail_clientemailid"]);
                                    }
                                }

                                if (code == "")
                                {
                                    Hashtable hstble = new Hashtable();
                                    code = GetClientEmailCode(connectionString);
                                    hstble.Add("uniqueid", Guid.NewGuid().ToString());
                                    hstble.Add("code", code);
                                    hstble.Add("date", "getdate()");
                                    //hstble.Add("createddate", "getdate()");
                                    //hstble.Add("createdby", "0");
                                    hstble.Add("subject", strSubject);
                                    hstble.Add("initialsubject", strSubject);
                                    hstble.Add("fromemailid", fromEmailId);
                                    hstble.Add("fromemailname", contactPerson);
                                    hstble.Add("emailid", fromEmailId);
                                    hstble.Add("contactperson", contactPerson);
                                    hstble.Add("toemailid", toEmailId);
                                    hstble.Add("ccemailid", ccEmailId);
                                    hstble.Add("clientemailtypeid", clientEmailTypeId);
                                    hstble.Add("isread", "0");
                                    hstble.Add("statusid", "1");
                                    hstble.Add("isnew", "1");
                                    hstble.Add("isdeleted", "0");
                                    hstble.Add("lastupdatedate", "getdate()");
                                    if (drcontact != null)
                                    {
                                        clientId = Convert.ToInt32(drcontact["contacts_clientid"]);
                                        mobileNo = GlobalUtilities.ConvertToString(drcontact["contacts_mobileno"]);
                                        hstble.Add("clientid", clientId);
                                        hstble.Add("mobileno", mobileNo);
                                    }
                                    isnew = 1;
                                    InsertUpdate obje = new InsertUpdate();
                                    clientEmailId = obje.InsertData(hstble, "tbl_clientemail", connectionString);
                                }

                                if (clientEmailId == 0)
                                {
                                    ErrorLog.WriteLog("Error occurred while creating clientemail: " + strSubject);
                                    continue;
                                }
                                // get Email Body as Plain Text 
                                MessagePart body = message.FindFirstHtmlVersion();
                                if (body == null)
                                {
                                    body = message.FindFirstPlainTextVersion();
                                    emailBody = body.GetBodyAsText();
                                }
                                else
                                {
                                    emailBody = body.GetBodyAsText();
                                }

                                string shortDescription = GlobalUtilities.ConvertHtmlToText(emailBody, 100);
                                Hashtable hstbl = new Hashtable();
                                string uniqueId = Guid.NewGuid().ToString();
                                hstbl.Add("clientemailid", clientEmailId);
                                hstbl.Add("date", "getdate()");
                                //hstbl.Add("createddate", "getdate()");
                                //hstbl.Add("createdby", "0");
                                hstbl.Add("subject", strSubject);
                                hstbl.Add("body", emailBody);
                                hstbl.Add("fromemailid", fromEmailId);
                                hstbl.Add("toemailid", toEmailId);
                                hstbl.Add("fromemailname", contactPerson);
                                hstbl.Add("uniqueid", uniqueId);
                                hstbl.Add("userid", "0");
                                hstbl.Add("isclient", "1");
                                hstbl.Add("shortdescription", shortDescription);
                                InsertUpdate obj = new InsertUpdate();
                                if (AppConstantsWinform.IsMultitenant)
                                {
                                    clientEmailDetailId = obj.InsertData(hstbl, "tbl_clientemaildetail", connectionString);
                                }
                                else
                                {
                                    clientEmailDetailId = obj.InsertData(hstbl, "tbl_clientemaildetail");
                                }
                                if (clientEmailDetailId > 0)
                                {
                                    if (strUID != null && strUID != "")
                                    {
                                        try
                                        {
                                            List<MessagePart> attachments = message.FindAllAttachments();
                                            if (attachments.Count > 0)
                                            {
                                                StringBuilder attachmentNames = new StringBuilder();
                                                string uploadFolderPath = _webAppPath + "/";
                                                if (subdomain == "" || subdomain == "rpluscrm_master_v8")
                                                {
                                                    uploadFolderPath += "upload";
                                                }
                                                else
                                                {
                                                    uploadFolderPath += subdomain + "/upload";
                                                }
                                                uploadFolderPath += "/clientemail/" + uniqueId;
                                                if (!Directory.Exists(uploadFolderPath)) Directory.CreateDirectory(uploadFolderPath);
                                                foreach (MessagePart attachment in attachments)
                                                {
                                                    string fileName = Guid.NewGuid().ToString() + "_" + attachment.FileName;
                                                    string filePath = uploadFolderPath + "/" + fileName;
                                                    File.WriteAllBytes(filePath, attachment.Body);
                                                    string newFileName = GlobalUtilities.GetFileNameWithSize(filePath, fileName) + "," + fileName;
                                                    if (attachmentNames.ToString() == "")
                                                    {
                                                        attachmentNames.Append(newFileName);
                                                    }
                                                    else
                                                    {
                                                        attachmentNames.Append("|" + newFileName);
                                                    }
                                                }
                                                query = @"update tbl_clientemaildetail set clientemaildetail_attachment='" + attachmentNames.ToString() + @"'
                                                            where clientemaildetail_clientemaildetailid=" + clientEmailDetailId;
                                                DbTable.ExecuteQuery(query);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                    }

                                    query = @"update tbl_clientemail set clientemail_isread=0,clientemail_lastupdatedate=getdate(),
                                            clientemail_isnew=" + isnew + @",clientemail_lastclientemaildetailid=" + clientEmailDetailId + @" 
                                            where clientemail_clientemailid=" + clientEmailId;
                                    DbTable.ExecuteQuery(query);
                                    RPlusLiveClientEmail objclientemail = new RPlusLiveClientEmail();
                                    objclientemail.UpdateClientEmailUnReadCount(clientEmailTypeId, connectionString);
                                }
                                if (clientEmailId > 0 && isnew == 0)
                                {
                                    query = @"select clientemail_enquiryid,clientemail_complaintid from tbl_clientemail
                                            clientemail_clientemailid=" + clientEmailId;
                                    DataRow dremail = DbTable.ExecuteSelectRow(query);
                                    int enquiryId = GlobalUtilities.ConvertToInt(dremail["clientemail_enquiryid"]);
                                    int complaintId = GlobalUtilities.ConvertToInt(dremail["clientemail_complaintid"]);
                                    if (complaintId > 0 || enquiryId > 0)
                                    {
                                        string submodule = "";
                                        int submoduleId = 0;
                                        if (complaintId > 0)
                                        {
                                            submodule = "complaint";
                                            submoduleId = complaintId;
                                        }
                                        else
                                        {
                                            submodule = "enquiry";
                                            submoduleId = enquiryId;
                                        }
                                        query = "update tbl_" + submodule + " set " + submodule + "_statusid=" + (int)EnumStatus.WaitingSupportResponse +
                                                      " where " + submodule + "_" + submodule + "id=" + submoduleId;
                                        DbTable.ExecuteQuery(query);
                                    }
                                }
                                if (clientEmailId > 0 && isnew == 1)
                                {
                                    if (clientEmailTypeId == 1)//enquiry
                                    {
                                        bool isEmailAutoEnquiry = CommonR.GetSettingBool("Is Email Integration Auto Enquiry");
                                        if (isEmailAutoEnquiry)
                                        {
                                            CreateEnquiryComplaint(connectionString, true, clientEmailId, strUID, clientId, contactPerson, fromEmailId,
                                                            mobileNo, strSubject, emailBody);
                                        }
                                    }
                                    else if (clientEmailTypeId == 2)//complaint
                                    {
                                        bool isEmailAutoComplaint = CommonR.GetSettingBool("Is Email Integration Auto Complaint");
                                        if (isEmailAutoComplaint)
                                        {
                                            CreateEnquiryComplaint(connectionString, false, clientEmailId, strUID, clientId, contactPerson, fromEmailId,
                                                            mobileNo, strSubject, emailBody);
                                        }
                                    }
                                }
                            }
                        }
                        pop3Client.DeleteMessage(i);
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.WriteLog(ex.ToString());
                        continue;
                    }
                }
            }
            pop3Client.Disconnect();
            ErrorLog.WriteLog("OutlookIntegration:ReadEmail : Completed");
            //Console.Read();
            Environment.Exit(0);
            //Thread.Sleep(60000);
            //ReadEmail();
        }
        private string GetEmailUniqueId(string body)
        {
            int startIndex = body.IndexOf("emailuniqueid='");
            if (startIndex < 0) return "";
            int endIndex = body.IndexOf("'", startIndex + 11);
            if (endIndex < 0) return "";
            string uniqueId = body.Substring(startIndex + 11, endIndex - startIndex - 11);
            return uniqueId;
        }
        private string GetCodeFromSubject(string subject)
        {
            int startIndex = subject.IndexOf("##");
            if (startIndex > 0)
            {
                int endIndex = subject.IndexOf("##", startIndex + 3);
                if (endIndex > startIndex)
                {
                    string code = subject.Substring(startIndex + 2, endIndex - startIndex - 2);
                    return code;
                }
            }
            return "";
        }
        private string GetClientEmailCode(string connectionString)
        {
            string query = "";
            query = "select top 1 clientemail_code from tbl_clientemail order by clientemail_clientemailid desc";
            DataRow drc = DbTable.ExecuteSelectRow(query, connectionString, false);
            int counter = 1;
            if (drc != null) counter = Convert.ToInt32(Convert.ToString(drc["clientemail_code"]).Replace("ER-", "")) + 1;
            string code = "ER-" + counter;
            return code;
        }
        private int CreateEnquiryComplaint(string connectionString, bool isEnquiry, int clientEmailId, string emailUniqueId, int clientId, string contactPerson, 
            string emailId, string mobileNo, string subject, string description)
        {
            string code = "";
            string query = "";
            int counter = 1;
            if (isEnquiry)
            {
                query = "select top 1 enquiry_code from tbl_enquiry order by enquiry_enquiryid desc";
                DataRow drc = DbTable.ExecuteSelectRow(query);
                if (drc != null) counter = Convert.ToInt32(Convert.ToString(drc["enquiry_code"]).Replace("E-", "")) + 1;
                code = "E-" + counter;
            }
            else
            {
                query = "select top 1 complaint_code from tbl_complaint order by complaint_complaintid desc";
                DataRow drc = DbTable.ExecuteSelectRow(query);
                if (drc != null) counter = Convert.ToInt32(Convert.ToString(drc["complaint_code"]).Replace("C-", "")) + 1;
                code = "C-" + counter;
            }

            Hashtable hstbl = new Hashtable();
            hstbl.Add("sourcetypeid", "1");//email
            hstbl.Add("clientemailid", clientEmailId);
            hstbl.Add("emailuniqueid", emailUniqueId);
            hstbl.Add("uniqueid", Guid.NewGuid().ToString());
            hstbl.Add("clientid", clientId);
            hstbl.Add("contactperson", contactPerson);
            hstbl.Add("emailid", emailId);
            hstbl.Add("mobileno", mobileNo);
            hstbl.Add("subject", subject);
            hstbl.Add("description", description);
            hstbl.Add("code", code);
            hstbl.Add("statusid", "1");
            if (isEnquiry)
            {
                hstbl.Add("enquiryno", code);
                hstbl.Add("enquirydate", "getdate()");
            }
            else
            {
                hstbl.Add("ticketno", code);
                hstbl.Add("complaintdate", "getdate()");
            }
            InsertUpdate obj = new InsertUpdate();
            int id = 0;
            if (isEnquiry)
            {
                id = obj.InsertData(hstbl, "tbl_enquiry", connectionString);
                query = "update tbl_clientemail set clientemail_enquiryid=" + id + 
                    " where clientemail_clientemailid=" + clientEmailId;
                DbTable.ExecuteQuery(query, connectionString, false);
            }
            else
            {
                id = obj.InsertData(hstbl, "tbl_complaint", connectionString);
                query = "update tbl_clientemail set clientemail_complaintid=" + id +
                    " where clientemail_clientemailid=" + clientEmailId;
                DbTable.ExecuteQuery(query, connectionString, false);
            }
            return id;
        }
    }
}
