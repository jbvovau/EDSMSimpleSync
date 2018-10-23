using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDSMDomain.Models
{
    public class Station
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string StationType { get; set; }

        [JsonProperty(PropertyName = "distanceToArrival")]
        public decimal DistanceToArrival { get; set; }

        [JsonProperty(PropertyName = "allegiance")]
        public string Allegiance { get; set; }

        [JsonProperty(PropertyName = "government")]
        public string Government { get; set; }

        [JsonProperty(PropertyName = "economy")]
        public string Economy { get; set; }

        [JsonProperty(PropertyName = "haveMarket")]
        public bool HaveMarket { get; set; }

        [JsonProperty(PropertyName = "haveShipyard")]
        public bool HaveShipyard { get; set; }

        [JsonProperty(PropertyName = "controllingFaction")]
        public Faction ControllingFaction { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
