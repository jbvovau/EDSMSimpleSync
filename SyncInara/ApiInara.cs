using EDSMDomain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EDSMDomain.Api
{
    public class ApiInara : RestApi
    {
        public const string API_JOURNAL = "https://inara.cz/inapi/v1/";

        public ApiInara()
        {

        }


        public string PostData(string data)
        {
            var result = "";

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, API_JOURNAL))
            {
                /*
                requestMessage.Headers.Add("appName", "EDSimpleSync");
                requestMessage.Headers.Add("appVersion", "0.3.0");
                requestMessage.Headers.Add("isDeveloped", "true");
                requestMessage.Headers.Add("APIkey", this.ApiKey);
                requestMessage.Headers.Add("commanderName", this.CommanderName); */
                // requestMessage.Cons.Add("Content-Type", "application/json");

                var input = new InaraRequest();
                input.header = new Header
                {
                    appName = "EDSimpleSync",
                    appVersion = "0.3.0",
                    isDeveloped = true,
                    APIkey = this.ApiKey,
                    commanderName = this.CommanderName
                };

                var json = JsonConvert.SerializeObject(input, Formatting.Indented);

                requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = RestApi.client.SendAsync(requestMessage);
                result = response.Result.Content.ReadAsStringAsync().Result;
            }

            return result;
        }



        private class InaraRequest
        {
            public Header header;

        }

        private class Header
        {
            public string appName;
            public string appVersion;
            public bool isDeveloped;
            public string APIkey;
            public string commanderName;
        }
    }
}
