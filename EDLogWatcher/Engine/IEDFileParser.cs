using System;
using System.Collections.Generic;
using System.Text;

namespace EDLogWatcher.Engine
{
    /// <summary>
    /// Any file parser
    /// </summary>
    public interface IEDFileParser
    {
        bool Accept(string path);

        bool Parse(string path);
    }
}
