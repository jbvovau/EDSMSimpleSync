using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using EDLogWatcher;
using EDLogWatcher.Engine;
using EDSMDomain.Api;
using EDSMDomain.Models;
using EDSMDomain.Services;
using log4net;
using Newtonsoft.Json;

namespace EDSMSync
{
    /// <summary>
    /// Sync for EDSM Journal Log
    /// </summary>
    public class EDSMEngine : IDisposable
    {
        // sync events
        public delegate void SyncEvent(string type, string message);
        public event SyncEvent NewSyncEvent;

        // logger
        private static ILog logger = LogManager.GetLogger(typeof(EDSMEngine));

        // EDSM API
        private IServiceJournal _serviceJournale;

        // journal log directory listener
        private LogWatcher _edlogs;

        // manage journal log by batch
        private SortedList<string,string> _sortedEvents;

        // currently sending 
        private bool _send;

        // keep last update trace
        private DateTime _dateLastEvent;
        private readonly TimeSpan _inactivityToUpdate = TimeSpan.FromSeconds(2) ;

        private readonly string FIELD_LAST_DATE = "last_event_date";

        private GameStatus _gameStatus;
        private DateTime _lastUpdate;
        private DateTime _lastGameStatisDate;

        private HashSet<string> _blockedEvents;

        public EDSMEngine()
        {
            this._sortedEvents = new SortedList<string, string>();
            this._gameStatus = new GameStatus();

            this._blockedEvents = new HashSet<string>();
        }

        #region properties

        /// <summary>
        /// Elite Dangerous Journal Log Directory
        /// </summary>
        public string Directory { get; set; }

        public string ApiName { get; set; }

        public string ApiKey { get; set; }

        public IList<string> DiscaredEvents { get; private set; }

        public DateTime LastEventDate { get; set; }

 
        private IServiceJournal ServiceJournal
        {
            get
            {
                if (_serviceJournale == null)
                {
                    this._serviceJournale = new SerivceJournal(this.ApiName, this.ApiKey);
                }
                return this._serviceJournale;
            }
        }
        #endregion

        public void Dispose()
        {
            this._send = false;
            if (this._edlogs != null)
            {
                this._edlogs.Dispose();
                this._edlogs = null;
            }
        }

        public void LoadLastDate()
        {
            string data = EDConfig.Instance.Get(FIELD_LAST_DATE);
            if (data != null)
            {
                DateTime last;
                if (DateTime.TryParse(data, out last))
                {
                    this.LastEventDate = last;
                }

                if (DateTime.Now.Subtract(LastEventDate) > TimeSpan.FromDays(7))
                {
                    LastEventDate = DateTime.Now.Subtract(TimeSpan.FromDays(7));

                    this.customLog("WARN", "Events older than seven days are not sent, please sync manually");
                }
            }
        }

        private void SaveLastDate()
        {
            EDConfig.Instance.Set(FIELD_LAST_DATE, this.LastEventDate.ToString());
        }


        public void Listen()
        {

            this._lastUpdate = DateTime.Now;

            // load discarded events
            this.DiscaredEvents = this.ServiceJournal.GetDiscardedEvents();

            logger.Debug("Discarded Events loaded. Count : " + this.DiscaredEvents.Count);

            // define new log parser
            this._edlogs = new LogWatcher();
            this._edlogs.NewJournalLogEntry += _edlogs_NewJournalLogEntry;
            this._edlogs.ListenDirectory(this.Directory);

            // read all file to check
            this._edlogs.ReadAll();

            this.StartSender();

        }

        /// <summary>
        /// New entry 
        /// </summary>
        /// <param name="line"></param>
        private void _edlogs_NewJournalLogEntry(string line)
        {
            this._lastUpdate = DateTime.Now;

            try
            {
                // this.Api.PostJournalLine(line);
                JournalEvent evt = JsonConvert.DeserializeObject<JournalEvent>(line);

                // update game status
                this.UpdateGameStatus(evt);


                //check date
                string key = evt.Timestamp + evt.EventName;
                var date = DateTime.Parse(evt.Timestamp);

                if (date >= this.LastEventDate)
                {
                    // add game status to line
                    line = this.AddGameStatusToJournalEntry(line);


                    if (this.isBlocked(line))
                    {
                        return;
                    }

                    if (line != null)
                    {
                        customLog(evt, "EVENT", "new event");
                        lock (this._sortedEvents)
                        {
                            this._sortedEvents[key] = line;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // todo
                logger.Error("Error parsing line : " + line, ex);
            }
        }

        /// <summary>
        /// Start Synchro
        /// </summary>
        public void Listen(string path)
        {
            this.Directory = path;
            this.Listen();
        }

        /// <summary>
        /// Start Sender to EDSM Thread
        /// </summary>
        private void StartSender()
        {
            this._send = true;

            var thread = new Thread(this.Send);
            thread.Priority = ThreadPriority.BelowNormal;
            thread.IsBackground = true;

            thread.Start();
        }

        /// <summary>
        /// The send to edsm method
        /// </summary>
        private void Send()
        {
            int count = 0;

            while (this._send)
            {
                var inactivity = DateTime.Now.Subtract(this._lastUpdate);
                if (inactivity  < this._inactivityToUpdate)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                string data = null;
                string key = null;

                if(this._sortedEvents.Count > 0)
                {
                    lock (_sortedEvents)
                    {
                        key = _sortedEvents.Keys[0];
                        data = _sortedEvents[key];
                    }
                } 

                if (!string.IsNullOrEmpty(data))
                {
                    var evt = JsonConvert.DeserializeObject<JournalEvent>(data);

                    if (this.isBlocked(data))
                    {
                        lock (_sortedEvents)
                        {
                            this._sortedEvents.Remove(key);
                        }
                        continue;
                    }

                    if (IsDiscardedEvent(evt))
                    {
                        customLog(evt, "DEBUG", "discarded");
                        lock (_sortedEvents)
                        {
                            this._sortedEvents.Remove(key);
                        }
                        this.SetLastDate(evt);
                        this.blockEvent(data);
                        continue;
                    }

                    // post to EDSM server
                    var result = this.ServiceJournal.PostJournalEntry(data);

                    // text result
                    var msg = "";
                    if (result.events != null && result.events.Length > 0)
                    {
                        msg = "[" + result.events[0].msgnum + " - " + result.events[0].msg + "]";
                        this.SetLastDate(evt);
                    } else
                    {
                        msg = "[" + result.msgnum + " - " + result.msg + "]";
                    }

                    // ok
                    if (result.msgnum == 100)
                    {


                        if (evt != null)
                        {
                            customLog(evt, "SYNC", "sent to EDSM : " + msg);
                        }
                        else
                        {
                            logger.Info("[Sent to EDSM]" + data);
                        }

                        lock (_sortedEvents)
                        {
                            this._sortedEvents.Remove(key);
                        }

                        this.blockEvent(data);

                        if (count++ % 10 == 0)
                        {
                            this.SaveLastDate();
                        }
                        Thread.Sleep(500);
                    } else
                    {
                        // not ok......
                        customLog(evt, "ERROR", msg);
                        if (result.msgnum == 203)
                        {
                            // for the moment : break
                        }
                    }
                } else
                {
                    this._lastUpdate = DateTime.Now;
                    this.SaveLastDate();
                    Thread.Sleep(10000);
                }

                
            }
        }


        /// <summary>
        ///  (see doc here : https://www.edsm.net/fr/api-journal-v1 )
        /// </summary>
        /// <param name="evt"></param>
        private void UpdateGameStatus(JournalEvent evt)
        {

            DateTime date = DateTime.Parse(evt.Timestamp);
            if (date < this._lastGameStatisDate)
            {
                return;
            }

            this._lastGameStatisDate = date;

            switch (evt.EventName)
            {
                case "LoadGame":
                    _gameStatus.SystemId = 0;
                    _gameStatus.System = null;
                    _gameStatus.Coordinates = null;
                    _gameStatus.StationId = null;
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
                    _gameStatus.StationId = null;
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

                    break;
                case "JoinACrew":
                case "QuitACrew":
                    _gameStatus.DontSendEvents = (evt.EventName == "JoinACrew" && evt.Captain != _gameStatus.Cmdr);
                    _gameStatus.System = null;
                    _gameStatus.SystemId = 0;
                    _gameStatus.Coordinates = null;
                    _gameStatus.StationId = null;
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
            // TODO (bug) entry._systemCoordinates = _gameStatus.Coordinates;
            entry._marketId = _gameStatus.StationId;
            entry._shipId = _gameStatus.ShipId;

            var newdata = JsonConvert.SerializeObject(entry);


            return newdata;
        }

        private bool IsDiscardedEvent(JournalEvent evt)
        {
            return this.IsDiscardedEvent(evt.EventName);
        }

        private bool IsDiscardedEvent(string name)
        {
            if (name == null) return true;

            return (this.DiscaredEvents.Contains(name)) ;
        }


        private void SetLastDate(JournalEvent evt)
        {
            DateTime date = DateTime.Parse(evt.Timestamp);

            if (this.LastEventDate < date) this.LastEventDate = date;
        }
  
        private void customLog(string type, string message)
        {
            this.customLog(null, type, message);
        }

        private void customLog(JournalEvent evt, string type, string message)
        {
            StringBuilder sb = new StringBuilder();
            if (evt != null)
            {
                sb.Append('[');
                sb.Append(evt.Timestamp);
                sb.Append("] [");
                sb.Append(evt.EventName.PadRight(20));
                sb.Append("] ");
            }
            sb.Append(message);

            logger.Info(sb.ToString());

            if (this.NewSyncEvent != null)
            {
                this.NewSyncEvent(type, sb.ToString()) ;
            }

        }

        private void blockEvent(string data)
        {
            // test if block is too big

            if (_blockedEvents.Count > 50)
            {
                var toRemove = new List<string>();

                var purgeDate = this.LastEventDate.Subtract(TimeSpan.FromMinutes(5));
                foreach(var evt in _blockedEvents)
                {
                    var obj = JsonConvert.DeserializeObject<JournalEvent>(evt);
                    var date = DateTime.Parse(obj.Timestamp);
                    if (date < purgeDate)
                    {
                        toRemove.Add(evt);
                    }
                }

                foreach(var removed in toRemove)
                {
                    this._blockedEvents.Remove(removed);
                }
            }

            this._blockedEvents.Add(data);
        }

        private bool isBlocked(string data)
        {
            return this._blockedEvents.Contains(data);
        }

    }






}
