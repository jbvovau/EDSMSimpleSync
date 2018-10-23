using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;

namespace EDSync.Inara.Api
{
    /// <summary>
    /// Inara basic request
    /// </summary>
    public class InaraRequest
    {
        public InaraRequest()
        {
            this.Events = new List<InaraEvent>();
        }

        [JsonProperty(PropertyName = "header")]
        public Header Header;

        [JsonProperty(PropertyName = "events")]
        public IList<InaraEvent> Events;

        public void SetCustomResponse(long customId, int status)
        {
            var evt = this.Events.FirstOrDefault(e => e.CustomID == customId);
            if (evt != null)
            {
                evt.Result = status;
            }
        }

    }


    /// <summary>
    /// Inara Header
    /// </summary>
    public class Header
    {
        public string appName;
        public string appVersion;
        public bool isDeveloped;
        public string APIkey;
        public string commanderName;
    }

    public class InaraEvent
    {
        private static long _currentId = 0;
            
        public InaraEvent()
        {
            this.CustomID = (++_currentId);
        }

        public InaraEvent(string name)
        {
            this.CustomID = (++_currentId);
            this.EventName = name;
        }

        [JsonProperty(PropertyName = "eventName")]
        public string EventName;

        [JsonProperty(PropertyName = "eventTimestamp")]
        public string Timestamp;

        [JsonProperty(PropertyName = "eventData")]
        public object Data;

        [JsonProperty(PropertyName = "eventCustomID")]
        public long CustomID { get; set; }

        public int Result { get; set; }

        /// <summary>
        /// Add custom data
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddData(string key, object value)
        {
            var dic = this.Data as Dictionary<string, object>;
            if (dic == null) dic = new Dictionary<string, object>();
            dic[key] = value;
            this.Data = dic;
        }
    }

    public class InaraResponse {

        [JsonProperty(PropertyName = "events")]
        public List<InaraResponseEvent> Events { get; set; }
    }

    public class InaraResponseEvent
    {
        [JsonProperty(PropertyName = "eventStatus")]
        public int eventStatus;

        [JsonProperty(PropertyName = "eventCustomID")]
        public long CustomID;
    }
}
