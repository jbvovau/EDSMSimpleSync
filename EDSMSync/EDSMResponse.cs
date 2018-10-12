using System;
using System.Collections.Generic;
using System.Text;

namespace EDSMSync
{
    public class EDSMResponse
    {

        // {"msgnum":100,"msg":"OK","events":[{"msgnum":103,"msg":"Duplicate event request"}]}

        public int msgnum;

        public string msg;

        public EDSMResponse [] events;

        public override string ToString()
        {
            return string.Format("{0} - {1} - {2}", msgnum, msg, events);
        }
    }
}
