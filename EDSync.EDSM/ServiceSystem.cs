using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDSMDomain.Api;
using EDSMDomain.Models;
using EDSMDomain.Services;
using Newtonsoft.Json;

namespace EDSync.EDSM
{
    public class ServiceSystem : IServiceSystem
    {
        private ApiEDSM _api;

        public ServiceSystem()
        {
            this._api = new ApiEDSM();
        }

        public EDSMSystem GetFactions(string systemName, long systemId)
        {
            var json =_api.GetSystemFactions(systemName, systemId);
            var result = JsonConvert.DeserializeObject<EDSMSystem>(json);
            return result;
        }

        public EDSMSystem GetStations(string systemName, long systemId)
        {
            var json = _api.GetSystemStations(systemName, systemId);
            var result = JsonConvert.DeserializeObject<EDSMSystem>(json);
            return result;
        }

        public EDSMSystem GetCelestialBodies(string systemName, long systemId)
        {
            var json = _api.GetCelestialBodies(systemName, systemId);
            var result = JsonConvert.DeserializeObject<EDSMSystem>(json);
            return result;
        }

    }
}
