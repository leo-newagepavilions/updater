using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Updater;
using PatchClient.MCPSvc;
using System.Diagnostics;

namespace updater
{
    public class NAPMessaging
    {
        private PatchClient.MCPSvc.MCPSvcSoapClient _mcpSvc;
        private static string marketName = ConfigurationManager.AppSettings["MarketSN"];

        public NAPMessaging()
        {
            _mcpSvc = new MCPSvcSoapClient();           
        }

        public  MCPStatusResponse PostUpdaterState(NAPUpdateModel updaterModel)
        {
            try
            {
                return  _mcpSvc.UpdateMCPState(new PatchClient.MCPSvc.AuthHeader(), updaterModel.MarketId, updaterModel.McpId, updaterModel.McpState);

            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }            
            //var connection = new NAPConnection();

            //var webservice = ConfigurationManager.AppSettings["ServiceUrl"];
            //var url = webservice + "NAPPatchTransitions/PostNAPPatchTransition";
            //var client = connection.EstablishConnection(url);            

            //HttpResponseMessage response = await client.PostAsJsonAsync(url, updaterModel);
            //if (response.IsSuccessStatusCode)
            //    return true;
            //else
            //    return false;
            
        }

        public ConfirmMCPResponse IsProceed( int patchId)
        {
            try
            {
                return _mcpSvc.ConfirmMCPforProcessing(new AuthHeader(), marketName, patchId);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public bool GetServerCommand()
        {

            return true;
        }

        public List<NAPUpdateModel> GetAllUpdaterState()
        {
            var connection = new NAPConnection();
           
            var webservice = ConfigurationManager.AppSettings["ServiceUrl"];
            var url = webservice + "NAPUpdaterStates/GetNAPUpdaterStates";
            var client = connection.EstablishConnection(url);

            HttpResponseMessage response = client.GetAsync(url).Result;
                
            if (response.IsSuccessStatusCode)
            {
                var str = response.Content.ReadAsStringAsync().Result;
                var updaterInfo = JsonConvert.DeserializeObject<List<NAPUpdateModel>>(str);
                return updaterInfo;

            }
            return null;
        }
    }
}
