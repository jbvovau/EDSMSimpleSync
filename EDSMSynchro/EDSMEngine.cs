using EDLogWatcher;
using EDSMDomain.Models;
using EDSMDomain.Services;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using EDLogWatcher.Parser;
using EDLogWatcher.Watcher;
using EDSync.Core;
using EDSync.Core.Filter;

namespace EDSync.EDSM
{
    /// <summary>
    /// Sync for EDSM Journal Log
    /// </summary>
    public class EDSMEngine : IEntryManager
    {
        private const int MAX_PER_BATCH = 50;

        // sync Details
        public delegate void SyncEvent(string type, string message);
        public event SyncEvent NewSyncEvent;

        #region private fields

        // logger
        private static ILog logger = LogManager.GetLogger(typeof(EDSMEngine));


        // manage journal log by batch
        private SortedList<string, string> _sortedEvents;

        // currently sending 
        private bool _send;

        // keep last update trace
        private readonly TimeSpan _inactivityToUpdate = TimeSpan.FromSeconds(5);

        private readonly string FIELD_LAST_DATE = "last_event_date";

        private GameStatus _gameStatus;

        private DateTime _lastGameStatisDate;
        private DateTime _lastActivity;
        #endregion

        public EDSMEngine()
        {
            _sortedEvents = new SortedList<string, string>();
            _gameStatus = new GameStatus();
        }

        #region properties

        public IServiceJournal ServiceJournal { get; set; }

        public IServiceSystem ServiceSystem { get; set; }

        public IList<string> DiscaredEvents { get; private set; }

        public IEntryFilter EntryFilter { get; set; }

        public GameStatus Status
        {
            get
            {
                return _gameStatus;
            }
        }

        #endregion

        public void Dispose()
        {
            _send = false;
        }


        public void Configure()
        {

            // load discarded Details
            DiscaredEvents = ServiceJournal.GetDiscardedEvents();

            logger.Debug("Discarded Events loaded. Count : " + DiscaredEvents.Count);

            StartSender();

        }

        /// <summary>
        /// New entry 
        /// </summary>
        /// <param name="line"></param>
        public void AddEntry(string line)
        {
            _lastActivity = DateTime.Now;

            try
            {
                // this.Api.PostJournalLine(line);
                JournalEvent evt = JsonConvert.DeserializeObject<JournalEvent>(line);

                // update game status
                UpdateGameStatus(evt);

                //check date
                string key = evt.Timestamp + evt.EventName;
                var date = DateTime.Parse(evt.Timestamp);

                // add game status to line
                line = AddGameStatusToJournalEntry(line);

                if (line != null)
                {
                    if (EntryFilter != null && !EntryFilter.Accepted(line))
                    {
                        return;
                    }

                    customLog(evt, "EVENT", "new event");
                    lock (_sortedEvents)
                    {
                        _sortedEvents[key] = line;
                    }
                }
            }
            catch (Exception ex)
            {
                // todo
                logger.Error("Error parsing line : " + line, ex);
            }
        }


        #region EDSM sender

        /// <summary>
        /// Start Sender to EDSM Thread
        /// </summary>
        private void StartSender()
        {
            _send = true;

            var thread = new Thread(Send);
            thread.Priority = ThreadPriority.BelowNormal;
            thread.IsBackground = true;

            thread.Start();
        }

        /// <summary>
        /// The send to edsm method
        /// </summary>
        private void Send()
        {
            while (_send)
            {
                var inactivity = DateTime.Now.Subtract(_lastActivity);
                if (inactivity < _inactivityToUpdate)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                // list to send
                var list = new List<object>();
                // key to message sent
                var eventSent = new List<string>();
                var toRemove = new List<string>();
                int i = 0;
                string currentLastDate = null;

                lock (_sortedEvents)
                {
                    while (list.Count < MAX_PER_BATCH && i < _sortedEvents.Count)
                    {
                        var key = _sortedEvents.Keys[i++];

                        var data = _sortedEvents[key];
                        if (EntryFilter != null) EntryFilter.Discard(data);
                        var evt = JsonConvert.DeserializeObject<JournalEvent>(data);
                        currentLastDate = evt.Timestamp;

                        if (IsDiscardedEvent(evt))
                        {
                            customLog(evt, "DEBUG", "discarded");
                        }
                        else
                        {
                            // add object as json
                            list.Add(JsonConvert.DeserializeObject(data));
                            eventSent.Add(evt.Timestamp + " - " + evt.EventName.PadRight(25));
                        }
                        toRemove.Add(key);
                    }
                }

                if (list.Count > 0)
                {
                    // post to EDSM server
                    var result = ServiceJournal.PostJournalEntry(JsonConvert.SerializeObject(list));

                    // parse result
                    if (result.MessageNumber == 100)
                    {
                        if (this.EntryFilter != null) EntryFilter.Confirm();

                        // batch result
                        if (result.Details != null && result.Details.Length > 0)
                        {
                            int index = 0;
                            foreach(var detail in result.Details)
                            {
                                string whatWasSent = "";
                                if (eventSent.Count > index)
                                {
                                    whatWasSent = eventSent[index];
                                }

                                var msg = whatWasSent + "[" + detail.MessageNumber + " - " + detail.Message + "]";
                                customLog("SYNC", msg);
                                index++;
                            }
                        }

                      
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        // error
                        toRemove.Clear();
                        Thread.Sleep(15000);
                    }
                }
                else
                {
                   
                    Thread.Sleep(15000);
                }

                // remove pending Details
                lock (_sortedEvents)
                {
                    foreach (var key in toRemove)
                    {
                        _sortedEvents.Remove(key);
                    }
                }
            }
        }

        private void BatchSend()
        {

        }

        #endregion

        /// <summary>
        ///  (see doc here : https://www.edsm.net/fr/api-journal-v1 )
        /// </summary>
        /// <param name="evt"></param>
        private void UpdateGameStatus(JournalEvent evt)
        {

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

        private bool IsDiscardedEvent(JournalEvent evt)
        {
            return IsDiscardedEvent(evt.EventName);
        }

        private bool IsDiscardedEvent(string name)
        {
            if (name == null)
            {
                return true;
            }

            return (DiscaredEvents.Contains(name));
        }


        private void customLog(string type, string message)
        {
            customLog(null, type, message);
        }

        private void customLog(JournalEvent evt, string type, string message)
        {
            StringBuilder sb = new StringBuilder();
            if (evt != null)
            {
                sb.Append(evt.Timestamp);
                sb.Append(" - ");
                sb.Append(evt.EventName.PadRight(25));
            }
            sb.Append(message);

            logger.Info(sb.ToString());

            if (NewSyncEvent != null)
            {
                NewSyncEvent(type, sb.ToString());
            }

        }


    }






}
