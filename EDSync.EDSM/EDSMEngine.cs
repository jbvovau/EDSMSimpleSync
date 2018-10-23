using EDSync.Core;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using EDSMDomain.Models;
using EDSMDomain.Services;
using EDSync.Core.Filter;

namespace EDSync.EDSM
{
    /// <summary>
    /// Sync for EDSM Journal Log
    /// </summary>
    public class EDSMEngine : SyncPlugin
    {
        private const int MAX_PER_BATCH = 50;


        #region private fields

        // logger
        private static ILog logger = LogManager.GetLogger(typeof(EDSMEngine));

        private GameStatus _gameStatus;

        private DateTime _lastGameStatisDate;
        #endregion

        public EDSMEngine(IEntryFilter filter) : base(filter, "EDSM")
        {
            _gameStatus = new GameStatus();
        }

        #region properties

        public IServiceSystem ServiceSystem { get; set; }

        public IList<string> DiscaredEvents { get; private set; }

        public GameStatus Status
        {
            get
            {
                return _gameStatus;
            }
        }

        #endregion



        public void Configure()
        {
            // load discarded Details
            DiscaredEvents = ServiceJournal.GetDiscardedEvents();

            logger.Debug("Discarded Events loaded. Count : " + DiscaredEvents.Count);

        }

        /// <summary>
        /// New entry 
        /// </summary>
        /// <param name="line"></param>
        public override bool AddEntry(string line)
        {
            bool result = false;

            try
            {
                // this.Api.PostJournalLine(line);
                var name = Utils.GetName(line);
                if (this.IsDiscardedEvent(name))
                {
                    return false;
                }

                // update game status
                UpdateGameStatus(line);

                // add game status to line
                line = AddGameStatusToJournalEntry(line);

                if (line != null)
                {
                    result = base.AddEntry(line);
                }
            }
            catch (Exception ex)
            {
                // todo
                logger.Error("Error parsing line : " + line, ex);
            }

            return result;
        }


        /// <summary>
        ///  (see doc here : https://www.edsm.net/fr/api-journal-v1 )
        /// </summary>
        /// <param name="evt"></param>
        private void UpdateGameStatus(string line)
        {

            var evt = JsonConvert.DeserializeObject<JournalEvent>(line);

            DateTime date = DateTime.Parse(evt.Timestamp);
            if (date < _lastGameStatisDate)
            {
                return;
            }

            _lastGameStatisDate = date;

            switch (evt.EventName)
            {
                case "LoadGame":
                    _gameStatus.SystemId = 0;
                    _gameStatus.System = null;
                    _gameStatus.Coordinates = null;
                    _gameStatus.StationId = 0;
                    _gameStatus.Station = null;
                    break;
                case "ShipyardBuy":
                    _gameStatus.ShipId = null;
                    break;
                case "SetUserShipName":
                case "ShipyardSwap":
                case "Loadout":
                    _gameStatus.ShipId = evt.ShipID.ToString();
                    break;
                case "Undocked":
                    _gameStatus.Station = null;
                    _gameStatus.StationId = 0;
                    break;
                case "Location":
                case "FSDJump":
                case "Docked":

                    if (evt.StarSystem != _gameStatus.System)
                    {
                        _gameStatus.Coordinates = null;
                    }

                    if (evt.StarSystem != "ProvingGround" && evt.StarSystem != "CQC")
                    {
                        if (evt.SystemAddress > 0)
                        {
                            _gameStatus.SystemId = evt.SystemAddress;
                        }

                        _gameStatus.System = evt.StarSystem;

                        if (evt.StarPos != null)
                        {
                            _gameStatus.Coordinates = evt.StarPos;
                        }
                    }
                    else
                    {
                        _gameStatus.System = null;
                        _gameStatus.SystemId = 0;
                        _gameStatus.Coordinates = null;
                    }

                    if (evt.MarketID != 0)
                    {
                        _gameStatus.StationId = evt.MarketID;
                    }
                    if (evt.StationName != null)
                    {
                        _gameStatus.Station = evt.StationName;
                    }

                    break;
                case "JoinACrew":
                case "QuitACrew":
                    _gameStatus.DontSendEvents = (evt.EventName == "JoinACrew" && evt.Captain != _gameStatus.Cmdr);
                    _gameStatus.System = null;
                    _gameStatus.SystemId = 0;
                    _gameStatus.Coordinates = null;
                    _gameStatus.StationId = 0;
                    _gameStatus.Station = null;
                    break;
            }



        }

        private string AddGameStatusToJournalEntry(string data)
        {
            if (_gameStatus.DontSendEvents)
            {
                return null;
            }

            dynamic entry = JsonConvert.DeserializeObject(data);

            entry._stationName = _gameStatus.Station;
            entry._systemAddress = _gameStatus.SystemId;
            entry._systemName = _gameStatus.System;

            entry._marketId = _gameStatus.StationId;
            entry._shipId = _gameStatus.ShipId;

            if (_gameStatus.Coordinates != null)
            {
                // hack with the dynamic type
                var coor = JsonConvert.SerializeObject(_gameStatus.Coordinates);
                entry._systemCoordinates = JsonConvert.DeserializeObject(coor);
            }

            var newdata = JsonConvert.SerializeObject(entry);


            return newdata;
        }

        private bool IsDiscardedEvent(string name)
        {
            if (name == null)
            {
                return true;
            }

            return (DiscaredEvents.Contains(name));
        }


    }






}
