using System;
using System.Text;
using System.Collections.Generic;
using EDLogWatcher;
using EDLogWatcher.Parser;
using EDLogWatcher.Watcher;
using EDSync.Core.Filter;
using EDUploader;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace EDSMDomain.Tests
{
    /// <summary>
    /// Description résumée pour UploaderTest
    /// </summary>
    [TestFixture()]
    public class UploaderTest
    {
        public UploaderTest()
        {
        }


        [Test]
        public void SimpleDirectoryAndParser()
        {
            const string dir = "_DIR_";

            var fileParser = this.CreateFileParser();
            var dirListener = this.CreateDirectoryListener(dir);

            dirListener.Add(fileParser);

            Assert.NotNull(dirListener.Parsers);
            Assert.AreEqual(1, dirListener.Parsers.Count);

            dirListener.Listen();
            Assert.AreEqual(dir, dirListener.Directory);

            Assert.IsNull((fileParser as DumbFileParser).LastReadFile);

            dirListener.ReadAll();

            Assert.IsNotNull((fileParser as DumbFileParser).LastReadFile);
            Assert.AreEqual(dir, (fileParser as DumbFileParser).LastReadFile);
        }


        [Test]
        public void SyncEngineTest()
        {
            const string dir = "___DIRE_______";
            // create dir listener
            DumbDirectoryListener dirListener = this.CreateDirectoryListener(dir);
            var engine = new SyncEngine(dirListener);

            Assert.NotNull(engine.DirectoryListener);

            // create a file parser
            DumbFileParser fileParser = this.CreateFileParser();
            engine.Add(fileParser);

            Assert.AreEqual(1, dirListener.Parsers.Count);

            engine.Listen();

            // test a new file modified
            const string sample = "___SAMPLE.log";
            dirListener.TestNewFile(sample);

            Assert.AreEqual(sample, fileParser.LastReadFile);

            // test an entry manager
            var entryParser = new DumbEntryProvider();
            engine.Add(entryParser);
            Assert.AreEqual(2, dirListener.Parsers.Count);

            var entryManager = new DumbEntryManager();
            engine.Add(entryManager);
            Assert.AreEqual(2, engine.Parsers.Count);
            Assert.AreEqual(1, engine.EntryManagers.Count);
            Assert.AreEqual(1, entryParser.EntryManagers.Count);

            // a second entry manager
            var entryManager2 = new DumbEntryManager();
            engine.Add(entryManager2);
            Assert.AreEqual(2, engine.Parsers.Count);
            Assert.AreEqual(2, engine.EntryManagers.Count);

            // send line
            Assert.AreEqual(0, entryManager.Count);
            Assert.AreEqual(0, entryManager2.Count);
            dirListener.TestNewFile(sample);

            // dumb sends 3 lines
            Assert.AreEqual(3, entryManager.Count);
            Assert.AreEqual(3, entryManager2.Count);
            Assert.AreEqual("003", entryManager.LastEntry);
            Assert.AreEqual("003", entryManager2.LastEntry);
        }


        #region mocks

        private DumbFileParser CreateFileParser()
        {
            return new DumbFileParser();
        }

        private DumbDirectoryListener CreateDirectoryListener(string dir)
        {
            return new DumbDirectoryListener(dir);
        }

        #region mocks class

        private class DumbFileParser : IFileParser
        {
            public string LastReadFile { get; private set; }

            public bool Accept(string path)
            {
                return true;
            }

            public virtual bool Parse(string path)
            {
                this.LastReadFile = path;
                return true;
            }

            public void Stop()
            {
            }
        }

        private class DumbDirectoryListener : IDirectoryListener
        {
            public DumbDirectoryListener(string path)
            {
                this.Directory = path;
                this.Parsers = new List<IFileParser>();
            }
            public string Directory { get; private set; }

            public IList<IFileParser> Parsers { get; private set; }

            public void Add(IFileParser parser)
            {
                this.Parsers.Add(parser);
            }

            public void Listen()
            {
            }

            public void Stop()
            {

            }

            public void ReadAll()
            {
                foreach (var p in this.Parsers)
                {
                    p.Parse(this.Directory);
                }
            }

            public void TestNewFile(string path)
            {
                foreach (var fileParser in Parsers)
                {
                    fileParser.Parse(path);
                }
            }
        }

        private class DumbEntryProvider : DumbFileParser, IEntryParser
        {
            public DumbEntryProvider() :base ()
            {
                this.EntryManagers = new List<IEntryManager>();
            }

            public IList<IEntryManager> EntryManagers { get; private set; }

            public void Add(IEntryManager manager)
            {
                this.EntryManagers.Add(manager);
            }

            public override bool Parse(string path)
            {
                var ok = base.Parse(path);

                foreach (var entryManager in EntryManagers)
                {
                    entryManager.AddEntry("001");
                    entryManager.AddEntry("002");
                    entryManager.AddEntry("003");
                }

                return true;
            }
        }

        private class DumbEntryManager : IEntryManager
        {
            public DumbEntryManager()
            {
                this.Count = 0;
            }

            public bool Enabled => true;

            public IEntryFilter EntryFilter { get; set; }

            public int Count { get; private set; }

            public string LastEntry { get; private set; }

            public bool AddEntry(string data)
            {
                this.Count++;
                this.LastEntry = data;
                return true;
            }
        }

        #endregion

        #endregion
    }
}
