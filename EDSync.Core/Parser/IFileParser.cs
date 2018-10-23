using System;
using System.Collections.Generic;
using System.Text;

namespace EDLogWatcher.Parser
{
    /// <summary>
    /// Any file parser
    /// </summary>
    public interface IFileParser
    {
        /// <summary>
        /// If a file is accepted by this parser
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool Accept(string path);

        /// <summary>
        /// Parse af ile
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool Parse(string path);

        void Stop();
    }
}
