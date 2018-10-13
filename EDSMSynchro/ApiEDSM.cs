﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EDSMSync
{
    public class ApiEDSM
    {

        #region const

        // see : https://docs.microsoft.com/en-us/azure/architecture/antipatterns/improper-instantiation/
        private static readonly HttpClient client = new HttpClient();

        // https://www.edsm.net/api-journal-v1/discard

        private const string POST_API_JOURNAL = "https://www.edsm.net/api-journal-v1";

        #endregion

        #region Properties

        public string CommanderName { get; set; }

        public string ApiKey { get; set; }

        public string FromSoftware { get; set; }

        public string FromSoftwareVersion { get; set; }

        #endregion

        #region public method

        public EDSMResponse PostJournalLine(string data)
        {
            var values = new Dictionary<string, string>
            {
                { "commanderName", this.CommanderName },
                { "apiKey", this.ApiKey },
                { "fromSoftware" , this.FromSoftware},
                { "fromSoftwareVersion", this.FromSoftwareVersion } ,
                {"message", data }
            };

            var taskResponseTask = this.Post(POST_API_JOURNAL, values);

            var value = taskResponseTask.Result;

            var result = JsonConvert.DeserializeObject<EDSMResponse>(value);
            return result;
        }

        public IList<string> GetDiscardedEvents()
        {
            var result = this.Get(POST_API_JOURNAL + "/discard");
            while (!result.IsCompleted)
            {
                System.Threading.Thread.Sleep(500);
            }
            var list = JsonConvert.DeserializeObject<List<string>>(result.Result);
            return list;
        }


        #endregion

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

        private async Task<string> Get(string url)
        {
            string response = null;
            response = await client.GetStringAsync(url);
            return response;
        }

    }
}