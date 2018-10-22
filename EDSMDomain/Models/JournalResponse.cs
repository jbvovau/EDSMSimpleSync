using System;
using System.Collections.Generic;
using System.Text;

namespace EDSMDomain.Models
{

    public class JournalResponse
    {

        // {"MessageNumber":100,"Message":"OK","Details":[{"MessageNumber":103,"Message":"Duplicate event request"}]}

        public JournalResponse()
        {
            this.Details = new List<JournalResponse>();
        }

        public int MessageNumber;

        public string Message;

        public List<JournalResponse> Details;

        public string GetMessageAt(int i)
        {
            if (this.Details != null && this.Details.Count > i)
            {
                return string.Format("[{0}] {1}", this.Details[i].MessageNumber, this.Details[i].Message);
            }
            return "";
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} - {2}", MessageNumber, Message, Details);
        }
    }
}
