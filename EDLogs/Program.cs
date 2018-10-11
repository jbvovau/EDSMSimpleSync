using System;
using EDLogs.Engine;

namespace EDLogs
{
    /// <summary>
    /// Log parser for Elite Dangerous
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start EDLOGS !!");

            // define new log engine
            var engine = new LogManager();

            // test a fake listener
            engine.NewJournalLog += Engine_NewJournalLog;

            engine.ListenDirectory(@"C:\samplelogs\");

            while (true)
            {
                System.Threading.Thread.Sleep(5000);
            }

          
        }

        private static void Engine_NewJournalLog(Models.JournalEvent e)
        {
            Log(string.Format("[JournalEvent][{0}] {1}", e.Timestamp, e.EventName));
        }

        public static void Log(string message)
        {
            var line = string.Format("[{0}] {1}", DateTime.Now, message);
            Console.WriteLine((line));
        }
    }
}
