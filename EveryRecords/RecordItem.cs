using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace EveryRecords
{
    public class RecordItem
    {
        public string Category { get; set; }

        public string RecordTime { get; set; }

        public string Comments { get; set; }

        public string Path { get; set; }

        public int Amount { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Comments))
            {
                return string.Format(CultureInfo.InvariantCulture, "{0} - {1}:{2}", RecordTime.Split('-')[0], Path, Amount);
            }

            return string.Format(CultureInfo.InvariantCulture, "{0} - {1}:{2}({3})", RecordTime.Split('-')[0], Path, Amount, Comments);
        }
    }
}
