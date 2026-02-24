using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Data;
using WebComponent;

namespace RplusScheduler
{
    public class RPlusTempFolderCleanup
    {
        public void Run()
        {
            ErrorLog.WriteLog("Temp folder cleanup started");
            string folderPath = ConfigurationManager.AppSettings["CleanupTempFolderMainPath"];
            if (folderPath == null || folderPath == "") return;
            Array arr = folderPath.Split(',');
            for (int i = 0; i < arr.Length; i++)
            {
                folderPath = arr.GetValue(i).ToString();
                CleanupTempFolder(folderPath);
                CleanupLogFolder(folderPath);
                CleanupUpdateLogFolder(folderPath);
                CleanupDeletedFolder(folderPath);
            }
            ErrorLog.WriteLog("Temp folder cleanup completed");
        }
        private void CleanupTempFolder(string folderPath)
        {
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            folderPath = folderPath.ToLower();
            folderPath = folderPath.Replace("\\", "/");
            if (folderPath.Contains("/temp/") || folderPath.EndsWith("/temp"))
            {
                bool isfirst = true;
                int count = 0;
                foreach (FileInfo f in dir.GetFiles())
                {
                    if (isfirst)
                    {
                        ErrorLog.WriteLog("Temp Folder cleanup started for : " + folderPath);
                        isfirst = false;
                    }
                    try
                    {
                        f.Delete();
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.WriteLog("Unable to delete the file : " + f.FullName);
                    }
                    count++;
                }
                if (count > 0)
                {
                    ErrorLog.WriteLog("Temp Folder cleanup completed for : " + folderPath);
                }
            }
            foreach(DirectoryInfo sdir in dir.GetDirectories())
            {
                CleanupTempFolder(sdir.FullName);
            }
        }
        private void CleanupLogFolder(string folderPath)
        {
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            folderPath = folderPath.ToLower();
            folderPath = folderPath.Replace("\\", "/");
            if (!folderPath.Contains("updatelog") && !folderPath.Contains("deleteddata") && (folderPath.Contains("/log/") || folderPath.EndsWith("/log")))
            {
                int count = 0;
                foreach (FileInfo f in dir.GetFiles())
                {
                    TimeSpan ts = DateTime.Now - f.CreationTime;
                    if (f.Name.EndsWith(".txt") && ts.Days > 7)
                    {
                        if (count == 0)
                        {
                            ErrorLog.WriteLog("Log folder cleanup started for : " + folderPath);
                        }
                        try
                        {
                            count++;
                            f.Delete();
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.WriteLog("Unable to delete the log file : " + f.FullName);
                        }
                    }
                   
                }
                if (count > 0)
                {
                    ErrorLog.WriteLog("Log Folder cleanup completed for : " + folderPath);
                }
            }
            foreach (DirectoryInfo sdir in dir.GetDirectories())
            {
                CleanupLogFolder(sdir.FullName);
            }
        }
        private void CleanupUpdateLogFolder(string folderPath)
        {
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            folderPath = folderPath.ToLower();
            folderPath = folderPath.Replace("\\", "/");
            if (folderPath.Contains("/updatelog/") && !folderPath.Contains("deleteddata"))
            {
                int count = 0;
                foreach (FileInfo f in dir.GetFiles())
                {
                    TimeSpan ts = DateTime.Now - f.CreationTime;
                    if (f.Name.EndsWith(".log") && ts.Days > 365)
                    {
                        if (count == 0)
                        {
                            ErrorLog.WriteLog("Updatelog folder cleanup started for : " + folderPath);
                        }
                        try
                        {
                            count++;
                            f.Delete();
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.WriteLog("Unable to delete the update log file : " + f.FullName);
                        }
                    }

                }
                if (count > 0)
                {
                    ErrorLog.WriteLog("Updatelog Folder cleanup completed for : " + folderPath);
                }
            }
            foreach (DirectoryInfo sdir in dir.GetDirectories())
            {
                CleanupUpdateLogFolder(sdir.FullName);
            }
        }
        private void CleanupDeletedFolder(string folderPath)
        {
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            folderPath = folderPath.ToLower();
            folderPath = folderPath.Replace("\\", "/");
            if (folderPath.Contains("/deleteddata/") || folderPath.EndsWith("/deleteddata"))
            {
                int count = 0;
                foreach (FileInfo f in dir.GetFiles())
                {
                    TimeSpan ts = DateTime.Now - f.CreationTime;
                    if (f.Name.EndsWith(".txt") && ts.Days > 30)
                    {
                        if (count == 0)
                        {
                            ErrorLog.WriteLog("Deleted data folder cleanup started for : " + folderPath);
                        }
                        try
                        {
                            count++;
                            f.Delete();
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.WriteLog("Unable to delete the Deleted data log file : " + f.FullName);
                        }
                    }

                }
                if (count > 0)
                {
                    ErrorLog.WriteLog("Deleted data Folder cleanup completed for : " + folderPath);
                }
            }
            foreach (DirectoryInfo sdir in dir.GetDirectories())
            {
                CleanupDeletedFolder(sdir.FullName);
            }
        }
        
    }
}
