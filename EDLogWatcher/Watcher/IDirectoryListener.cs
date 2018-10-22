using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDLogWatcher.Parser;

namespace EDLogWatcher.Watcher
{
    /// <summary>
    /// Listen a directory and send file modified to 
    /// </summary>
    public interface IDirectoryListener
    {
        /// <summary>
        /// Journal Directory listened
        /// </summary>
        /// 
        string Directory { get; }

        /// <summary>
        /// All file parsers in the listener
        /// </summary>
        IList<IFileParser> Parsers { get; }

        /// <summary>
        /// Listen a specific directory
        /// </summary>
        void Listen();

        /// <summary>
        /// Mark all the files as "modified" to force the parsing
        /// </summary>
        void ReadAll();

        /// <summary>
        /// Add a file parser
        /// </summary>
        /// <param name="parser"></param>
        void Add(IFileParser parser);
    }
}
