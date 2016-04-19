using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PatchClient.MCPSvc;

namespace PatchClient
{

    public class DownloaderArgs : EventArgs
    {
        public int PatchId;
        public string MarketId;
        public MCPStatus State;
        public string PatchName;

        public DownloaderArgs(int patchId,string marketId)
        {
            PatchId = patchId;
            MarketId = marketId;
        }

        public DownloaderArgs(int patchId, string marketId,string patchName, MCPStatus state)
        {
            PatchId = patchId;
            MarketId = marketId;
            PatchName = patchName;
            State = state;
        }
    }



    public class NAPDownloaderDelegate
    {
        public event EventHandler<DownloaderArgs> Downloading;
        public event EventHandler<DownloaderArgs> Downloaded;

        public int PatchId { get; set; }
        public string MarketId { get; set; }
        public MCPStatus States { get; set; }
        public String PatchName { get; set; }

        private bool reentry = true;
        public WebClient Client;

        public void DownloadPackages()
        {

            Client.Credentials = CredentialCache.DefaultCredentials;
            //client.OpenReadCompleted += new EventHandler((s, e) => { });
            Client.DownloadFileCompleted += Client_DownloadFileCompleted;
            Client.DownloadProgressChanged += Client_DownloadProgressChanged;

        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (Downloading != null && reentry)
            {
                reentry = false;
                Downloading(this, new DownloaderArgs(PatchId, MarketId,PatchName, MCPStatus.Downloading));
            }
        }

        private void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (Downloaded != null)
            {
                Downloaded(this, new DownloaderArgs(PatchId, MarketId, PatchName, MCPStatus.Downloaded));
            }
        }
    }
}
