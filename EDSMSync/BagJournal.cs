using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace EDSMSync
{
    public class BagJournal
    {
        private SortedDictionary<DateTime, string> _sorted;
        private DateTime _lasttime;

        private IDictionary<string, List<DateTime>> _batchToKey;

        public BagJournal()
        {
            this._sorted = new SortedDictionary<DateTime, string>();
            this._lasttime = new DateTime(1900,1,1);

            this._batchToKey = new Dictionary<string, List<DateTime>>();
        }

        /// <summary>
        /// Add entry to journal
        /// </summary>
        /// <param name="data"></param>
        public void AddEntry(DateTime timestamp, string data)
        {
            if (timestamp >= _lasttime)
            {
                lock (_sorted)
                {
                    if (_sorted.ContainsKey(timestamp))
                    {
                        _sorted.Remove(timestamp);
                    }
                    this._sorted.Add(timestamp, data);
                }
            }
        }

        /// <summary>
        /// Return a batch of entries
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public string NextBatch(int count)
        {
            StringBuilder sb = new StringBuilder();

            lock (_sorted)
            {
                // future key to remove
                var toremove = new List<DateTime>();

                int i = 0;
                while (i < count && i < _sorted.Count)
                {
                    var key = _sorted.Keys.First();
                    var value = _sorted[key];

                    sb.Append(value);
                    sb.Append("\n");
                    toremove.Add(key);
                    i++;
                }

                this._batchToKey.Add(sb.ToString() , toremove);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Set batch commited
        /// </summary>
        /// <param name="data"></param>
        public void CommitBatch(string data)
        {
            var list = this._batchToKey[data];

            if (list != null)
            {
                foreach (var date in list )
                {
                    _sorted.Remove(date);
                }
            }
        }

        /// <summary>
        /// Forget every event before this date
        /// </summary>
        /// <param name="timestamp"></param>
        public void ForgetBefore(DateTime timestamp)
        {
            this._lasttime = timestamp;

            lock (_sorted)
            {
                foreach (var key in _sorted.Keys)
                {
                    if (key < timestamp)
                    {
                        _sorted.Remove(key);
                    }
                }
            }
        }
        
    }
}
