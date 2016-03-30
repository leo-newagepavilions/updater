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

namespace updater
{
    public class NAPConnection: IDisposable
    {
        public string ServerUrl { get; set; }
        public string ServerPort { get; set; }
        public DateTimeOffset IntervalCheck { get; set; }
        private HttpClient client = null;        


        public HttpClient EstablishConnection(string actionUrl)
        {
            try
            {
               
                client = HttpClientFactory.Create();                
                client.BaseAddress = new Uri(actionUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return client;                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }
         public void Dispose()
        {
            client.Dispose();          

        }
    }
}
