using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Updater
{
    public class ScriptResult
    {
        public bool Completed { get; set; }
        public string Result { get; set; }
        public int ErrorCode { get; set; }
        public string ErrMessage { get; set; }
    }

    public class NAPPatchException : Exception
    {
        private string _errorCode;
        private Exception _ex;
        public NAPPatchException() { }
        public NAPPatchException(string exitcode,Exception ex):base(exitcode, ex)
        {
            _errorCode = exitcode;
            _ex = ex;   
        }
    }
}
