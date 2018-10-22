using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace EDLogWatcher
{
    /// <summary>
    /// Configuration for this file
    /// </summary>
    public class EDConfig
    {
        private const string CONFIG_FILE = "edconfig.json";
        private Dictionary<string, string> _data;

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

        public string Get(string key)
        {
            this.Load();
            if (!_data.ContainsKey(key)) return null;
            return _data[key];
        }

        public void Set(string key, string value)
        {
            this.Load();
            this._data[key] = value;
            this.Save();
        }

        private void Save()
        {
            var json = JsonConvert.SerializeObject(_data);
            File.WriteAllText(CONFIG_FILE, json);
        }


        private void Load()
        {
            if (_data == null)
            {
                if (File.Exists(CONFIG_FILE))
                {
                    string text = File.ReadAllText(CONFIG_FILE);
                    _data = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
                    if (_data == null)
                    {
                        _data = new Dictionary<string, string>();
                    }
                }
                else
                {
                    _data = new Dictionary<string, string>();
                }
            }
        }
    }
}
