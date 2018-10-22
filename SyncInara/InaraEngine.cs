using EDLogWatcher.Parser;
using EDSMDomain.Api;
using EDSync.Core.Filter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncInara
{
    public class InaraEngine : IEntryManager
    {
        public InaraEngine()
        {

        }

        public IEntryFilter EntryFilter { get; set; }

        public void AddEntry(string data)
        {

        }

    }
}
