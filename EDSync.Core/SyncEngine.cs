using EDLogWatcher.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDLogWatcher.Watcher;
using EDSync.Core.Filter;
using System.Threading;
using EDSync.Core;

namespace EDUploader
{
    /// <summary>
    /// 
    /// </summary>
    public class SyncEngine : IEntryManager
    {
        public delegate void SyncMessage(string source, string type, string message);
        public event SyncMessage SyncMessageHandler;

        private bool _sending;
        private Thread _sender;

        private bool _paused;

        private int _countNewEntry = 0;

        public SyncEngine(IDirectoryListener directoryListener)
        {
            this.DirectoryListener = directoryListener;
            this.EntryManagers = new List<IEntryManager>();
            this._paused = false;
        }

        /// <summary>
        /// Enabled by default
        /// </summary>
        public bool Enabled => true;

        /// <summary>
        /// The directory listener
        /// </summary>
        public IDirectoryListener DirectoryListener { get; private set; }

        /// <summary>
        /// Parsers of every now file found by the DirectoryListener
        /// </summary>
        public ICollection<IFileParser> Parsers
        {
            get { return this.DirectoryListener.Parsers; }
        }

        /// <summary>
        /// Manage every new log entry found by the parsers
        /// </summary>
        public IList<IEntryManager> EntryManagers { get; private set; }

        public IEntryFilter EntryFilter { get; private set; }

        public void Dispose()
        {
            this.Stop();
        }

        /// <summary>
        /// Add a file parser
        /// </summary>
        /// <param name="parser"></param>
        public void Add(IFileParser parser)
        {
            this.DirectoryListener.Add(parser);

            var entryParser = parser as IEntryParser;
            if (entryParser != null)
            {
                entryParser.Add(this);
            }
        }

        public void Add(SyncPlugin plugin)
        {
            this.Add(plugin as IEntryManager);

            plugin.PluginEventHandler += this.pluginMessage;
        }

        public void Add(IEntryManager manager)
        {
            this.EntryManagers.Add(manager);
        }

        public void Listen()
        {
            
            this.build();
            this.testConnections();

            this.DirectoryListener.Listen();
            this.DirectoryListener.ReadAll();

            this.startSending();

            customMessage("SYNC", "INFO", "Synchro started");
        }

        public void Stop()
        {
            this.DirectoryListener.Stop();
            this._sending = false;
            customMessage("SYNC", "INFO", "Synchro Stopped");
        }

        /// <summary>
        /// Add a new entry, feed by Entry Parsers
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool AddEntry(string data)
        {
            this._paused = false;

            foreach (var manager in this.EntryManagers)
            {
                var isnew = manager.AddEntry(data);
                if (isnew)
                {
                    if (_countNewEntry++ < 50)
                    {
                        string name = Utils.GetName(data);
                        string timestamp = Utils.GetTimestamp(data);
                        customMessage("SYNC", "DEBUG", "New Entry : " + timestamp + " " + name);
                    }
                    if (_countNewEntry == 50)
                    {
                        customMessage("SYNC", "DEBUG", "(...wow so much entries very journal)");
                    }
                }
            }

            return true;
        }

        private void Send()
        {
            bool somethingHappened = false;

            foreach(var em in this.EntryManagers)
            {
                var syncPlugin = em as SyncPlugin;
                if (syncPlugin != null)
                {
                    // check if enabled
                    if (!syncPlugin.Enabled) continue;

                    // check last activity
                    var span = DateTime.Now.Subtract(syncPlugin.LastActivity);
                    if (span < TimeSpan.FromSeconds(5))
                    {
                        somethingHappened = true;
                        continue;
                    }

                    var list = syncPlugin.Next(syncPlugin.MaxPerBatch);

                    if (list.Count > 0)
                    {
                        somethingHappened = true;
                        _countNewEntry = 0;

                        customMessage(syncPlugin.Name, "SYNC", "Start sending data : " + list.Count);
                        var response = syncPlugin.ServiceJournal.PostJournalEntry(list);
                        if (response.MessageNumber == 100)
                        {
                            // build reply
                            for (int i = 0; i < list.Count; i++)
                            {
                                var evt = list[i];
                                var name = Utils.GetName(evt);
                                var timestamp = Utils.GetTimestamp(evt);
                                customMessage(syncPlugin.Name, "SYNC",
                                    string.Format("[{0}] {1} {2}", timestamp, name.PadRight(25),
                                        response.GetMessageAt(i)));
                            }

                            syncPlugin.Remove(list);
                            customMessage(syncPlugin.Name, "SYNC", "Data sent : " + list.Count);
                            if (syncPlugin.Throttle > 0)
                            {
                                Thread.Sleep(syncPlugin.Throttle);
                            }
                        }
                    }
                    else
                    {
                        // nothing : commit
                        foreach (var detail in syncPlugin.ServiceJournal.Commit())
                        {
                            customMessage(syncPlugin.Name, "SYNC", detail);
                        }
                    }
                }
            }

            if (!somethingHappened)
            {
                customMessage("SYNC", "INFO", "Sending is paused");
                this._paused = true;
            }

        }

        private void testConnections()
        {
            foreach(var plugin in this.EntryManagers)
            {
                var obj = plugin as SyncPlugin;
                if (obj != null)
                {
                    var result = obj.TestConnection();
                    obj.Enabled = result;
                }
            }
        }

        private void build()
        {
            // add this as EntryManager to parser
            foreach(var parser in this.Parsers)
            {
                var entryParser = parser as IEntryParser;
                if (entryParser != null)
                {
                    entryParser.Add(this);
                }
            }
        }

        private void pluginMessage(SyncPlugin plugin, string type, string message)
        {
            this.customMessage(plugin.Name, type, message);
        }

        private void customMessage(string source, string type, string message)
        {
            if (this.SyncMessageHandler != null)
            {
                this.SyncMessageHandler(source, type, message);
            }
        }

        #region sender thread

        private void startSending()
        {
            this._sender = new Thread(sender);
            this._sender.IsBackground = true;
            this._sender.Start();
        }

        private void sender()
        {
            this._sending = true;

            while (this._sending)
            {
                if (!this._paused) { 
                    this.Send();
                } else
                {
                    Thread.Sleep(10000);
                }
            }

            Console.WriteLine("Sending thread finished");
        }

        #endregion
    }
}
