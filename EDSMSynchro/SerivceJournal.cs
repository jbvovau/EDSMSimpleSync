using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDSMDomain.Api;
using EDSMDomain.Models;
using EDSMDomain.Services;

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
            throw new NotImplementedException();
        }

        public JournalResponse PostJournalEntry(string data)
        {
            return Api.PostJournalLine(data);
        }
    }
}
