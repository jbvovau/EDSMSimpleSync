using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDSMDomain.Models
{
    public class Faction
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "allegiance")]
        public string Allegiance { get; set; }

        [JsonProperty(PropertyName = "government")]
        public string Government { get; set; }

        [JsonProperty(PropertyName = "influence")]
        public decimal Influence { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "isPlayer")]
        public bool IsPlayer { get; set; }

        public override string ToString()
        {
            return Name;
        }

    }
}
