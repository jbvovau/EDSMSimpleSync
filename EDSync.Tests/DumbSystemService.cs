using EDSMDomain.Models;
using EDSMDomain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDSMDomain.Tests
{
    class DumbSystemService : IServiceSystem
    {
        public EDSMSystem GetCelestialBodies(string systemName, long systemId)
        {
            var sys = new EDSMSystem { Id = RandomID(), Name = "Dummy" };

            sys.CelestialBodies = new List<CelestialBody>();
            sys.CelestialBodies.Add(new CelestialBody { Id = RandomID(), Name = "Sun" });
            sys.CelestialBodies.Add(new CelestialBody { Id = RandomID(), Name = "Earth" });

            return sys;
        }

        public EDSMSystem GetFactions(string systemName, long systemId)
        {
            var sys = new EDSMSystem { Id = RandomID(), Name = "Dummy" };

            sys.Factions = new List<Faction>();
            sys.Factions.Add(new Faction { Id = RandomID(), Name = "Social Dummy Future" });
            sys.Factions.Add(new Faction { Id = RandomID(), Name = "Blob" });
            sys.ControllingFaction = sys.Factions[0];

            return sys;
        }

        public EDSMSystem GetStations(string systemName, long systemId)
        {
            var sys = new EDSMSystem { Id = RandomID() , Name = "Dummy" };

            sys.Stations = new List<Station>();

            sys.Stations.Add(new Station { Id = RandomID(), Name = "Vovau Orbital" });

            return sys;
        }

        private long RandomID()
        {
            var r = new Random();
            return r.Next();
        }
    }
}
