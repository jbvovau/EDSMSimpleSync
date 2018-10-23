using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDSync.Core
{
    public static class Utils
    {

        public static string GetName(string evt)
        {
            return GetFragment("event", evt);
        }

        public static string GetTimestamp(string evt)
        {
            return GetFragment("timestamp", evt);
        }

        private static string GetFragment(string name, string line)
        {
            string result = "<not found>";
            if (name == null || line == null) return result;
            int start = line.IndexOf(name);
            if (start >= 0)
            {
                start += name.Length + 3;
                int end = line.IndexOf("\"", start + 4);
                if (end > 0)
                {
                    result = line.Substring(start , end - start);
                }
            }

            return result;
        }
    }
}
