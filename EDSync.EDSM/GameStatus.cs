using System;
using System.Collections.Generic;
using System.Text;

namespace EDSync.EDSM
{
    public class GameStatus
    {
        public long SystemId;
        public string System;
        public List<decimal> Coordinates;
        public long StationId;
        public string Station;
        public string ShipId;

        public string Cmdr;

        public bool DontSendEvents ;
    }
}
