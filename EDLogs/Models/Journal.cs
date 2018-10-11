using System;
using System.Collections.Generic;
using System.Text;

namespace EDLogs.Models
{
    public class Journal
    {
        public IDictionary<string,JournalEvent> Events { get; set; }
    }
}
