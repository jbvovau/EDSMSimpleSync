using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using EDLogs.Models;

namespace EDLogs.Engine
{
    public class LogManager
    {

        public delegate void JournalLogHandler(JournalEvent e);

        public delegate void JournalLogEntryHandler(string line);

        /// <summary>
        ///  when a new entry comes in live
        /// </summary>
        public event JournalLogHandler NewJournalLog;

        public event JournalLogEntryHandler NewJournalLogEntry;

        /// <summary>
        /// Every journal log parsed, even old ones
        /// </summary>
        public event JournalLogHandler AllJournalLog;

        // files parsers
        private IList<IEDFileParser> _parsers;

        // file queued
        private IList<string> _queued;

        // queue runner
        private Thread _thread;
        private bool _run;

        // listened directory
        public string Directory { get; set; }

       
        public LogManager()
        {
            this._queued = new List<string>();
            this._parsers = new List<IEDFileParser>();

            this._parsers.Add(new JournalLogParser(this));
        }

        /// <summary>
        /// Listen change of the Log Directory
        /// </summary>
        /// <param name="path"></param>
        public void ListenDirectory(string path)
        {
            this.Directory = path;

            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.log";
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;

            this.StartQueue();
        }

        /// <summary>
        /// Parse all entries
        /// </summary>
        public void ReadAll()
        {
            var info = new DirectoryInfo(this.Directory);
            var files = info.GetFiles();

            foreach (var file in files)
            {
                this.AddToQueue(file.FullName);
            }

            if (!this._run)
            {
                this.RunQueue();

            }
        }

        public void StopListen()
        {
            this.StopQueue();
        }

        /// <summary>
        /// Event when a file is changed
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Copies file to another directory.
            //Program.Log("[File][" + e.ChangeType +"] " + e.FullPath);
            this.AddToQueue(e.FullPath);

        }

        #region queue management

        /// <summary>
        /// Set file in queue and avoid doublons
        /// </summary>
        /// <param name="path"></param>
        private void AddToQueue(string path)
        {
            lock (this._queued)
            {
                if (!this._queued.Contains(path))
                {
                    // Program.Log("[File][Add to Queue] " + path);
                    this._queued.Add(path);
                }
            }
        }

        /// <summary>
        /// Run queue to treat log files
        /// </summary>
        private void RunQueue()
        {
            while (this._run)
            {

                string path = null;

                // take current file
                lock (this._queued)
                {
                    if (this._queued.Count > 0)
                    {
                        path = this._queued[0];
                    }
                }

                if (path != null)
                {
                    // Program.Log("[Log][PARSING] " + path);

                    // remove file from queue before beeing parsed (to be replayed in case)
                    lock (this._queued)
                    {
                        this._queued.Remove(path);
                    }

                    foreach (var parser in _parsers)
                    {
                        if (parser.Accept(path))
                        {
                            parser.Parse(path);
                        }
                    }

                    // Program.Log("[Log][DONE] " + path);
                }

                Thread.Sleep(1000);
            }
        }

        private void StartQueue()
        {

            if (this._thread != null)
            {
                // this._thread.
            }

            this._run = true;

            this._thread = new Thread(this.RunQueue);
            this._thread.Start();
        }

        private void StopQueue()
        {
            this._run = false;
        }

        public void DispatchJournalLog(string data)
        {
            if (this.NewJournalLogEntry != null)
            {
                this.NewJournalLogEntry(data);
            }
        }

        public void Dispatch(JournalEvent evt)
        {
            if (this.NewJournalLog != null)
            {
                this.NewJournalLog(evt);
            }
        }

        #endregion


    }
}
