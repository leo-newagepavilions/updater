using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Updater
{
    public interface IChecker
    {
        string[] Checkup();
        bool IsNewUpdateAgent(string strVer);
        ScriptResult RunScript(string ps, string[] args);
        bool RunSqlScript();
        bool RunFileUpdate(string filePath);
        Nullable<DateTime> CheckupZipCreatedDateTime(string zipPath);
        List<FileInfo> GetPackagesByCreatedTime();
        void UnzipPackage(string zipPath, string extractPath);
        void UpdateDownloadInterval(int invterval);
       
    }
}
