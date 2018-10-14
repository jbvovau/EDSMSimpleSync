using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDSMDomain.Models;

namespace EDSMDomain.Services
{
    public class CacheServiceSystem : IServiceSystem
    {
        private IServiceSystem _service;
        private IStorage _storage;

        public CacheServiceSystem(IServiceSystem service, IStorage storage)
        {
            this._service = service;
            this._storage = storage;
        }

        public EDSMSystem GetSystem(string systemName, long systemId)
        {
            var key = "SYSTEM_" + systemName;

            var system = _storage.Get(key) as EDSMSystem;

            if (system == null || system.Stations == null)
            {
                system = GetStations(systemName, systemId);
            }

            if (system.Factions == null)
            {
                system = GetFactions(systemName, systemId);
            }

            if (system.CelestialBodies == null)
            {
                system = GetCelestialBodies(systemName, systemId);
            }


            return system;
        }

        public EDSMSystem GetCelestialBodies(string systemName, long systemId)
        {
            var sys = getExistingSystem(systemName, systemId);

            if (sys == null || sys.CelestialBodies == null)
            {
                sys = this._service.GetCelestialBodies(systemName, systemId);
                consolide(sys);
            }

            return getExistingSystem(systemName, systemId);
        }

        public EDSMSystem GetFactions(string systemName, long systemId)
        {
            var sys = getExistingSystem(systemName, systemId);

            if (sys == null || sys.Factions == null)
            {
                var sysFactions = this._service.GetFactions(systemName, systemId);
                consolide(sysFactions);
            }

            return getExistingSystem(systemName, systemId); 
        }

        public EDSMSystem GetStations(string systemName, long systemId)
        {
            var sys = getExistingSystem(systemName, systemId);

            if (sys == null || sys.Stations == null)
            {
                sys = this._service.GetStations(systemName, systemId);
                consolide(sys);
            }

            return getExistingSystem(systemName, systemId);
        }

        private void consolide(EDSMSystem system)
        {
            var current = getExistingSystem(system.Name, system.Id);

            if (current == null)
            {
                current = system;
            }

            if (current.Stations == null && system.Stations != null) current.Stations = system.Stations;
            if (current.Factions == null && system.Factions != null) current.Factions = system.Factions;
            if (current.CelestialBodies == null && system.CelestialBodies != null) current.CelestialBodies = system.CelestialBodies;


            var key = "SYSTEM_" + system.Name;

            this._storage.Set(key, current);
        }

        private EDSMSystem getExistingSystem(string systemName, long systemId)
        {
            var key = "SYSTEM_" + systemName;

            var system = _storage.Get(key) as EDSMSystem;

            return system;
        }

    }
}
