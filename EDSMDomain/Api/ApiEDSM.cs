using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EDSMDomain.Models;
using Newtonsoft.Json;

namespace EDSMDomain.Api
{
    public class ApiEDSM : RestApi
    {

        #region const


        // https://www.edsm.net/api-journal-v1/discard

        private const string API_JOURNAL_V1 = "https://www.edsm.net/api-journal-v1";

        private const string API_SYSTEM_V1 = "https://www.edsm.net/api-system-v1";

        private const string SYSTEM_STATON = "/stations";
        private const string SYSTEM_FACTIONS = "/factions";
        private const string SYSTEM_BODIES = "/bodies";

        private const string PARAM_SYSTEM_NAME = "systemName";
        private const string PARAM_SYSTEM_ID = "systemId";

        #endregion


    
        #region API_JOURNAL_V1 method https://www.edsm.net/fr/api-journal-v1

        public JournalResponse PostJournalLine(string data)
        {
            var values = new Dictionary<string, string>
            {
                { "commanderName", this.CommanderName },
                { "apiKey", this.ApiKey },
                { "fromSoftware" , this.FromSoftware},
                { "fromSoftwareVersion", this.FromSoftwareVersion } ,
                {"message", data }
            };

            var taskResponseTask = this.Post(API_JOURNAL_V1, values);

            var value = taskResponseTask.Result;

            var result = JsonConvert.DeserializeObject<JournalResponse>(value);
            return result;
        }

        public IList<string> GetDiscardedEvents()
        {
            var result = this.Get(API_JOURNAL_V1 + "/discard");
            while (!result.IsCompleted)
            {
                System.Threading.Thread.Sleep(500);
            }
            var list = JsonConvert.DeserializeObject<List<string>>(result.Result);
            return list;
        }

        #endregion

        #region API_SYSTEM_V1

        public string GetSystemStations(string systemName)
        {
            return GetSystemStations(systemName, 0);
        }

        public string GetSystemStations(string systemName, long systemId)
        {
            var json = this.executeSystem(SYSTEM_STATON, systemName, systemId);
            return json;
        }

        public string GetSystemFactions(string systemName)
        {
            return GetSystemFactions(systemName, 0);
        }

        public string GetSystemFactions(string systemName, long systemId)
        {
            var json = this.executeSystem(SYSTEM_FACTIONS, systemName, systemId);
            return json;
        }

        public string GetCelestialBodies(string systemName, long systemId)
        {
            var json = this.executeSystem(SYSTEM_BODIES, systemName, systemId);
            return json;
        }

        private string executeSystem(string action, string systemName, long systemId)
        {
            StringBuilder sb = new StringBuilder(API_SYSTEM_V1);
            sb.Append(action);
            sb.Append('?');
            sb.Append(PARAM_SYSTEM_NAME);
            sb.Append('=');
            sb.Append(systemName);

            if (systemId > 0)
            {
                sb.Append('&');
                sb.Append(PARAM_SYSTEM_ID);
                sb.Append('=');
                sb.Append(systemId);
            }

            return this.Get(sb.ToString()).Result;
        }


        #endregion


    }
}
