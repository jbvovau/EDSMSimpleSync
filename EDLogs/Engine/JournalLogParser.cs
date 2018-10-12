using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using EDLogs.Models;
using Newtonsoft.Json;

namespace EDLogs.Engine
{
    public class JournalLogParser : IEDFileParser
    {
        public JournalLogParser(LogWatcher manager)
        {
            this.Manager = manager;
        }

        public LogWatcher Manager { get; private set; }

        public bool Accept(string path)
        {
            return path != null && path.EndsWith(".log");
        }

        public bool Parse(string path)
        {
            if (this.Accept(path))
            {
                return this.Read(path);
            }
            return false;
        }

        #region read and extract events 

        public bool Read(string path)
        {
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
                                this.AddEvent(sb.ToString());
                                sb.Length = 0;
                            }
                        } while (line != null);

                        reader.Close();
                    }
                    return true;

            }catch(Exception ex)
            {
                log4net.LogManager.GetLogger(typeof(JournalLogParser)).Error("Error when reading file :" + path, ex);
            }
            return false;
        }

        public void AddEvent(JournalEvent evt)
        {
            if (this.Manager != null)
            {
                this.Manager.Dispatch(evt);
            }
        }

        /// <summary>
        /// Add Event by data
        /// </summary>
        /// <param name="data"></param>
        private void AddEvent(string data)
        {
            if (this.Manager != null)
            {
                this.Manager.DispatchJournalLog(data);
            }
            JournalEvent e = JsonConvert.DeserializeObject<JournalEvent>(data);
            this.AddEvent(e);

        }

        #endregion
    }
}
