using EDLogWatcher.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDLogWatcher.Watcher;
using EDSync.Core.Filter;
using EDSync.Core.Parser;
using System.Threading;

namespace EDUploader
{
    /// <summary>
    /// 
    /// </summary>
    public class SyncEngine : IEntryManager
    {

        private bool _sending;
        private Thread _sender;

        public SyncEngine(IDirectoryListener directoryListener)
        {
            this.DirectoryListener = directoryListener;
            this.EntryManagers = new List<IEntryManager>();
        }

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

        public void Add(IEntryManager manager)
        {
            this.EntryManagers.Add(manager);
        }

        public void Listen()
        {
            this.build();

            this.DirectoryListener.Listen();
            this.DirectoryListener.ReadAll();

            this.startSending();
        }

        public void Stop()
        {
            this.DirectoryListener.Stop();
            this._sending = false;
        }

        public void AddEntry(string data)
        {
            foreach(var manager in this.EntryManagers)
            {
                manager.AddEntry(data);
            }
        }

        private void Send(int count)
        {
            foreach(var em in this.EntryManagers)
            {
                var set = em as SyncPlugin;
                if (set != null)
                {
                    // check last activity
                    var span = DateTime.Now.Subtract(set.LastActivity);
                    if (span < TimeSpan.FromSeconds(25)) continue;

                    var list = set.Next(count);
                    var response = set.ServiceJournal.PostJournalEntry(list);
                    if (response.MessageNumber == 100)
                    {
                        set.Remove(list);
                    }
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
                this.Send(40);

                Thread.Sleep(10);
            }

            Console.WriteLine("Sending thread finished");
        }

        #endregion
    }
}
