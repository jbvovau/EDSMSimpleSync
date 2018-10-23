using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EDLogWatcher.Parser
{
    /// <summary>
    /// Parses Journal file and sends new event logs when read
    /// </summary>
    public class JournalLogParser : IEntryParser
    {
        private bool _run;

        public JournalLogParser()
        {
            this.EntryManagers = new List<IEntryManager>();
        }

        public IList<IEntryManager> EntryManagers { get; private set; }

        public bool Accept(string path)
        {
            return path != null && path.EndsWith(".log");
        }

        public bool Parse(string path)
        {
            if (Accept(path))
            {
                return Read(path);
            }
            return false;
        }

        public void Stop()
        {
            _run = false;
        }

        #region read and extract events 

        public bool Read(string path)
        {
            _run = true;

            try
            {
                var o = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, FileOptions.RandomAccess);

                using (var reader = new StreamReader(o))
                {

                    StringBuilder sb = new StringBuilder();

                    string line = null;
                    do
                    {
                        line = reader.ReadLine();

                        sb.Append(line);

                        // check current builder
                        if (sb.ToString().Trim().EndsWith("}"))
                        {
                            AddEvent(sb.ToString());
                            sb.Length = 0;
                        }
                    } while (line != null && _run);

                    reader.Close();
                }
                return true;

            }
            catch (Exception ex)
            {
                log4net.LogManager.GetLogger(typeof(JournalLogParser)).Error("Error when reading file :" + path, ex);
            }
            return false;
        }


        /// <summary>
        /// Add Event by data
        /// </summary>
        /// <param name="data"></param>
        private void AddEvent(string data)
        {
            foreach (var entryManager in EntryManagers)
            {
                entryManager.AddEntry(data);
            }
        }

        public void Add(IEntryManager manager)
        {
            this.EntryManagers.Add(manager);
        }

        #endregion
    }
}
