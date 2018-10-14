using EDSMDomain.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDUploader
{
    public interface IUploader
    {
        RestApi Api { get;  }

        void AddEntry(string data);

        string Send();
    }
}
