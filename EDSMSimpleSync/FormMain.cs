using EDLogWatcher;
using EDSMDomain.Services;
using EDSMSimpleSync.Utils;
using EDSMSync;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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

        // private EDUploader.UploaderEngine _uploaderEngine;

        private Thread _engineThread;

        private string _appVersion = "";

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
            DateTime.TryParse(EDConfig.Instance.Get("last_event_date"), out lastEvent);
            EDConfig.Instance.Set("last_event_date", lastEvent.ToString());

            this.tbApiKey.Text = api_key;
            this.tbCmdr.Text = name;
            this.tbDirectory.Text = journal_log;
        }

        private EDSMEngine buildEngine()
        {
            var name = EDConfig.Instance.Get("name");
            var api_key = EDConfig.Instance.Get("api_key");
            var journal_log = EDConfig.Instance.Get("journal_log");

            var engine = new EDSMEngine();
            engine.ServiceJournal = new SerivceJournal(name, api_key);
            engine.ServiceSystem = new CacheServiceSystem(new ServiceSystem(), new MemoryStorage());
            engine.Directory = journal_log;
            return engine;
        }

        private bool testEDSMConnection()
        {
            var result = this._edsmEngine.ServiceJournal.PostJournalEntry("TEST");

            if (result == null || result.msgnum != 302)
            {
                _edsmEngine_NewSyncEvent("ERROR", result.msg);
                return false;
            } else
            {
                _edsmEngine_NewSyncEvent("SYNC", "EDSM Connection is OK");
            }

            return true;
        }

        #region listening events

        private void startListen()
        {
            Thread thread = new Thread(this.threadListen);
            thread.Start();
        }

        private void stopListen()
        {
            // trash old
            if (this._edsmEngine != null)
            {
                this._edsmEngine.Dispose();
            }

            this.setStart(false);
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
            this._edsmEngine = this.buildEngine();

            // test folder
            if (!Directory.Exists(this._edsmEngine.Directory))
            {
                _edsmEngine_NewSyncEvent("ERROR", "Folder doesn't exist : " + _edsmEngine.Directory);
                return;
            }

            // test edsm connection
            if (!this.testEDSMConnection())
            {
                return;
            }

            this.setStart(true);

            // list to new sync events for displaying
            this._edsmEngine.NewSyncEvent += _edsmEngine_NewSyncEvent;

            // load last date
            _edsmEngine.LoadLastDate();

            // listen journa log directory
            _edsmEngine.Listen();
            this._edsmEngine_NewSyncEvent("APP", "Start listenning");

            // dev
            // this.startUploader();

        }

        private void _edsmEngine_NewSyncEvent(string type, string message)
        {

            this.Invoke((MethodInvoker)delegate
            {
                // juste add text for the moment

                Color color = Color.White;

                if (type == "SYNC")
                {
                    color = Color.LightGreen;
                }

                if (type == "EVENT")
                {
                    color = Color.LightBlue;
                }

                if (type == "DEBUG")
                {
                    color = Color.Gray;
                }

                if (type == "ERROR")
                {
                    color = Color.Red;
                }

                
                int start = rtbLogs.TextLength;
                rtbLogs.AppendText(message);
                rtbLogs.AppendText("\n");
                int end = rtbLogs.TextLength;

                // Textbox may transform chars, so (end-start) != text.Length
                rtbLogs.Select(start, end - start);
                {
                    rtbLogs.SelectionColor = color;
                    // could set box.SelectionBackColor, box.SelectionFont too.
                }
                rtbLogs.SelectionLength = 0; // clear
                rtbLogs.ScrollToCaret();

                // error
                if (type == "ERROR" && message != null && message.Contains("[203"))
                {
                    this.stopListen();
                }

                // clean window
                if (rtbLogs.Text.Length > 5000)
                {
                    rtbLogs.Text.Remove(0, 3000);
                }

                this.updateCurrentSystem();
            });
            return;
        }

        /// <summary>
        /// For new version
        /// </summary>
        /*
        private void startUploader()
        {
            return;
            // for new version
            this._uploaderEngine = new EDUploader.UploaderEngine();

            // Inara
            IUploader inara = new InaraUploader();
            inara.Api.CommanderName = "";
            inara.Api.ApiKey = "";
            inara.Api.FromSoftware = "EDSMSimpleSync";
            inara.Api.FromSoftwareVersion = this._appVersion;

            // edsm

            this._uploaderEngine.Add(inara);

            this._uploaderEngine.Listen(_edsmEngine.Directory);
        }
        */
        #endregion

        private void initLog()
        {
            log4net.Config.XmlConfigurator.Configure();

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;

            this.Text += " - version " + version;
        }

        #region UI events

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
                this.tbDirectory.Text = this.folderBrowserDialogJournal.SelectedPath;
                EDConfig.Instance.Set("journal_log", this.folderBrowserDialogJournal.SelectedPath);
            }
            
        }


        private void setStart(bool started)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.btnStart.Enabled = !started;
                this.btnStop.Enabled = started;

                this.tbCmdr.Enabled = !started;
                this.tbApiKey.Enabled = !started;
                this.tbDirectory.Enabled = !started;
                this.btnSelectFolder.Enabled = !started;
            });
        }

        

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.stopListen();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            EDConfig.Instance.Set("name", this.tbCmdr.Text);
            EDConfig.Instance.Set("api_key", this.tbApiKey.Text);
            EDConfig.Instance.Set("journal_log", this.tbDirectory.Text);


            this.startListen();
        }

        #endregion

        #region current system panel

        private void updateCurrentSystem()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)updateCurrentSystem);
                return;
            }

            var status = this._edsmEngine.Status;
            this.tbCurrentSystem.Text = status.System;
            this.tbCurrentStation.Text = status.Station;
        }

        #endregion

    }
}
