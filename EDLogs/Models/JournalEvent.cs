using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace EDLogs.Models
{
    public class JournalEvent
    {
        [JsonProperty(PropertyName = "timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty(PropertyName = "event")]
        public string EventName { get; set; }

        #region header

        [JsonProperty(PropertyName = "part")]
        public int Part { get; set; }

        [JsonProperty(PropertyName = "language")]
        public string Language { get; set; }

        [JsonProperty(PropertyName = "gameversion")]
        public string GameVersion { get; set; }

        [JsonProperty(PropertyName = "build")]
        public string Build { get; set; }

        #endregion

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        #region login

        [JsonProperty(PropertyName = "package")]
        public string Package { get; set; }

        public string Commander { get; set; }

        public string Ship { get; set; }

        public long ShipID { get; set; }

        public string GameMode { get; set; }

        public string Group { get; set; }

        public long Credits { get; set;  }

        public long Load { get; set; }

        #endregion

        #region progress

        public int Combat { get; set; }
        public int Trade { get; set; }
        public int Explore { get; set; }
        public int Empire { get; set; }
        public int Federation { get; set; }
        public int CQC { get; set; }

        #endregion

        #region travel

        public string StationName { get; set; }
        public string StationType { get; set; }
        public string Outpost { get; set; }
        public string StarSystem { get; set; }
        public string Faction { get; set; }
        public string FactionState { get; set; }
        public string Allegiance { get; set; }
        public string Economy { get; set; }
        public string Government { get; set; }
        public string Security { get; set; }
        #endregion

        #region Docking
        public string Reason { get; set; }
        public string LandingPad { get; set; }
        #endregion

        #region FSD Jump

        public decimal [] StarPos { get; set; }
        public string Body { get; set; }
        public string BodyType { get; set; }
        public decimal JumpDist { get; set; }
        public decimal FuelUsed { get; set; }
        public decimal FuelLevel { get; set; }
        public string Power { get; set; }
        public string PowerplayState { get; set; }
        public string [] Powers { get; set; }
        #endregion

        public decimal Latittude { get; set; }
        public decimal Longitude { get; set; }

        #region Location 
        public bool Docked { get; set; }

        #endregion

        #region combat

        public string Target { get; set; }
        public string VictimFaction { get; set; }
        public long TotalReward { get; set; }
        public object[] Rewards { get; set; }
        public string AwardingFaction { get; set; }
        public int SharedWithOthers { get; set; }

        public string TaKillerNamerget { get; set; }
        public string KillerShip { get; set; }
        public string KillerRank { get; set; }

        public string Interdictor { get; set; }
        public bool IsPlayer { get; set; }

        public bool ShieldsUp { get; set; }

        #endregion

        public decimal Health { get; set; }

        #region interdiction

        public bool Submitted { get; set; }
        public bool Success { get; set; }
        public bool Interdicted { get; set; }
        public string CombatRank { get; set; }

        #endregion

        #region exploration

        #endregion

    }
}
