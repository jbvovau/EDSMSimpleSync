using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace EDSMSync
{
    /// <summary>
    /// All events sorted by date
    /// </summary>
    public class SortedJournal
    {
        private SortedDictionary<DateTime, string> _sorted;
        private DateTime _lasttime;

        private IDictionary<string, DateTime> _dataToKey;

        public SortedJournal()
        {
            this._sorted = new SortedDictionary<DateTime, string>();
            this._lasttime = new DateTime(1900,1,1);

            this._dataToKey = new Dictionary<string, DateTime>();
        }

        public DateTime CurrentTime { get; private set; }

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
        /// Next entry to send
        /// </summary>
        /// <returns></returns>
        public string NextEntry()
        {
            if (_sorted.Count == 0) return null;

            var date = _sorted.Keys.First();
            var text = _sorted[date];

            _dataToKey.Add(text, date);
            return text;
        }

        public void ConfirmEntry(string data)
        {
            var key = _dataToKey[data];
            this.CurrentTime = key;

            this._sorted.Remove(key);
            this._dataToKey.Remove(data);
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
