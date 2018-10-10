using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EDLogs.Engine
{
    public class LogManager
    {
        public string Directory { get; set; }

        public void ListenDirectory(string path)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.log";
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            //Copies file to another directory.
            Program.Log("File : " + e.ChangeType +" - " + e.FullPath);
        }

    }
}
