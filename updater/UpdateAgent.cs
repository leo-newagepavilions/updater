using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using SimpleInjector;
using NLog;
using updater;
using System.Configuration;

namespace Updater
{
    public enum eUpdateState : short
    {
        Initial = 0,
        Downloading = Initial << 1,
        Downloaded = Initial << 2,
        Updating = Initial << 3,
        Updated = Initial << 4,
        Rebooted = Initial << 5,
        Pending = Initial << 6,
        Completed = Initial << 7,
        Aborted = Initial << 8,
        Cancelling = Initial << 9,
        Canceled = Initial << 10,
        Extracted = Initial << 11
    }

    public class UpdateAgent : IChecker
    {
        private Container _container = new Container();
        private ILogger _log;
        public readonly string downloadFolder = ConfigurationManager.AppSettings["DownloadFolder"];

        public UpdateAgent()
        {
            _container.Register<ILogger>(() => LogManager.GetCurrentClassLogger(),Lifestyle.Singleton);
            _log = _container.GetInstance<ILogger>();
        }

        public string ExtractPath { get; set; }


        public string[] Checkup()
        {
            if (Directory.Exists(downloadFolder))
            {
                return Directory.GetFiles(downloadFolder);
            }
            return null;
        }

        public Nullable<DateTime> CheckupZipCreatedDateTime(string zipPath)
        {
            if (File.Exists(zipPath))
            {
                return File.GetCreationTimeUtc(zipPath);
            }
            return null;
        }   
        
       
        public bool IsNewUpdateAgent(string strVer)
        {
            throw new NotImplementedException();
        }

        public bool RunFileUpdate(string filePath)
        {
            throw new NotImplementedException();
        }

        public ScriptResult RunScript(string ps, string[] args)
        {
            throw new NotImplementedException();
        }

        public ScriptResult RunScript(string ps, string args)
        {
            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ps,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                Console.WriteLine("output" + line);                
            }
            return null;
        }

        public bool RunSqlScript()
        {
            throw new NotImplementedException();
        }

        public void UnzipPackage(string zipPath, string extractPath)
        {
            try
            {                
                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    if (Directory.Exists(extractPath))
                        Directory.Delete(extractPath, true);

                    archive.ExtractToDirectory(extractPath);
                }
            }
            catch(Exception ex)
            {
                _log.Error(ex, "Failed to unzip package.");
                throw;
            }
        }

        public List<FileInfo> GetPackagesByCreatedTime()
        {
            if (Directory.Exists(downloadFolder))
            {
                var di = new DirectoryInfo(downloadFolder);
                return di.GetFiles().OrderBy(x => x.CreationTime).ToList();                             
            }
            return null;
        }

        public void UpdateDownloadInterval(int interval)
        {
            if (interval == 0) return;
            ConfigurationManager.AppSettings["DownloadFolder"] = interval.ToString();
        }
    }
}
