using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace EveryRecords.Persistence
{
    public class RecordItem
    {
        [XmlAttribute("Category")]
        public string Category { get; set; }

        [XmlAttribute("RecordDate")]
        public string RecordDate { get; set; }

        [XmlAttribute("Comments")]
        public string Comments { get; set; }

        [XmlAttribute("Path")]
        public string Path { get; set; }

        [XmlAttribute("Amount")]
        public double Amount { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Comments))
            {
                return string.Format(CultureInfo.InvariantCulture, "{0} - {1}:{2}", RecordDate, Category, Amount);
            }

            return string.Format(CultureInfo.InvariantCulture, "{0} - {1}({2}):{3}", RecordDate, Category, Comments, Amount);
        }
    }
}
