using EDLogWatcher;
using EDSMSync;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EDSMSimpleSync
{
    public partial class FormMain : Form
    {
        private EDSMEngine _edsmEngine;

        private Thread _engineThread;

        public FormMain()
        {
            InitializeComponent();

            this.initLog();
            this.fillConfig();
            this._engineThread = new Thread(this.startListen);
            this._engineThread.IsBackground = true;
            this._engineThread.Start();

        }


        private void fillConfig()
        {
            // get edsm sample config
            var name = EDConfig.Instance.Get("name");
            var api_key = EDConfig.Instance.Get("api_key");
            var journal_log = EDConfig.Instance.Get("journal_log");

            if (journal_log == null)
            {
                journal_log = @"C:\Users\" + Environment.UserName
                    + @"\Saved Games\Frontier Developments\Elite Dangerous";
                EDConfig.Instance.Set("journal_log", journal_log);
            }

            // date
            DateTime lastEvent;
            if (!DateTime.TryParse(EDConfig.Instance.Get("last_event_date"), out lastEvent)){
                // can't get a date ? 
                lastEvent = DateTime.Now.Subtract(TimeSpan.FromDays(1));
                EDConfig.Instance.Set("last_event_date", lastEvent.ToString());
            }

            this.tbApiKey.Text = api_key;
            this.tbCmdr.Text = name;
            this.tbDirectory.Text = journal_log;
        }

        private void startListen()
        {
            Thread thread = new Thread(this.threadListen);
            thread.Start();
        }

        /// <summary>
        /// Start sync
        /// </summary>
        private void threadListen()
        {
            // trash old
            if (this._edsmEngine != null)
            {
                this._edsmEngine.Dispose();
            }

            // build a new sync engine
            this._edsmEngine = new EDSMEngine();

            // list to new sync events for displaying
            this._edsmEngine.NewSyncEvent += _edsmEngine_NewSyncEvent;

            // load last date
            _edsmEngine.LoadLastDate();

            // set api info
            _edsmEngine.ApiName = EDConfig.Instance.Get("name");
            _edsmEngine.ApiKey = EDConfig.Instance.Get("api_key");

            // listen journa log directory
            _edsmEngine.Listen(EDConfig.Instance.Get("journal_log"));
            this._edsmEngine_NewSyncEvent("APP", "Start listenning");
        }

        private void _edsmEngine_NewSyncEvent(string type, string message)
        {

            this.Invoke((MethodInvoker)delegate
            {
                // juste add text for the moment

                Color color = Color.White;

                if (type == "SYNC")
                {
                    color = Color.Green;
                }

                if (type == "EVENT")
                {
                    color = Color.LightBlue;
                }

                if (type == "DEBUG")
                {
                    color = Color.Gray;
                }

                rtbLogs.AppendText("\n");
                int start = rtbLogs.TextLength;
                rtbLogs.AppendText(message);
                int end = rtbLogs.TextLength;

                // Textbox may transform chars, so (end-start) != text.Length
                rtbLogs.Select(start, end - start);
                {
                    rtbLogs.SelectionColor = color;
                    // could set box.SelectionBackColor, box.SelectionFont too.
                }
                rtbLogs.SelectionLength = 0; // clear
            });
            return;
        }

        private void initLog()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            var path = EDConfig.Instance.Get("journal_log");
            if (Directory.Exists(path))
            {
                this.folderBrowserDialogJournal.SelectedPath = path;
            }
            DialogResult result = this.folderBrowserDialogJournal.ShowDialog();
            if (result == DialogResult.OK)
            {
                EDConfig.Instance.Set("journal_log", this.folderBrowserDialogJournal.SelectedPath);
                this.startListen();
            }
            
        }

    }
}
