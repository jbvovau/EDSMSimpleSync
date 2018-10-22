using EDSMDomain.Models;
using EDSMDomain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncInara
{
    public class ServiceInara : IServiceJournal
    {
        public IList<string> GetDiscardedEvents()
        {
            throw new NotImplementedException();
        }

        public JournalResponse PostJournalEntry(string data)
        {
            throw new NotImplementedException();
        }

        public JournalResponse PostJournalEntry(IList<string> lines)
        {
            throw new NotImplementedException();
        }
    }
}
