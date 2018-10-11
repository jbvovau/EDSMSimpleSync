using System;
using System.Collections.Generic;
using System.Text;

namespace EDLogs.Engine
{
    /// <summary>
    /// Any file parser
    /// </summary>
    public interface IEDFileParser
    {
        bool Accept(string path);

        void Parse(string path);
    }
}
