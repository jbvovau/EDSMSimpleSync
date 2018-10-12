using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using EDLogs;
using EDLogs.Engine;
using EDLogs.Models;
using EDSMSync;
using log4net;
using log4net.Config;
using Newtonsoft.Json;

namespace Start
{
    class Program
    {

        private static ILog log = log4net.LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            configLog();

            log.Info("START");


            // get edsm sample config
            var config_edsm = GetEDSMConfig();

            EDConfig.Instance.Set("name", config_edsm.name);
            EDConfig.Instance.Set("api_key", config_edsm.api_key);


            #region testing and dev : EDSMSync

            // build a new sync engine
            var sync = new EDSMEngine();

            sync.LoadLastDate();

            // set api info
            sync.ApiName = config_edsm.name;
            sync.ApiKey = config_edsm.api_key;

            // fetch last date

            sync.Listen(@"C:\samplelogs\");
            // sync.Listen(@"C:\Users\VOVAU\Saved Games\Frontier Developments\Elite Dangerous");


            while (true)
            {
                System.Threading.Thread.Sleep(15000);
            }

            #endregion



            #region testing and dev : Log Engine 

            Console.WriteLine("Start EDLOGS !!");

            // define new log engine
            var engine = new LogWatcher();

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
            log.Info(string.Format("[JournalEvent][{0}] {1}", e.Timestamp, e.EventName));
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

        private static void configLog()
        {
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead("log4net.config"));
            var repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                       typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
        }

        private class EdsmConfig
        {
            public string name;
            public string api_key;
        }
    }
}
