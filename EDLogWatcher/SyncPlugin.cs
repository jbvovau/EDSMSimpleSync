using EDLogWatcher.Parser;
using EDSMDomain.Services;
using EDSync.Core.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDSync.Core.Parser
{
    public class SyncPlugin : IEntryManager
    {

        private IList<string> _lines;
        private bool _sending;

        public SyncPlugin(IEntryFilter filter)
        {
            this.EntryFilter = filter;
            this._lines = new List<string>();
            this._sending = false;
            this.LastActivity = DateTime.Now;
        }

        public IEntryFilter EntryFilter { get; private set; }

        public IServiceJournal ServiceJournal { get; set; }

        /// <summary>
        /// Get last time entry was added
        /// </summary>
        public DateTime LastActivity { get; private set; }

        public virtual void AddEntry(string data)
        {
            this.LastActivity = DateTime.Now;

            if (EntryFilter.Accepted(data))
            {
                // add to batch
                lock (_lines)
                {
                    _lines.Add(data);
                }
                EntryFilter.Discard(data);
                this._sending = true;
            }
        }

        /// <summary>
        /// Next line to send
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public IList<string> Next(int count)
        {
            var result = new List<string>();

            lock (_lines)
            {
                int i = 0;
                while (i < count && i < _lines.Count)
                {
                    result.Add(_lines[i++]);
                }
            }

            return result;
        }

        /// <summary>
        /// Remove sent line
        /// </summary>
        /// <param name="lines"></param>
        public void Remove(IList<string> lines)
        {
            lock (_lines)
            {
                foreach(var data in lines)
                {
                    this._lines.Remove(data);
                }

                if (_lines.Count == 0 && _sending)
                {
                    this.EntryFilter.Confirm();
                    _sending = false;
                }
            }
        }


    }
}
