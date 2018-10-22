using EDLogWatcher.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDLogWatcher.Watcher;

namespace EDUploader
{
    /// <summary>
    /// 
    /// </summary>
    public class SyncEngine : IDisposable
    {
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

        public void Dispose()
        {

        }

        /// <summary>
        /// Add a file parser
        /// </summary>
        /// <param name="parser"></param>
        public void Add(IFileParser parser)
        {
            this.DirectoryListener.Add(parser);
            this.build();
        }

        public void Add(IEntryManager manager)
        {
            this.EntryManagers.Add(manager);
            this.build();
        }

        public void Listen()
        {
            this.build();

            this.DirectoryListener.Listen();
            this.DirectoryListener.ReadAll();
        }

        public void Stop()
        {

        }

        private void build()
        {
            foreach (var parser in this.Parsers)
            {
                IEntryParser entryParser = parser as IEntryParser;
                if (entryParser != null)
                {
                    foreach (var entryManager in this.EntryManagers)
                    {
                        if (!entryParser.EntryManagers.Contains(entryManager))
                        {
                            entryParser.Add(entryManager);
                        }
                    }
                }
            }
        }

    }
}
