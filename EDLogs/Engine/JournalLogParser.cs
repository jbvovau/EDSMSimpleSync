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
        public JournalLogParser(LogManager manager)
        {
            this.Manager = manager;
        }

        public LogManager Manager { get; private set; }

        public bool Accept(string path)
        {
            return path != null && path.EndsWith(".log");
        }

        public void Parse(string path)
        {
            if (this.Accept(path))
            {
                this.Read(path);
            }
        }

        #region read and extract events 

        public void Read(string path)
        {
            var fileStream = new System.IO.FileStream(path, FileMode.Open);
            using (fileStream)
            {
                using (var reader = new StreamReader(fileStream))
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
                    fileStream.Close();

                }
            }
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
            try
            {
                JournalEvent e = JsonConvert.DeserializeObject<JournalEvent>(data);
                this.AddEvent(e);
            }
            catch (Exception ex)
            {
                Program.Log("[Exception] " + ex.Message);
            }
        }

        #endregion
    }
}
