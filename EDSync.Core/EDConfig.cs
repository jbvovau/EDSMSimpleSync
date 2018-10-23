using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EDSync.Core.Storage;
using Newtonsoft.Json;

namespace EDSync.Core
{
    /// <summary>
    /// Configuration for this file
    /// </summary>
    public class EDConfig
    {
        private static EDConfig _instance;

        public static EDConfig Instance
        {
            get
            {
                if (_instance == null) _instance = new EDConfig();
                return _instance;
            }
        }

        private EDConfig()
        {

        }

        public IStorage Storage { get; set; }

        public string Get(string key)
        {
            var value = this.Storage.Get(key);
            if (value == null) return null;
            return value.ToString();
        }

        public void Set(string key, string value)
        {
            this.Storage.Set(key, value);
            this.Save();
        }

        private void Save()
        {
            this.Storage.Save();
        }
    }
}
