using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDLogWatcher.Parser
{
    /// <summary>
    /// Treat each journal entry from Logs
    /// </summary>
    public interface IEntryManager
    {

        void AddEntry(string data);
    }
}
