﻿using EDSMDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDSMDomain.Services
{
    public interface IServiceJournal
    {
        IList<string> GetDiscardedEvents();

        EDSMResponse PostJournalEntry(string data);
    }
}