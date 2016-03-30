using NLog;
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

            var updater = new UpdateAgent();
            var files = updater.GetPackagesByCreatedTime();
            foreach(FileInfo f in files)
            {
                updater.UnzipPackage(downloadFolder + f, target);
                //messager.PostUpdaterState(new NAPUpdateState { MarketId = Convert.ToInt16(marketId), PatchId = 0, PatchStateId = (int)eUpdateState.Extracted }).ConfigureAwait(true);
            }
            downloaderInterval.Interval = Convert.ToDouble(checkerInterval);
            downloaderInterval.Start();
            downloaderInterval.Elapsed += DownloaderInterval_Elapsed;  
            
            updaterData
                       
            Console.ReadLine();
        }

        private static void DownloaderInterval_Elapsed(object sender, ElapsedEventArgs e)
        {
            var listdownloader = downloader.Checker(marketName);

            foreach (var package in listdownloader)
            {
                if (!string.IsNullOrEmpty(package.PatchScriptUrl))
                {
                    try
                    {
                        //send initial state to server
                        messager.PostUpdaterState(new NAPUpdateState { MarketId = Convert.ToInt16(marketId), PatchId = package.PatchScriptId, PatchStateId = (int)eUpdateState.Initial }).ConfigureAwait(true);
                        downloader.DownloadPackage(package);

                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex, "Failed to downloading");
                        Debug.WriteLine(ex);
                    }
                }

            }
        }
    }
}
