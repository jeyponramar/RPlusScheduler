using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Windows.Forms;
using WebComponent;
using System.Data;

namespace RplusScheduler
{
    public class AppConstantsWinform
    {
        public static bool IsMultitenant = true;
        public static string RPlusMasterConnectionString = ConfigurationManager.ConnectionStrings["RPlusMasterConnectionString"].ConnectionString;
        public static string ConnectionString = "";//ConfigurationSettings.AppSettings["ConnectionString"];
        public static string CompanyName = "";
        public static string RPlusMasterDB = "";

        //EMail Integration
        public static string PopHostName = ConfigurationSettings.AppSettings["PopHostName"];
        public static int PopPort = Convert.ToInt32(ConfigurationSettings.AppSettings["PopPort"]);
        public static string PopUserName = ConfigurationSettings.AppSettings["PopUserName"];
        public static string PopPassword = ConfigurationSettings.AppSettings["PopPassword"];
        public static bool PopSSL = Convert.ToBoolean(ConfigurationSettings.AppSettings["PopSSL"]);

        public static string Subdomain = "";
        public static void Init()
        {
            if (ConfigurationSettings.AppSettings["CompanyName"] != null)
            {
                CompanyName = ConfigurationSettings.AppSettings["CompanyName"];
            }
            RPlusMasterDB = ConfigurationSettings.AppSettings["RPlusMasterDB"];
            AppConstants.WinformWebAppSAASPath = ConfigurationManager.AppSettings["RPlusSAASPath"];
        }
        public static void InitEmailIntegration()
        {
            //EMail Integration
            PopHostName = ConfigurationSettings.AppSettings["PopHostName"];
            PopPort = Convert.ToInt32(ConfigurationSettings.AppSettings["PopPort"]);
            PopUserName = ConfigurationSettings.AppSettings["PopUserName"];
            PopPassword = ConfigurationSettings.AppSettings["PopPassword"];
            PopSSL = Convert.ToBoolean(ConfigurationSettings.AppSettings["PopSSL"]);
        }
        //public static bool IsIndiaMart_API_Enabled
        //{
        //    get
        //    {
        //        if (IndiaMart_API_URL != "") return true;
        //        return false;
        //    }
        //}
        
        public static string GetChildConnectionString(string subdomain)
        {
            return RPlusMasterConnectionString.Replace("RPlusCRM_Master_V8", ChildDatabase(subdomain));
        }
        public static string ApplicationPath = Application.StartupPath.Replace("\\bin\\Debug", "").Replace("\\bin\\Release", "") + "\\";
        public static string ChildDatabase(string subdomain)
        {
            return "RPlusCRM_" + subdomain + "_V8";
        }
    }
}
