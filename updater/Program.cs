using NLog;
using PatchClient.ClinentDbModel.DataModel;
using PatchClient.MCPSvc;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using updater;
using updaterData;

namespace Updater
{
    class Program
    {
        public static string marketName = ConfigurationManager.AppSettings["MarketSN"];
        public static string target = ConfigurationManager.AppSettings["Target"];
        public static string executeDir = ConfigurationManager.AppSettings["ExecuteDir"];
        
        public static string downloadFolder = ConfigurationManager.AppSettings["DownloadFolder"];
        public static string marketId = ConfigurationManager.AppSettings["MarketId"];
        public static string checkerInterval = ConfigurationManager.AppSettings["DownloadCheckerInterval"];

        private static Timer downloaderInterval = new Timer();
        private static NAPDownloader downloader = new NAPDownloader();
        private static NAPMessaging messager = new NAPMessaging();
        private static updaterClientData updaterClient = new updaterClientData();

        private static Container _container = new Container();
        private static ILogger _log;

        static void Main(string[] args)
        {

            _container.Register<ILogger>(() => LogManager.GetCurrentClassLogger(), Lifestyle.Singleton);
            _log = _container.GetInstance<ILogger>();

           
            //add state to local db                       
            //updaterClient.UpdateClientPatchData(new NAPClientPatch { MarketId = 1, MarketSN = "nvdev", PatchScriptId =1, PatchState = (int)MCPStatus.Downloaded, DownloadDate = DateTime.Now, PatchName = "test" });


            var updater = new UpdateAgent();
            var clientPatchInfo = updaterClient.GetAllNewClientPatchData();
            //var patch = new NAPClientPatch();
            //patch.PatchName = "PatchTester.zip";

            //patch.PatchScriptId = 10;

            foreach (var patch in clientPatchInfo)
            {
                var patchAtDirInfo = updater.GetPackageByPatchName(patch.PatchName);
                
                //////////////////
                ///temp removed
                // if (patchAtDir == null) continue;
                //var patchName = updater.GetPackageByPatchName(patch.PatchName).Name;
                //////////////////

                var patchNameInfo = updater.GetPackageByPatchName(patch.PatchName);
                
                var fileNameAsDir = Path.GetFileNameWithoutExtension(patchNameInfo.Name);

                updater.UnzipPackage(downloadFolder + patchNameInfo.Name, target);
                var scriptFile = updater.GetScriptFile(target + "\\" + fileNameAsDir, ""); // script file name is fixed?

                var confirmResponse = messager.IsProceed(patch.PatchScriptId);
                //if (!confirmResponse.Processable) return;
                
                var stateResponseUpdating = messager.PostUpdaterState(new NAPUpdateModel { MarketId = marketName, McpId = patch.PatchScriptId, McpState = MCPStatus.Updating }).Result;
                var psfilePath = Path.Combine(target, fileNameAsDir, scriptFile.Name);
                var psFileDir = Path.Combine(target, fileNameAsDir);
                string[] arguments = new string[]{ executeDir, psFileDir + "\\" + "Patches" };
                try
                {
                    var report = updater.RunScript(psfilePath, arguments);
                    var stateResponseUpdated = messager.PostUpdaterState(new NAPUpdateModel { MarketId = marketName, McpId = patch.PatchScriptId, McpState = MCPStatus.Updated }).Result;
                    
                    //add state as applied to local db                       
                    updaterClient.UpdateClientPatchData(new NAPClientPatch { MarketId = Convert.ToInt16(marketId), PatchScriptId = patch.PatchScriptId, MarketSN = marketName, PatchState = "Applied", DownloadDate = DateTime.Now, PatchName = patch.PatchName });
                }
                catch (NAPPatchException ex)
                {
                    _log.Error("Failed to execute powershelll",ex);
                }
                catch(Exception ex)
                {
                    _log.Error(ex);
                }
            }

            //unzip file to target folder
            //var files = updater.GetPackagesByCreatedTime();            

            downloaderInterval.Interval = Convert.ToDouble(checkerInterval);
            downloaderInterval.Start();
            downloaderInterval.Elapsed += DownloaderInterval_Elapsed;

            
            Console.ReadLine();
        }

        private static void DownloaderInterval_Elapsed(object sender, ElapsedEventArgs e)
        {
            downloaderInterval.Stop();
           
            downloader.Checker(marketName);
            //downloaderInterval.Start();                       
        }
    }
}
