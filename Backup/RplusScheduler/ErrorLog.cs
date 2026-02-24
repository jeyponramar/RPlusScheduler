using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using WebComponent;

namespace RplusScheduler
{
    public class ErrorLog
    {
        public static string ApplicationPath = Application.StartupPath.Replace("\\bin\\Debug", "").Replace("\\bin\\Release", "");
        public static void WriteLog(string Message)
        {
            WriteLog(Message, true);
        }
        public static void WriteLog(string Message, bool includeSubDomain)
        {
            StreamWriter sw = null;
            string FilePath = "";
            string FolderPath = "";
            if (AppConstants.WinformSubdomain == "" || includeSubDomain == false)
            {
                FolderPath = ApplicationPath + "/log";
            }
            else
            {
                FolderPath = ApplicationPath + "/log/" + AppConstants.WinformSubdomain;
            }
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
            FilePath = FolderPath + "/" + DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + ".txt";

            try
            {
                Message = Environment.NewLine + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + " : " +  Message;
                sw = new StreamWriter(FilePath, true);
                sw.Write(Message);

            }
            catch { }
            finally
            {
                if (sw != null) sw.Close();
                sw = null;
            }
        }
    }
}
