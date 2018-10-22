using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDSync.Core;
using EDSync.Core.Storage;

namespace EDSMSimpleSync.Utils
{
    class CustomConfig
    {
        public CustomConfig(IStorage storage, string name)
        {
            this.Storage = storage;
            this.ConfigName = name;
        }

        public IStorage Storage { get; set; }

        public string ConfigName { get; private set; }

        public string CommanderName
        {
            get { return Get("cmdr"); }
            set
            {
                Set("cmdr", value);
            }
        }


        public string ApiKey
        {
            get { return Get("api_key"); }
            set
            {
                Set("api_key", value);
            }
        }


        public DateTime LastDate
        {
            get
            {
                DateTime lastDate;
                DateTime.TryParse(Get("last_date"), out lastDate);
                return lastDate;
            }
            set
            {
                Set("last_date", value.ToString());
            }
        }

        private string Get(string key)
        {
            return EDConfig.Instance.Get("__" + this.ConfigName + "___" + key);
        }

        private void Set(string key, string value)
        {
            EDConfig.Instance.Set("__" + this.ConfigName + "___" + key, value);
        }
    }
}
