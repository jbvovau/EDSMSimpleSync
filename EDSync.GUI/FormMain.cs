using EDSMSimpleSync.Utils;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using EDLogWatcher.Filter;
using EDLogWatcher.Watcher;
using EDSMDomain.Services;
using EDSync.Core;
using EDSync.Core.Storage;
using EDSync.EDSM;
using EDSync.Inara;
using EDSync.Inara.Api;
using EDSync.Services;
using EDUploader;

namespace EDSMSimpleSync
{
    public partial class FormMain : Form
    {
        // the main sync engine
        private SyncEngine _syncEngine;

        // parser for EDSM
        private EDSMEngine _edsmEngine;

        // private EDUploader.UploaderEngine _uploaderEngine;

        private Thread _engineThread;

        private string _appVersion = "";

        private const string CONFIG_FILE = "edconfig.json";

        private IStorage _storage;

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
            // version
            this._appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            // define storage
            this._storage = new FileStorage(CONFIG_FILE);
            EDConfig.Instance.Storage = this._storage;


            var journal_log = EDConfig.Instance.Get("journal_log");

            if (journal_log == null)
            {
                journal_log = @"C:\Users\" + Environment.UserName
                    + @"\Saved Games\Frontier Developments\Elite Dangerous";
                EDConfig.Instance.Set("journal_log", journal_log);
            }

            // get edsm sample config
            var customConfig = new CustomConfig(_storage, "edsm");
            var configInara = new CustomConfig(_storage, "inara");

            this.tbApiKey.Text = customConfig.ApiKey;
            this.tbCmdr.Text = customConfig.CommanderName;
            this.tbInaraCmdr.Text = configInara.CommanderName;
            this.tbInaraApiKey.Text = configInara.ApiKey;

            this.tbDirectory.Text = journal_log;
        }

        /// <summary>
        /// Build new sync engine
        /// </summary>
        /// <returns></returns>
        private SyncEngine buildEngine()
        {
            // fetch config in UI
            this.fetchConfiguration();

            var journal_log = EDConfig.Instance.Get("journal_log");

            // build journal log watcher
            var watcher = new JournalLogWatcher(journal_log);

            // build sync engine with given log watcher
            var engine = new SyncEngine(watcher);

            // listen EDSM
            engine.Add(this.BuildEDSMJournal());

            // listen Inara
            engine.Add(this.BuildInaraEngine());

            return engine;
        }

        #region listening Details

        private void startListen()
        {
            Thread thread = new Thread(this.threadListen);
            thread.Start();
        }

        private void stopListen()
        {
            // trash old
            if (this._syncEngine != null)
            {
                this._syncEngine.Dispose();
            }

            this.setStart(false);
        }

        /// <summary>
        /// Start sync
        /// </summary>
        private void threadListen()
        {
            // trash old
            if (this._syncEngine != null)
            {
                this._syncEngine.Dispose();
            }

            // build a new sync engine
            this._syncEngine = this.buildEngine();

            // listen events
            this._syncEngine.SyncMessageHandler += syncMessage;

            // test folder
            if (!Directory.Exists(this._syncEngine.DirectoryListener.Directory))
            {
                _edsmEngine_NewSyncEvent("ERROR", "Folder doesn't exist : " + _syncEngine.DirectoryListener.Directory);
                return;
            }

            this.setStart(true);

            // listen journa log directory
            _syncEngine.Listen();

            this._edsmEngine_NewSyncEvent("APP", "Start listenning");

            // dev
            // this.startUploader();

        }

        private void syncMessage(string source, string type, string message)
        {
            _edsmEngine_NewSyncEvent(type, "[" + source + "] " + message);
        }

        private void _edsmEngine_NewSyncEvent(string type, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                message = "<EMPTY>";
            }

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


        #endregion

        private void initLog()
        {
            log4net.Config.XmlConfigurator.Configure();

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;

            this.Text += " - version " + version;
        }

        private void fetchConfiguration()
        {
            var customConfig = new CustomConfig(_storage, "edsm");

            customConfig.CommanderName = this.tbCmdr.Text;
            customConfig.ApiKey = this.tbApiKey.Text;

            var configInara = new CustomConfig(_storage, "inara");
            configInara.CommanderName = this.tbInaraCmdr.Text;
            configInara.ApiKey = this.tbInaraApiKey.Text;

            EDConfig.Instance.Set("journal_log", this.tbDirectory.Text);

            customConfig.Storage.Save();
        }

        #region UI Details

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
                this.tbInaraCmdr.Enabled = !started;
                this.tbInaraApiKey.Enabled = !started;
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

            this.fetchConfiguration();

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

        #region configure plugins

        private EDSMEngine BuildEDSMJournal()
        {
            var filter = new DateEntryFilter(_storage, "edsm");

            // add entry manager for EDSM
            this._edsmEngine = new EDSMEngine(filter);
            _edsmEngine.Throttle = 1000;
            _edsmEngine.MaxPerBatch = 40;

            // EDSM config
            var customConfig = new CustomConfig(_storage, "edsm");

            // api
            var api = new ApiEDSM();
            api.ApiKey = customConfig.ApiKey;
            api.CommanderName = customConfig.CommanderName;
            api.FromSoftwareVersion = _appVersion;
            api.FromSoftware = "EDSimpleSync";

            _edsmEngine.ServiceJournal = new SerivceJournal(api);
            // _edsmEngine.ServiceJournal = new VoidServiceJournal();
            _edsmEngine.ServiceSystem = new CacheServiceSystem(new ServiceSystem(), new MemoryStorage());

            return this._edsmEngine;
        }

        private SyncPlugin BuildInaraEngine()
        {
            // define inara filter
            var filter = new DateEntryFilter(_storage, "inara");

            var customConfig = new CustomConfig(_storage, "inara");

            // inara API
            var api = new ApiInara();
            api.ApiKey = customConfig.ApiKey;
            api.CommanderName = customConfig.CommanderName;
            api.FromSoftwareVersion = _appVersion;
            api.FromSoftware = "EDSimpleSync";

            // define inara service
            IServiceJournal service = new ServiceInara(api);

            // debug
            // service = new VoidServiceJournal();

            var inara = new SyncPlugin(filter, "Inara");
            inara.ServiceJournal = service;
            inara.Throttle = 0;
            inara.MaxPerBatch = 200;

            return inara;
        }

        #endregion

    }
}
