using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using EDLogs;
using EDLogs.Engine;
using EDLogs.Models;
using log4net;
using Newtonsoft.Json;

namespace EDSMSync
{
    /// <summary>
    /// Sync for EDSM Journal Log
    /// </summary>
    public class EDSMEngine
    {
        // logger
        private static ILog log = LogManager.GetLogger(typeof(EDSMEngine));

        // EDSM API
        private ApiEDSM _api;

        // journal log directory listener
        private LogWatcher _edlogs;

        // manage journal log by batch
        private SortedJournal _bag;

        // currently sending 
        private bool _send;

        // keep last update trace
        private DateTime _lastUpdate;
        private readonly TimeSpan _inactivityToUpdate = TimeSpan.FromSeconds(5) ;

        private readonly string FIELD_LAST_DATE = "last_event_date";

        private DateTime _fromDate;

        private GameStatus _gameStatus;

        public EDSMEngine()
        {
            this._bag = new SortedJournal();
            this._gameStatus = new GameStatus();
        }


        /// <summary>
        /// Elite Dangerous Journal Log Directory
        /// </summary>
        public string Directory { get; set; }

        public string ApiName { get; set; }

        public string ApiKey { get; set; }

        public IList<string> DiscaredEvents { get; private set; }

        public DateTime FromEventDate {
            get
            {
                return _fromDate;
            }
            set
            {
                _fromDate = value;
                _bag.ForgetBefore(_fromDate);
            }
        }

        private ApiEDSM Api
        {
            get
            {
                if (_api == null)
                {
                    _api = new ApiEDSM();
                    _api.CommanderName = ApiName;
                    _api.ApiKey = ApiKey;
                    _api.FromSoftware = "EDSMSimpleSync";
                    _api.FromSoftwareVersion = "0.0.1";
                }

                return _api;
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
                    this.FromEventDate = last;
                }
            }
        }

        private void SaveLastDate()
        {
            var date = this._bag.CurrentTime;
            if (this.FromEventDate < date) FromEventDate = date;

            EDConfig.Instance.Set(FIELD_LAST_DATE, date.ToString());
        }


        public void Listen()
        {
            // set last event to concider
            this._bag.ForgetBefore(this.FromEventDate);

            this._lastUpdate = DateTime.Now;

            // load discarded events
            this.DiscaredEvents = this.Api.GetDiscardedEvents();

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
                DateTime date = DateTime.Parse(evt.Timestamp);

                if (date >= this.FromEventDate)
                {
                    // add game status to line
                    line = this.AddGameStatusToJournalEntry(line);

                    if (line != null)
                    {

                        log.Info(string.Format("[new event][{0}] {1}", evt.Timestamp, evt.EventName));
                        lock (this._bag)
                        {
                            this._bag.AddEntry(date, line);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // todo
                log.Error("Error parsing line : " + line, ex);
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
                    Thread.Sleep(500);
                    continue;
                }

                var data = this._bag.NextEntry();

                if (!string.IsNullOrEmpty(data))
                {
                    var evt = JsonConvert.DeserializeObject<JournalEvent>(data);

                    if (IsDiscardedEvent(evt))
                    {
                        log.Debug(string.Format("[Discarded][{0}] {1}", evt.Timestamp, evt.EventName));
                        this._bag.ConfirmEntry(data);
                        continue;
                    }

                    // blob
                    var result = this.Api.PostJournalLine(data);
                    if (result.msgnum == 100)
                    {
                        // text result
                        var msg = "";
                        if (result.events != null && result.events.Length > 0)
                        {
                            msg = "[" + result.events[0].msgnum + " - " + result.events[0].msg + "]";
                        }

                        
                        if (evt != null)
                        {
                            log.Debug(string.Format("[Sent to EDSM][{0}] {1} : {2}", evt.Timestamp, evt.EventName, msg));
                        }
                        else
                        {
                            log.Debug("[Sent to EDSM]" + data);
                        }


                        this._bag.ConfirmEntry(data);
                        if (count++ % 10 == 0)
                        {
                            if (this._bag.CurrentTime > this.FromEventDate)
                            {
                                this.FromEventDate = this._bag.CurrentTime;
                            }
                            this.SaveLastDate();
                        }
                        Thread.Sleep(250);
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

  
    }






}
