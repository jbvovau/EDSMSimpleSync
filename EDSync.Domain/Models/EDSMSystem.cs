using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDSMDomain.Models
{
    public class EDSMSystem
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "controllingFaction")]
        public Faction ControllingFaction { get; set; }

        [JsonProperty(PropertyName = "stations")]
        public List<Station> Stations { get; set; }

        [JsonProperty(PropertyName = "factions")]
        public List<Faction> Factions { get; set; }

        [JsonProperty(PropertyName = "bodies")]
        public List<CelestialBody> CelestialBodies { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Name, this.Id);
        }
    }
}
