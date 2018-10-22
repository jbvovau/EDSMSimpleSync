using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EDSMDomain.Api
{
    public class ApiInara : RestApi
    {
        public const string API_JOURNAL = "https://inara.cz/inapi/v1/";

        public ApiInara()
        {

        }

        public override void Configure(HttpClient client)
        {
            base.Configure(client);

            client.DefaultRequestHeaders.Add("appName", "EDSMSimpleSync");
            client.DefaultRequestHeaders.Add("appVersion", "0.0.2");
            client.DefaultRequestHeaders.Add("isDeveloped", "true");
            client.DefaultRequestHeaders.Add("APIkey", this.ApiKey);
            client.DefaultRequestHeaders.Add("commanderName", this.CommanderName);
            client.DefaultRequestHeaders.Add("Content-Type", "application/json");
        }

    }
}
