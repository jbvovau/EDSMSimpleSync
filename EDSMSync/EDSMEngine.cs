using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using EDLogs.Engine;
using EDLogs.Models;
using Newtonsoft.Json;

namespace EDSMSync
{
    public class EDSMEngine
    {
        // EDSM API
        private ApiEDSM _api;

        // log directory listener
        private LogManager _edlogs;

        // manage journal log by batch
        private BagJournal _bag;

        private bool _send;

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

        public void Listen()
        {

            this._bag.ForgetBefore(this.LastEventDate);

            // define new log parser
            this._edlogs = new LogManager();
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


        private void StartSender()
        {
            this._send = true;

            var thread = new Thread(this.Send);
            thread.Priority = ThreadPriority.BelowNormal;
            thread.IsBackground = true;

            thread.Start();
        }

        private void Send()
        {
            while (this._send)
            {
                // wait 15s
                Thread.Sleep(15000);

                var data = this._bag.NextBatch(1);

                if (!string.IsNullOrEmpty(data))
                {

                    // blob
                    var result = this.Api.PostJournalLine(data);

                    this._bag.CommitBatch(data);
                }

                
            }
        }

    }
}
