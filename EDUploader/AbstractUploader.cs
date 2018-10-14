using EDSMDomain.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDUploader
{
    public abstract class AbstractUploader : IUploader
    {
        public AbstractUploader(RestApi api)
        {
            this.Api = api;
        }

        public RestApi Api { get; private set; }

        public void AddEntry(string data)
        {
            this.SendJournal(data);
        }

        public string Send()
        {

            return null;
        }

        /// <summary>
        /// Send data 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract string SendJournal(string data);
    }
}
