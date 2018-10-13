using System;
using System.Collections.Generic;
using System.Text;

namespace EDSMSync
{
    public class GameStatus
    {
        public long SystemId;
        public string System;
        public List<string> Coordinates;
        public string StationId;
        public string Station;
        public string ShipId;

        public string Cmdr;

        public bool DontSendEvents ;
    }
}
