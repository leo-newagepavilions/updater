using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Updater;

namespace updater
{
    public class NAPMessaging
    {
        public async Task<bool> PostUpdaterState(NAPUpdateState updaterState)
        {
            var connection = new NAPConnection();

            var webservice = ConfigurationManager.AppSettings["ServiceUrl"];
            var url = webservice + "NAPPatchTransitions/PostNAPPatchTransition";
            var client = connection.EstablishConnection(url);            

            HttpResponseMessage response = await client.PostAsJsonAsync(url, updaterState);
            if (response.IsSuccessStatusCode)
                return true;
            else
                return false;
            
        }
        public bool GetServerCommand()
        {

            return true;
        }

        public List<NAPUpdateState> GetAllUpdaterState()
        {
            var connection = new NAPConnection();
           
            var webservice = ConfigurationManager.AppSettings["ServiceUrl"];
            var url = webservice + "NAPUpdaterStates/GetNAPUpdaterStates";
            var client = connection.EstablishConnection(url);

            HttpResponseMessage response = client.GetAsync(url).Result;
                
            if (response.IsSuccessStatusCode)
            {
                var str = response.Content.ReadAsStringAsync().Result;
                var updaterInfo = JsonConvert.DeserializeObject<List<NAPUpdateState>>(str);
                return updaterInfo;

            }
            return null;
        }
    }
}
