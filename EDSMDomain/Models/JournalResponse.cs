using System;
using System.Collections.Generic;
using System.Text;

namespace EDSMDomain.Models
{

    public class JournalResponse
    {

        // {"MessageNumber":100,"Message":"OK","Details":[{"MessageNumber":103,"Message":"Duplicate event request"}]}

        public int MessageNumber;

        public string Message;

        public JournalResponse [] Details;

        public override string ToString()
        {
            return string.Format("{0} - {1} - {2}", MessageNumber, Message, Details);
        }
    }
}
