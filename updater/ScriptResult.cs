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
        public int ErrorCode { get; set; }
        public string ErrMessage { get; set; }
    }
}
