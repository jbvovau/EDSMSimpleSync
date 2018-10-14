using EDSMDomain.Api;
using EDUploader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncInara
{
    public class InaraUploader : AbstractUploader
    {
        public InaraUploader(): base(new ApiInara())
        {

        }


        public override string SendJournal(string data)
        {
            // test
            var json = JsonConvert.DeserializeObject(data);
            var list = new object[] { json };

            data = JsonConvert.SerializeObject(list);

            var result = this.Api.Post(ApiInara.API_JOURNAL, data);

            return null;
        }
    }
}
