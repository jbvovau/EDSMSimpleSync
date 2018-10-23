using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDLogWatcher.Parser
{
    /// <summary>
    /// A File Parser able to transform content into Journal Entries add provides it to an IEntryManager
    /// </summary>
    public interface IEntryParser : IFileParser
    {
        /// <summary>
        /// List of Entry Managers
        /// </summary>
        IList<IEntryManager> EntryManagers { get; }

        /// <summary>
        /// Add a new entry manager to the parser
        /// </summary>
        /// <param name="manager"></param>
        void Add(IEntryManager manager);
    }
}
