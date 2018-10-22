using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDSMDomain.Models;
using EDSMDomain.Services;

namespace EDSMSimpleSync.Dev
{
    /// <summary>
    /// Sync information.... with the void !!
    /// </summary>
    class VoidServiceJournal : IServiceJournal
    {
        public IList<string> GetDiscardedEvents()
        {
            return new List<string>();
        }

        public JournalResponse PostJournalEntry(IList<string> lines)
        {
            return PostJournalEntry("MULTI LINES");
        }

        public JournalResponse PostJournalEntry(string data)
        {
            if (data == "TEST")
            {
                var result = new JournalResponse {Message = "OK", MessageNumber = 302};
                return result;
            }

            Console.WriteLine("VOID : " + data);
            return new JournalResponse { Message = "Sent to the VOID...", MessageNumber = 100 };
        }
    }
}
