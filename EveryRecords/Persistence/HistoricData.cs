using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Xml.Serialization;

namespace EveryRecords.Persistence
{
    [XmlRoot("HistoricData")]
    public class HistoricData
    {
        public HistoricData()
        {
            HistoricItems = new List<HistoricItem>();
        }

        [XmlArray("HistoricItems"), XmlArrayItem("HistoricItem")]
        public List<HistoricItem> HistoricItems { get; set; }
    }
}