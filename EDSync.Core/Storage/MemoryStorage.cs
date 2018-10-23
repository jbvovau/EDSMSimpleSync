using EDSync.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDSMSimpleSync.Utils
{
    public class MemoryStorage : IStorage
    {
        private IDictionary<string, object> _data;

        public MemoryStorage()
        {
            this._data = new Dictionary<string, object>();
        }

        public object Get(string key)
        {
            if (_data.ContainsKey(key))
            {
                return _data[key];
            }
            return null;
        }

        public void Save()
        {
        }

        public void Set(string key, object obj)
        {
            _data[key] = obj;
        }
    }
}
