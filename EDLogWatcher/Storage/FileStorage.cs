using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDSync.Core.Storage
{
    public class FileStorage : IStorage
    {

        private Dictionary<string, object> _data;

        public FileStorage(string path)
        {
            this.StorageFile = path;
        }

        public string StorageFile { get; private set; }

        public object Get(string key)
        {
            this.Load();
            if (!_data.ContainsKey(key)) return null;
            return _data[key];
        }

        public void Set(string key, object value)
        {
            this.Load();
            this._data[key] = value;
            this.Save();
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(_data, Formatting.Indented);
            File.WriteAllText(StorageFile, json);
        }


        private void Load()
        {
            if (_data == null)
            {
                if (File.Exists(StorageFile))
                {
                    var settings = new JsonSerializerSettings();
                    string text = File.ReadAllText(StorageFile);
                    _data = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
                    if (_data == null)
                    {
                        _data = new Dictionary<string, object>();
                    }
                }
                else
                {
                    _data = new Dictionary<string, object>();
                }
            }
        }

    }
}
