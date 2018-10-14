using EDSMDomain.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace EDLogWatcher.Engine
{
    public class JournalLogParser : IEDFileParser
    {
        private bool _run;

        public JournalLogParser(LogWatcher manager)
        {
            Manager = manager;
        }

        public LogWatcher Manager { get; private set; }

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

        public void AddEvent(JournalEvent evt)
        {
            if (Manager != null)
            {
                Manager.Dispatch(evt);
            }
        }

        /// <summary>
        /// Add Event by data
        /// </summary>
        /// <param name="data"></param>
        private void AddEvent(string data)
        {
            if (Manager != null)
            {
                Manager.DispatchJournalLog(data);
            }
            JournalEvent e = JsonConvert.DeserializeObject<JournalEvent>(data);
            AddEvent(e);

        }

        #endregion
    }
}
