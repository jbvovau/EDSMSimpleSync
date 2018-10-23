using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDSync.Core.Storage
{
    public interface IStorage
    {
        object Get(string key);

        void Set(string key, object obj);

        void Save();
    }
}
