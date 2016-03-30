using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Updater;

namespace updater
{
    public class NAPDownloader
    {
        private string patchServer = ConfigurationManager.AppSettings["PatchServer"];
        private string saveFolder =  ConfigurationManager.AppSettings["DownloadFolder"];
        private string webservice = ConfigurationManager.AppSettings["ServiceUrl"];
     
        private WebClient client = null;
        private NAPMessaging messager = null;
        private NAPDownloaderDto packageDto;

        public NAPDownloader()
        {
            messager = new NAPMessaging();
        }       

        public IEnumerable<NAPDownloaderDto> Checker(string marketSN)
        {
            try
            {
                using (var client = new HttpClient())
                {                   
                    client.BaseAddress = new Uri(webservice);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var url = "Napmarkets/GetPatchListByMarket/" + marketSN;
                    HttpResponseMessage response = client.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var str = response.Content.ReadAsStringAsync().Result;
                        var downloaderInfo = JsonConvert.DeserializeObject<List<NAPDownloaderDto>>(str);
                        return downloaderInfo;                        
                    }                   
                }               
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        public void DownloadPackage(NAPDownloaderDto package)
        {
            try
            {
                packageDto = package;
                // Create a new WebClient instance.
                client = new WebClient();
                client.Credentials = CredentialCache.DefaultCredentials;
                client.DownloadFileCompleted += Client_DownloadFileCompleted;
                client.DownloadProgressChanged += Client_DownloadProgressChanged;

                // Concatenate the domain with the Web resource filename.
                var strWebResource = patchServer + package.PatchScriptUrl;
                // Download the Web resource and save it into the current filesystem folder.
                client.DownloadFile(strWebResource, saveFolder + package.PatchScriptUrl);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
           await messager.PostUpdaterState(new NAPUpdateState { MarketId = packageDto.MarketId, PatchId = packageDto.PatchScriptId, PatchStateId = (int)eUpdateState.Downloading });
        }

        private async void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            await messager.PostUpdaterState(new NAPUpdateState { MarketId = packageDto.MarketId, PatchId = packageDto.PatchScriptId, PatchStateId = (int)eUpdateState.Downloaded });
        }
    }
}
