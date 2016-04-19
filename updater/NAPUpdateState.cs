using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Updater;

namespace updater
{
   
    public class NAPUpdateModel
    {        
        public PatchClient.MCPSvc.MCPStatus McpState { get; set; }
        public int McpId { get; set; }
        public string MarketId { get; set; }       
        public string PatchName { get; set; }

    }
}
