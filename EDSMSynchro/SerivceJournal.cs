using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDSMDomain.Api;
using EDSMDomain.Models;
using EDSMDomain.Services;
using Newtonsoft.Json;

namespace EDSync.EDSM
{
    public class SerivceJournal : IServiceJournal
    {
        public SerivceJournal(ApiEDSM api)
        {
            this.Api = api;
        }

        public ApiEDSM Api { get; private set; }

        public IList<string> GetDiscardedEvents()
        {
            return Api.GetDiscardedEvents();
        }

        public JournalResponse PostJournalEntry(IList<string> lines)
        {
            if (lines == null || lines.Count == 0) return new JournalResponse { Message = "Nothing to send", MessageNumber = 100 };

            var list = new List<object>();

            foreach(var line in lines)
            {
                list.Add(JsonConvert.DeserializeObject(line));
            }

            return PostJournalEntry(JsonConvert.SerializeObject(list));
        }

        public JournalResponse PostJournalEntry(string data)
        {
            return Api.PostJournalLine(data);
        }
    }
}
