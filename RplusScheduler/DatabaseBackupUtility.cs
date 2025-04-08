using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Configuration;
using System.IO;

namespace WebComponent
{
    public class DatabaseBackupUtility
    {
        public void BackupDatabaseInBlob()
        {
            try
            {
                string apiUrl = ConfigurationManager.AppSettings["RPlusCRMWebAPI"];
                apiUrl += "/azureBlobFileUpload/DatabaseBackup";
                WebRequest request = WebRequest.Create(apiUrl);
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                request.ContentType = "application/json";
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string responseFromServer = reader.ReadToEnd();
                ErrorLog.WriteLog("BackupDatabaseInBlob:Response:" + responseFromServer);
            }
            catch (Exception ex)
            {
                ErrorLog.WriteLog("BackupDatabaseInBlob:Error:" + ex.Message);
            }
        }
    }
}
