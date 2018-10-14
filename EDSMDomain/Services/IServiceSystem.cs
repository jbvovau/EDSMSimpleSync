using EDSMDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDSMDomain.Services
{
    public interface IServiceSystem
    {
        EDSMSystem GetStations(string systemName, long systemId);

        EDSMSystem GetFactions(string systemName, long systemId);

        EDSMSystem GetCelestialBodies(string systemName, long systemId);
    }
}
