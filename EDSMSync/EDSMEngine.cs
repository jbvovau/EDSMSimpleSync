using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
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
        private readonly TimeSpan _inactivityToUpdate = TimeSpan.FromSeconds(15) ;

        private readonly string EDSM_SYNC_JSON = "edsm_sync.json";

        public EDSMEngine()
        {
            this._bag = new BagJournal();
        }


        /// <summary>
        /// Elite Dangerous Journal Log Directory
        /// </summary>
        public string Directory { get; set; }

        public string ApiName { get; set; }

        public string ApiKey { get; set; }

        /// <summary>
        /// Force a last event date
        /// </summary>
        public DateTime LastEventDate { get; set; }

        public DateTime CurrentEventDate { get
            {
                return _bag.CurrentTime;
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
            if (File.Exists(EDSM_SYNC_JSON))
            {
                lock (EDSM_SYNC_JSON)
                {
                    var fileStream = new FileStream(EDSM_SYNC_JSON, FileMode.Open);
                    using (fileStream)
                    {
                        using (var reader = new StreamReader(fileStream, Encoding.UTF8))
                        {
                            var text = reader.ReadToEnd();
                            var obj = JsonConvert.DeserializeObject<SyncData>(text);
                            this.LastEventDate = obj.last_event;
                            this._bag.ForgetBefore(obj.last_event);
                        }
                    }
                }
            }
        }

        private void SaveLastDate()
        {
            var data = new SyncData();
            data.last_event = this.CurrentEventDate;
            var json = JsonConvert.SerializeObject(data);

            lock (EDSM_SYNC_JSON)
            {
                var fileStream = new FileStream(EDSM_SYNC_JSON, FileMode.OpenOrCreate);
                using (fileStream)
                {
                    using (var writer = new StreamWriter(fileStream, Encoding.UTF8))
                    {
                        writer.Write(json);
                        writer.Close();
                    }
                }
            }

        }


        public void Listen()
        {
            // set last event to concider
            this._bag.ForgetBefore(this.LastEventDate);

            this._lastUpdate = DateTime.Now;

            // define new log parser
            this._edlogs = new LogWatcher();
            this._edlogs.NewJournalLogEntry += _edlogs_NewJournalLogEntry;
            this._edlogs.ListenDirectory(this.Directory);

            // read all file to check
            // this._edlogs.ReadAll();

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

                lock (this._bag)
                {
                    this._bag.AddEntry(date, line);
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
            while (this._send)
            {
                Thread.Sleep(250);
                var inactivity = DateTime.Now.Subtract(this._lastUpdate);
                if (inactivity  < this._inactivityToUpdate)
                {
                    continue;
                }

                // wait 15s


                var data = this._bag.NextEntry();

                if (!string.IsNullOrEmpty(data))
                {

                    // blob
                    log.Debug("Send journal to EDSM");
                    var result = this.Api.PostJournalLine(data);
                    log.Debug("result : " + result);
                    if (result.msgnum == 100)
                    {
                        this._bag.ConfirmEntry(data);
                        this.SaveLastDate();
                    }
                } else
                {
                    this._lastUpdate = DateTime.Now;
                    Thread.Sleep(10000);
                }

                
            }
        }


        private class SyncData
        {
            public DateTime last_event;
        }

    }






}
