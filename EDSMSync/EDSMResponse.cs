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
    }
}
