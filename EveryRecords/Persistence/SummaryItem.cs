using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace EveryRecords.Persistence
{
    public class SummaryItem
    {
        [XmlAttribute("Category")]
        public string Category { get; set; }

        [XmlAttribute("Summary")]
        public double Summary { get; set; }
    }
}
