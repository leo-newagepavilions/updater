using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace updater
{
    public class NAPDownloaderDto
    {
        public int MarketId { get; set; }
        public string  Marketname { get; set; }
        public int PatchScriptId { get; set; }
        public string PatchScriptUrl { get; set; }
        public string PatchLevel { get; set; }
        public string PatchType { get; set; }
        public DateTime PatchReleaseDateTime { get; set; }

    }
}
