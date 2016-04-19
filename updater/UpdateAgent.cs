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
            string result = string.Empty;
            Process proc = null;
            try
            {
               
                proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        //FileName = @"C:\Users\Pavilion\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Windows PowerShell\Windows PowerShell",

                        FileName = "powershell.exe ",
                        Arguments = String.Format(" -noprofile -executionpolicy bypass -file {0} {1} {2} ", ps, args[0], args[1]),
                        UseShellExecute = false,
                        Verb = "RunAs",
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                proc.Start();
                while (!proc.StandardOutput.EndOfStream)
                {                    
                    result = proc.StandardOutput.ReadToEnd();
                    Console.WriteLine("output" + result);
                }
                return new ScriptResult { Completed = true, Result = result, ErrMessage = string.Empty, ErrorCode = proc.ExitCode };
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
                throw new NAPPatchException(proc.ExitCode.ToString(), ex);
            }            
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

        public FileInfo GetScriptFile(string targetDirPath,string scriptFile)
        {
            if (Directory.Exists(targetDirPath))
            {
                
                var di = new DirectoryInfo(targetDirPath);
                //return di.GetFiles(".ps1", SearchOption.TopDirectoryOnly).FirstOrDefault(x => x.Name.Equals(scriptFile));
                //return di.GetFiles(".bat", SearchOption.TopDirectoryOnly).FirstOrDefault(x => x.Name.Equals(scriptFile));
                return di.GetFiles().FirstOrDefault();
               // return di.GetFiles(".bat", SearchOption.TopDirectoryOnly).FirstOrDefault();
            }
            return null;
        }

        public List<FileInfo> GetAllScriptFiles(string targetPath)
        {

            if (Directory.Exists(targetPath))
            {
                var di = new DirectoryInfo(targetPath);
                return di.GetFiles(".ps1",SearchOption.TopDirectoryOnly).OrderBy(x => x.CreationTime).ToList();
            }
            return null;
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

        public FileInfo GetPackageByPatchName(string name)
        {
            if (Directory.Exists(downloadFolder))
            {
                var di = new DirectoryInfo(downloadFolder);
                return di.GetFiles().FirstOrDefault(x => x.Name.Equals(name,StringComparison.OrdinalIgnoreCase));
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
