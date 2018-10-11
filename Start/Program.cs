using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using EDLogs.Engine;
using EDLogs.Models;
using EDSMSync;
using Newtonsoft.Json;

namespace Start
{
    class Program
    {


        static void Main(string[] args)
        {
            // get edsm sample config
            var config_edsm = GetEDSMConfig();


            #region testing and dev : EDSMSync

            var sync = new EDSMEngine();
            sync.ApiName = config_edsm.name;
            sync.ApiKey = config_edsm.api_key;

            sync.Listen(@"C:\samplelogs\");


            while (true)
            {
                System.Threading.Thread.Sleep(15000);
            }

            #endregion



            #region testing and dev : Log Engine 

            Console.WriteLine("Start EDLOGS !!");

            // define new log engine
            var engine = new LogManager();

            // test a fake listener here
            engine.NewJournalLog += Engine_NewJournalLog;

            // listen new change
            engine.ListenDirectory(@"C:\samplelogs\");

            engine.ReadAll();

            while (true)
            {
                System.Threading.Thread.Sleep(5000);
            }

            #endregion



        }

        private static void Engine_NewJournalLog(JournalEvent e)
        {
            Log(string.Format("[JournalEvent][{0}] {1}", e.Timestamp, e.EventName));
        }

        public static void Log(string message)
        {
            var line = string.Format("[{0}] {1}", DateTime.Now, message);
            Console.WriteLine((line));
        }

        private static EdsmConfig GetEDSMConfig()
        {
            var config = new EdsmConfig();

            var fileStream = new System.IO.FileStream("edsm_config.json", FileMode.Open);
            using (fileStream)
            {
                using (var reader = new StreamReader(fileStream))
                {
                    var data = reader.ReadToEnd();

                    config = JsonConvert.DeserializeObject<EdsmConfig>(data);

                }
            }

            return config;
        }


        private class EdsmConfig
        {
            public string name;
            public string api_key;
        }
    }
}
