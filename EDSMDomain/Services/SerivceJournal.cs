using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDSMDomain.Api;
using EDSMDomain.Models;

namespace EDSMDomain.Services
{
    public class SerivceJournal : IServiceJournal
    {

        private ApiEDSM _api;
        private string _apiKey;

        public SerivceJournal(string name, string apiKey)
        {
            _api = new ApiEDSM();
            _api.CommanderName = name;
            _api.ApiKey = apiKey;
            _api.FromSoftware = "EDSMSimpleSync";
            _api.FromSoftwareVersion = "0.0.1"; //TODO lol
        }

        public IList<string> GetDiscardedEvents()
        {
            return _api.GetDiscardedEvents();
        }

        public EDSMResponse PostJournalEntry(string data)
        {
            return _api.PostJournalLine(data);
        }
    }
}
