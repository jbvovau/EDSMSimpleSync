using EDSMDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDSMDomain.Services
{
    public interface IServiceJournal
    {
        bool IsEventDiscarded(string name);

        JournalResponse TestConnection();

        JournalResponse PostJournalEntry(string data);

        JournalResponse PostJournalEntry(IList<string> lines);

        /// <summary>
        /// Commmit result and returns messages
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> Commit();
    }
}
