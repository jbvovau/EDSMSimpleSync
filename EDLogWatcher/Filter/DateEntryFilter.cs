using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using EDSync.Core;
using EDSync.Core.Filter;
using EDSync.Core.Storage;
using Newtonsoft.Json;

namespace EDLogWatcher.Filter
{
    /// <summary>
    /// Define a filter for Journal Event by date
    /// </summary>
    public class DateEntryFilter : IEntryFilter
    {
        private LinkedList<string> _discarded;

        public DateEntryFilter(IStorage storage, string name)
        {
            this.Storage = storage;
            this._discarded = new LinkedList<string>();

            this.Name = name;
            this.Load();
        }

        public IStorage Storage { get; set; }

        /// <summary>
        /// Filter name
        /// </summary>
        public string Name { get; private set; }

        public DateTime LastDate { get; set; }

        public bool Accepted(string entry)
        {
            if (this._discarded.Contains(entry)) return false;

            DateTime date = this.GetTime(entry);

            if (date < this.LastDate) return false;

            return true;
        }

        public void Discard(string entry)
        {
            while (_discarded.Count > 30)
            {
                _discarded.RemoveFirst();
            }

            _discarded.AddLast(entry);

            DateTime date = this.GetTime(entry);

            if (date > this.LastDate)
            {
                this.LastDate = date;
            }
        }

        public void Confirm()
        {
            this.Save();
        }

        private void Load()
        {
            object date = this.Storage.Get("__date_entry_filter_" + this.Name);
            if (date != null)
            {
                DateTime result;
                if (DateTime.TryParse(date.ToString(), out result))
                {
                    this.LastDate = result;
                }
            }

        }

        private void Save()
        {
            this.Storage.Set("__date_entry_filter_" + this.Name, this.LastDate.ToString());
        }

        private DateTime GetTime(string data)
        {
            GameEvent json = JsonConvert.DeserializeObject<GameEvent>(data);
            return json.timestamp;
        }

        private class GameEvent
        {
            public DateTime timestamp;
        }

    }
}
