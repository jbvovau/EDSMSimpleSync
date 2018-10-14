using System;
using System.Collections.Generic;
using System.Text;

namespace EDSMDomain.Models
{
    public class Journal
    {
        public IDictionary<string,JournalEvent> Events { get; set; }
    }
}
