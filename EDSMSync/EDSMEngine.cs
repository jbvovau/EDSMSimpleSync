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

        private static ILog log = LogManager.GetLogger(typeof(EDSMEngine));

        // EDSM API
        private ApiEDSM _api;

        // log directory listener
        private LogWatcher _edlogs;

        // manage journal log by batch
        private BagJournal _bag;

        private bool _send;

        // keep last update trace
        private DateTime _lastUpdate;
        private readonly TimeSpan _inactivityToUpdate = TimeSpan.FromSeconds(5) ;

        private readonly string FIELD_LAST_DATE = "last_event_date";

        private DateTime _fromDate;

        private readonly List<string> _discaredEvents = new List<string>();

        public EDSMEngine()
        {
            this._bag = new BagJournal();
            this.InitDiscardedEvents();
        }


        /// <summary>
        /// Elite Dangerous Journal Log Directory
        /// </summary>
        public string Directory { get; set; }

        public string ApiName { get; set; }

        public string ApiKey { get; set; }

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

                //check date
                DateTime date = DateTime.Parse(evt.Timestamp);

                if (date >= this.FromEventDate)
                {
                    log.Info(string.Format("[new event][{0}] {1}", evt.Timestamp, evt.EventName));
                    lock (this._bag)
                    {
                        this._bag.AddEntry(date, line);
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

                // wait 15s

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


        private bool IsDiscardedEvent(JournalEvent evt)
        {
            return this.IsDiscardedEvent(evt.EventName);
        }

        private bool IsDiscardedEvent(string name)
        {
            if (name == null) return true;

            return (this._discaredEvents.Contains(name)) ;
        }

        private void InitDiscardedEvents()
        {
            this._discaredEvents.Clear();
            this._discaredEvents.Add("Music");
            this._discaredEvents.Add("HeatWarning");
            this._discaredEvents.Add("ShipTargeted");
            this._discaredEvents.Add("ReceiveText");
            this._discaredEvents.Add("Shutdown");
            this._discaredEvents.Add("DockingRequested");
            this._discaredEvents.Add("DockingGranted");
            this._discaredEvents.Add("UnderAttack");
        }

    }






}
