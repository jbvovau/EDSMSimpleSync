using System;
using EDSMDomain.Services;
using EDSMSimpleSync.Utils;
using EDSync.Core.Storage;
using EDSync.Services;
using NUnit.Framework;

namespace EDSMDomain.Tests
{
    [TestFixture]
    public class ServiceSystemTest
    {
        [Test]
        public void TestTheTest()
        {
            var service = this.GetServiceSystem();

            var stations = service.GetStations("Dummy" ,0);
            Assert.IsNotNull(stations);
            Assert.IsNotNull(stations.Stations);

            var factions = service.GetFactions("Dummy", 0);
            Assert.IsNotNull(factions);
            Assert.IsNotNull(factions.Factions);

            var bodies = service.GetCelestialBodies("Dummy", 0);
            Assert.IsNotNull(bodies);
            Assert.IsNotNull(bodies.CelestialBodies);

        }

        [Test]
        public void TestCacheComplete()
        {
            var cached = new CacheServiceSystem(this.GetServiceSystem(), this.GetStorage());

            var sys = cached.GetSystem("Dummy", 1);

            Assert.IsNotNull(sys);
            Assert.IsNotNull(sys.Stations);
            Assert.IsNotNull(sys.Factions);
            Assert.IsNotNull(sys.CelestialBodies);
        }

        [Test]
        public void TestThingAreCached()
        {
            var cached = new CacheServiceSystem(this.GetServiceSystem(), this.GetStorage());

            var stations = cached.GetStations("Dummy", 0);
            Assert.IsNotNull(stations);
            Assert.IsNotNull(stations.Stations);

            var s2 = cached.GetStations("Dummy", 0);

            Assert.AreEqual(stations.Stations[0].Id, s2.Stations[0].Id);


            // test factions
            var factions = cached.GetStations("Dummy", 0);
            Assert.AreEqual(stations.Stations[0].Id, factions.Stations[0].Id);

        }

        private IServiceSystem GetServiceSystem()
        {
            return new DumbSystemService();
        }

        public IStorage GetStorage()
        {
            return new MemoryStorage();
        }
    }
}
