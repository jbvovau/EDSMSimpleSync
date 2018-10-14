using EDLogWatcher.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDUploader
{
    public class UploaderEngine
    {
        private LogWatcher _watcher;

        public UploaderEngine()
        {
            this.Uploaders = new List<IUploader>();
            this._watcher = new LogWatcher();
            this._watcher.NewJournalLogEntry += _watcher_NewJournalLogEntry;
        }

        public IList<IUploader> Uploaders { get; private set; }

        public string Directory { get; set; }

        public void Add(IUploader uploader)
        {
            this.Uploaders.Add(uploader);
        }

        public void Listen(string path)
        {
            this.Directory = path;
            this._watcher.ListenDirectory(path);
            this._watcher.ReadAll();
        }

        public void Stop()
        {

        }


        private void _watcher_NewJournalLogEntry(string line)
        {
            foreach(var uploader in this.Uploaders)
            {
                uploader.AddEntry(line);
            }
        }

    }
}
