using Newtonsoft.Json;
using PatchClient;
using PatchClient.ClinentDbModel.DataModel;
using PatchClient.MCPSvc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Updater;
using updaterData;

namespace updater
{
    public class NAPDownloader
    {
        private string patchServer = ConfigurationManager.AppSettings["PatchServer"];
        private string saveFolder =  ConfigurationManager.AppSettings["DownloadFolder"];
        private string webservice = ConfigurationManager.AppSettings["ServiceUrl"];

        public static string marketId = ConfigurationManager.AppSettings["MarketId"];
        //private WebClient client = new WebClient();
        private NAPMessaging messager = null;
        private NAPDownloaderDto packageDto;
        private updaterClientData updaterClient;
        private NAPDownloaderDelegate _delegater;

        private PatchClient.MCPSvc.MCPSvcSoapClient _mcpSvc;

        public NAPDownloader()
        {
            messager = new NAPMessaging();
            updaterClient = new updaterClientData();
            _mcpSvc = new PatchClient.MCPSvc.MCPSvcSoapClient();

            
        }       

        public void Checker(string marketSN)
        {           
            try
            {
                MCPManifestResponse response = _mcpSvc.GetMCPManifest(new PatchClient.MCPSvc.AuthHeader(), marketSN);
                if (response.Result == null) throw new Exception("Failed to check patches.");
                
                foreach (var mcp in response.MCPManifest)
                {                    
                        Debug.WriteLine(mcp.MCPName + "id:" + mcp.BlobKey);
                        DownloadPackage(mcp.MCPName, mcp.BlobKey, mcp.MCPID,marketSN);               
                }
                
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }          
        }

        public void DownloadPackage(string patchName,string blobKey,int patchId,string marketSN)
        {
           
                try
                {
                    _delegater = new NAPDownloaderDelegate();
                    _delegater.MarketId = marketSN;//"NADEV";
                    _delegater.PatchId = patchId;
                    _delegater.PatchName = patchName;

                    Debug.WriteLine("_delegater: " + patchName + "task:  " +Task.CurrentId?.ToString());

                    WebClient client = new WebClient();
                    _delegater.Client = client;
                    _delegater.DownloadPackages();
                    _delegater.Downloading += _delegater_Downloading;
                    _delegater.Downloaded += _delegater_Downloaded;
                    client.Credentials = CredentialCache.DefaultCredentials;
                    ////client.OpenReadCompleted += new EventHandler((s, e) => { });
                    //client.DownloadFileCompleted += 
                    //client.DownloadProgressChanged += Client_DownloadProgressChanged;

                    Uri uri = new Uri("https://pavilion.smartmarketadmindev.com/api/upload/getmcp/" + blobKey);
                    // Download the Web resource and save it into the current filesystem folder.
                    client.DownloadFileAsync(uri, saveFolder + patchName);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }                                   
        }

        private void _delegater_Downloaded(object sender, DownloaderArgs e)
        {
            var response = messager.PostUpdaterState(new NAPUpdateModel { MarketId = e.MarketId, McpId = e.PatchId, McpState = MCPStatus.Downloaded }).Result;
            Debug.WriteLine("downloaded: " + response.Success.ToString() + " patchName: " + e.PatchName);
            var clientData = new NAPClientPatch();
            //add state to local db                       
            updaterClient.AddClientPatchData(new NAPClientPatch { MarketId = Convert.ToInt16(marketId), MarketSN = e.MarketId, PatchScriptId = e.PatchId, PatchState = "Downloaded", DownloadDate = DateTime.Now,PatchName=e.PatchName });
        }

        private  void _delegater_Downloading(object sender, DownloaderArgs e)
        {
            var response = messager.PostUpdaterState(new NAPUpdateModel { MarketId = e.MarketId, McpId = e.PatchId, McpState = MCPStatus.Downloading}).Result;
            Debug.WriteLine("downloading: " +response.Success.ToString() + " patchName: " + e.PatchName);
            //await response;
        }        
    }
}
