using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDSync.Core.Filter
{
    public interface IEntryFilter
    {
        /// <summary>
        /// Is Entry accepted for sync
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool Accepted(string entry);

        /// <summary>
        /// Discard en entry
        /// </summary>
        /// <param name="entry"></param>
        void Discard(string entry);

        void Confirm();
    }
}
