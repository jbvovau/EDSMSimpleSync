using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDSync.Core.Filter;

namespace EDLogWatcher.Parser
{
    /// <summary>
    /// Treat each journal entry from Logs
    /// </summary>
    public interface IEntryManager
    {
        bool Enabled { get; }

        /// <summary>
        /// Something to filter entry
        /// </summary>
        IEntryFilter EntryFilter { get; }

        /// <summary>
        /// Add a new journal entry
        /// </summary>
        /// <param name="data"></param>
        bool AddEntry(string data);
    }
}
