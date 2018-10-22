using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using EDSync.Core;
using EDSync.Core.Filter;

namespace EDLogWatcher.Filter
{
    public class DateEntryFilter : IEntryFilter
    {
        public DateEntryFilter(string name)
        {
            this.Name = name;
            this.Load();
        }

        public string Name { get; private set; }

        public DateTime LastDate { get; set; }

        public bool Accepted(string entry)
        {
            return true;
        }

        public void Discard(string entry)
        {

            // this.Save();
        }

        public void Confirm()
        {
            this.Save();
        }

        private void Load()
        {
            var date = EDConfig.Instance.Get("____date_entry_filter_" + this.Name);
            if (date != null)
            {
                DateTime result;
                if (DateTime.TryParse(date, out result))
                {
                    this.LastDate = result;
                }
            }

        }

        private void Save()
        {
            EDConfig.Instance.Set("____date_entry_filter_" + this.Name, this.LastDate.ToString());
        }
    }
}
