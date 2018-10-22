using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EDSMDomain.Api
{
    public class RestApi
    {

        // see : https://docs.microsoft.com/en-us/azure/architecture/antipatterns/improper-instantiation/
        public static readonly HttpClient client = new HttpClient();

        #region Properties

        public string CommanderName { get; set; }

        public string ApiKey { get; set; }

        public string FromSoftware { get; set; }

        public string FromSoftwareVersion { get; set; }

        #endregion

        #region private


        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="values"></param>
        public async Task<string> Post(string url, Dictionary<string, string> values)
        {

            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }

        public async Task<string> Post(string url, string data)
        {

            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }

        public async Task<string> Get(string url)
        {

            string response = null;
            response = await client.GetStringAsync(url);
            return response;
        }

        #endregion
    }
}
