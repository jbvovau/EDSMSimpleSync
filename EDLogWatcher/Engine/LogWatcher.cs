using EDLogWatcher.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace EDLogWatcher.Engine
{
    public class LogWatcher : IDisposable
    {
        private static ILog log = LogManager.GetLogger(typeof(LogWatcher));

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

        // file watcher
        private FileSystemWatcher _watcher;

        // files parsers
        private IList<IEDFileParser> _parsers;

        // file queued
        private IList<string> _queued;

        // queue runner
        private Thread _thread;
        private Thread _threadListener;
        private bool _run;

        // current journal log to listen
        private string _mostRecentLog;

        private DateTime _lastGameAlive;

        // listened directory
        public string Directory { get; set; }


        public LogWatcher()
        {
            _queued = new List<string>();
            _parsers = new List<IEDFileParser>();

            _parsers.Add(new JournalLogParser(this));
        }

        public void Dispose()
        {
            this._run = false;
            this._watcher.EnableRaisingEvents = false;
            this._watcher.Dispose();
            this.StopQueue();
        }

        /// <summary>
        /// Listen change of the Log Directory
        /// </summary>
        /// <param name="path"></param>
        public void ListenDirectory(string path)
        {
            log.Debug("Listen to directory : " + path);

            Directory = path;

            _watcher = new FileSystemWatcher();
            _watcher.Path = path;
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.Filter = "*.*";
            _watcher.Changed += new FileSystemEventHandler(OnChanged);
            _watcher.EnableRaisingEvents = true;

            startThreads();
        }

        /// <summary>
        /// Parse all entries
        /// </summary>
        public void ReadAll()
        {
            var info = new DirectoryInfo(Directory);
            var files = info.GetFiles();

            foreach (var file in files)
            {
                AddToQueue(file.FullName);
            }

            if (!_run)
            {
                RunQueue();
            }
        }

        public void StopListen()
        {
            StopQueue();
        }

        /// <summary>
        /// Event when a file is changed
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // log.Info("File Changed : " + e.Name);
            this._lastGameAlive = DateTime.Now;
            // Copies file to another directory.
            //Program.Log("[File][" + e.ChangeType +"] " + e.FullPath);
            AddToQueue(e.FullPath);

        }

        #region queue management

        /// <summary>
        /// Set file in queue and avoid doublons
        /// </summary>
        /// <param name="path"></param>
        private void AddToQueue(string path)
        {
            lock (_queued)
            {
                if (!_queued.Contains(path))
                {
                    // Program.Log("[File][Add to Queue] " + path);
                    _queued.Add(path);
                }
            }
        }

        /// <summary>
        /// Run queue to treat log files
        /// </summary>
        private void RunQueue()
        {
            while (_run)
            {
                bool longwait = false;
                // take current file
                lock (_queued)
                {
                    var toremove = new List<string>();

                    // Program.Log("[Log][PARSING] " + path);
                    foreach (var path in _queued)
                    {
                        if (!_run) break;

                        // set game is alive
                        if (path.EndsWith(".log"))
                        {
                            // keep trace of current log
                            _mostRecentLog = path;
                        }

                        // parse files
                        foreach (var parser in _parsers)
                        {
                            if (parser.Accept(path))
                            {
                                var success = parser.Parse(path);

                                if (success)
                                {
                                    // remove file from queue before beeing parsed (to be replayed in case)
                                    toremove.Add(path);
                                }
                            }
                        }
                    }

                    foreach (var r in toremove)
                    {
                        _queued.Remove(r);
                    }

                }

                // log.Debug(_watcher.EnableRaisingEvents);
                if (_run) Thread.Sleep(5000);
            }
        }

        private void startThreads()
        {

            if (_thread != null)
            {
                // this._thread.
            }

            _run = true;

            // manage files modified in queue
            _thread = new Thread(RunQueue);
            _thread.IsBackground = true;
            _thread.Start();

            // listen files
            _threadListener = new Thread(RunListen);
            _threadListener.IsBackground = true;
            _threadListener.Start();
        }

        private void StopQueue()
        {
            _run = false;

            foreach(var parser in _parsers)
            {
                parser.Stop();
            }
        }

        public void DispatchJournalLog(string data)
        {
            if (NewJournalLogEntry != null)
            {
                NewJournalLogEntry(data);
            }
        }

        public void Dispatch(JournalEvent evt)
        {
            // log.Info(string.Format("[event][{0}] {1}", evt.Timestamp, evt.EventName));
            if (NewJournalLog != null)
            {
                NewJournalLog(evt);
            }
        }

        #endregion

        #region listen management

        private void RunListen()
        {
            while (this._run)
            {
                var span = DateTime.Now.Subtract(this._lastGameAlive);

                if (span.TotalSeconds < 16 && this._mostRecentLog != null)
                {
                    this.AddToQueue(this._mostRecentLog);
                }

                Thread.Sleep(15000);
            }
        }

        #endregion

    }
}
