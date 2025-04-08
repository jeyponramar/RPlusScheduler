using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;
using Newtonsoft.Json;
using WebComponent;
using System.Web;
using OpenPop.Pop3;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Linq;

namespace RplusScheduler
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //TestMail();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppConstants.IsWinform = true;

            AppConstants.ApplicationPath = AppConstantsWinform.ApplicationPath;
            AppConstants.CustomSettings = new CustomSettings();

            AppConstantsWinform.Init();

            //AppConstantsWinform.ConnectionString = ConfigurationSettings.AppSettings["ConnectionString"];
            AppConstantsWinform.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            //Console.WriteLine("Enter Path:");
            //string folder = Console.ReadLine();

            //ResizeImage(folder);
            //return;
            //RPlusWindowsScheduler objRPlusWindowsScheduler1 = new RPlusWindowsScheduler();
            //objRPlusWindowsScheduler1.RunScheduler(EnumSchedulerType.DailyTask);
            if (args.Length > 0)
            {
                string appname = "";
                appname = args.GetValue(0).ToString().Replace("application=", "");

                ErrorLog.WriteLog("Scheduler Started for " + appname);

                ErrorLog.WriteLog("IsMultitenant=" + AppConstantsWinform.IsMultitenant.ToString());

                if (appname == "thirdpartyapi")
                {
                    ProcessThirdpartyAPI();    
                }
                else if (appname == "tradeindia")
                {
                    ThirdPartyAPI obj = new ThirdPartyAPI();
                    obj.ProcessAPI(EnumAPIType.TRADE_INDIA);
                }
                else if (appname == "indiamart")
                {
                    ThirdPartyAPI obj = new ThirdPartyAPI();
                    obj.ProcessAPI(EnumAPIType.INDIA_MART);
                }
                else if (appname == "emailintegration")
                {
                    OutlookIntegration obj = new OutlookIntegration();
                    obj.ReadEmail();
                }
                //else if (appname == "tempfoldercleanup")
                //{
                //    RPlusTempFolderCleanup obj = new RPlusTempFolderCleanup();
                //    obj.Run();
                //}
                //else if (appname == "updatedocstoragesize")
                //{
                //    RPlusStorageCalculator obj = new RPlusStorageCalculator();
                //    obj.CalculateAllDatabaseSizeAndUpdate();
                //    obj.UpdateAllUploadFolderDocSize(); // only for first time
                //}
                else if (appname == "bulkemail" || appname == "bulksms" || appname == "bulkwhatsapp" || appname == "dailytask")
                {
                    RPlusWindowsScheduler objRPlusWindowsScheduler = new RPlusWindowsScheduler();
                    EnumSchedulerType schedulerType = EnumSchedulerType.BulkEmail;
                    if (appname == "bulksms")
                    {
                        schedulerType = EnumSchedulerType.BulkSMS;
                    }
                    else if (appname == "bulkwhatsapp")
                    {
                        schedulerType = EnumSchedulerType.BulkWhatsApp;
                    }
                    else if (appname == "dailytask")
                    {
                        schedulerType = EnumSchedulerType.DailyTask;
                    }
                    objRPlusWindowsScheduler.RunScheduler(schedulerType);
                }
                else if (appname == "rpluscrmadminutility")
                {
                    RPlusTempFolderCleanup obj = new RPlusTempFolderCleanup();
                    obj.Run();
                    //RPlusStorageCalculator objRPlusStorageCalculator = new RPlusStorageCalculator();
                    //objRPlusStorageCalculator.CalculateAllDatabaseSizeAndUpdate();
                    //objRPlusStorageCalculator.UpdateAllUploadFolderDocSize(); // only for first time
                    //DatabaseBackupUtility objDatabaseBackupUtility = new DatabaseBackupUtility();
                    //objDatabaseBackupUtility.BackupDatabaseInBlob();
                }
                //else if (appname == "backupdatabaseinblob")
                //{
                //    DatabaseBackupUtility objDatabaseBackupUtility = new DatabaseBackupUtility();
                //    objDatabaseBackupUtility.BackupDatabaseInBlob();
                //}
                ErrorLog.WriteLog("Scheduler Completed");
            }
            else
            {
                Console.WriteLine("Winform utility");
                frmWinformMain frm = new frmWinformMain();
                frm.ShowDialog();
                //Start();
            }
        }
        private static void ProcessThirdpartyAPI()
        {
            //if (AppConstants.IsIndiaMart_API_Enabled)
            {
                //AppConstantsWinform.ConnectionString = ConfigurationSettings.AppSettings["ConnectionString"];

                ThirdPartyAPI obj = new ThirdPartyAPI();
                obj.ProcessAPI(EnumAPIType.INDIA_MART);

                obj.ProcessAPI(EnumAPIType.TRADE_INDIA);

            }
        }
        private static void Start()
        {
            Scheduler obj = new Scheduler();
            obj.Start();
        }
        private static void TestMail()
        {
            SmtpClient mSmtpClient = new SmtpClient("mail.saitreat.com", 465);
            mSmtpClient.Credentials = new NetworkCredential("sales@saitreat.com", "Saitreat@2023#");
            MailMessage mail = new MailMessage("sales@saitreat.com", "rpluscrm@gmail.com");
            mail.Subject = "test";
            mail.Body = "test";
            mSmtpClient.Send(mail);


            Pop3Client pop3Client;
            pop3Client = new Pop3Client();
            pop3Client.Connect("mail.saitreat.com", 995, true);
            pop3Client.Authenticate("sales@saitreat.com", "Saitreat@2023#", AuthenticationMethod.UsernameAndPassword);

            int count = pop3Client.GetMessageCount();
        }
        //private static void ResizeImage(string rootFolder)
        //{
        //    //string folder = @"C:\Projects\Websites\rpluscrm.in\RPlusCRM_SAAS_V8\upload\shaktipowertronix";
        //    if (!Directory.Exists(AppConstants.ApplicationPath + "temp"))
        //    {
        //        Directory.CreateDirectory(AppConstants.ApplicationPath + "\\temp");
        //    }

        //    var folders = new Stack<string>();
        //    folders.Push(rootFolder);

        //    while (folders.Count > 0)
        //    {
        //        string currentFolder = folders.Pop();

        //        // Process files in the current folder
        //        try
        //        {
        //            foreach (string file in Directory.EnumerateFiles(currentFolder))
        //            {
        //                // Process each file here
        //                Console.WriteLine(file);
        //            }

        //            // Add subdirectories to stack
        //            foreach (string subFolder in Directory.EnumerateDirectories(currentFolder))
        //            {
        //                folders.Push(subFolder);
        //            }
        //        }
        //        catch (UnauthorizedAccessException e)
        //        {
        //            Console.WriteLine("Access denied: {e.Message}");
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Error: {ex.Message}");
        //        }
        //    }
        //    Console.WriteLine("Completed");
        //    Console.ReadLine();

        //    //ResizeImage(folder);
        //}
        //private static void ResizeImage1(string folderPath)
        //{
        //    DirectoryInfo dir = new DirectoryInfo(folderPath);
        //    foreach (DirectoryInfo sdir in dir.GetDirectories())
        //    {
        //        ResizeImage(sdir.FullName);
        //    }
        //    foreach (FileInfo file in dir.GetFiles())
        //    {
        //        if (file.Name.EndsWith(".jpg") || file.Name.EndsWith(".png") || file.Name.EndsWith(".bmp"))
        //        {
        //            ResizeAndSaveImage(file.FullName);
        //        }
        //    }
        //}
        //private static void ResizeAndSaveImage(string filePath)
        //{
        //    if (!File.Exists(filePath)) return;
        //    System.Drawing.Image image = System.Drawing.Image.FromFile(filePath);
        //    int maxWidthHeight = 800;
        //    if (image.Width <= maxWidthHeight && image.Height <= maxWidthHeight) return;
        //    image.Dispose();
        //    string temppath = AppConstants.ApplicationPath +  "temp\\" + Guid.NewGuid() + ".jpg";
        //    File.Copy(filePath, temppath);
        //    try
        //    {
        //        GlobalUtilities.ResizeImage(temppath, filePath, maxWidthHeight, maxWidthHeight);
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    GlobalUtilities.DeleteFile(temppath);
        //}
    }
}
