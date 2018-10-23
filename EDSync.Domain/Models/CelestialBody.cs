using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDSMDomain.Models
{
    public class CelestialBody
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string BodyType { get; set; }

        [JsonProperty(PropertyName = "rings")]
        public object [] Rings { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Name, this.Id);
        }
    }
}
