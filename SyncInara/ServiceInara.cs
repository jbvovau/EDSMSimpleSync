using EDSMDomain.Api;
using EDSMDomain.Models;
using EDSMDomain.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncInara
{
    public class ServiceInara : IServiceJournal
    {
        public ServiceInara(ApiInara api)
        {
            this.Api = api;
        }

        public ApiInara Api { get; private set; }

        public IList<string> GetDiscardedEvents()
        {
            return new List<string>();
        }

        public JournalResponse PostJournalEntry(string data)
        {
            // return Api.Post(Api.)
            throw new NotImplementedException();
        }

        public JournalResponse PostJournalEntry(IList<string> lines)
        {
            var list = new List<object>();

            foreach(var line in lines)
            {
                list.Add(JsonConvert.DeserializeObject(line));
            }

            var result = Api.PostData(JsonConvert.SerializeObject(list));

            throw new NotImplementedException();
        }
    }
}
