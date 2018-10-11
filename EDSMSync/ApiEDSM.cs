using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EDSMSync
{
    public class ApiEDSM
    {
        // see : https://docs.microsoft.com/en-us/azure/architecture/antipatterns/improper-instantiation/
        private static readonly HttpClient client = new HttpClient();

        private const string POST_API_JOURNAL = "https://www.edsm.net/api-journal-v1";

            
        public string CommanderName { get; set; }

        public string ApiKey { get; set; }

        public string FromSoftware { get; set; }

        public string FromSoftwareVersion { get; set; }

        public EDSMResponse PostJournalLine(string data)
        {
            var values = new Dictionary<string, string>
            {
                { "commanderName", this.CommanderName },
                { "apiKey", this.ApiKey },
                { "fromSoftware" , this.FromSoftwareVersion},
                { "fromSoftwareVersion", this.FromSoftwareVersion } ,
                {"message", data }
            };

            var taskResponseTask = this.Post(POST_API_JOURNAL, values);

            var value = taskResponseTask.Result;

            var result = JsonConvert.DeserializeObject<EDSMResponse>(value);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="values"></param>
        private async Task<string> Post(string url, Dictionary<string, string> values)
        {
            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }

        private async void Get(string url)
        {
            var responseString = await client.GetStringAsync(url);

        }

    }
}
