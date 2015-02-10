using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace EveryRecords.Persistence
{
    public class HistoricItem
    {
        [XmlAttribute("HistoricId")]
        public string HistoricId { get; set; }

        [XmlAttribute("Summary")]
        public double Summary { get; set; }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", HistoricId, Summary);
        }
    }
}
