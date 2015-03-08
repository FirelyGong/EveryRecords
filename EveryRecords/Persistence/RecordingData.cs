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
    [XmlRoot("RecordingData")]
    public class RecordingData
    {
        public RecordingData()
        {
            Records = new List<RecordItem>();
            Summaries = new List<SummaryItem>();
        }

        [XmlArray("Records"), XmlArrayItem("RecordItem")]
        public List<RecordItem> Records
        {
            get;
            set;
        }

        [XmlArray("Summaries"), XmlArrayItem("SummaryItem")]
        public List<SummaryItem> Summaries
        {
            get;
            set;
        }

        public void Clear()
        {
            Records.Clear();
            Summaries.Clear();
        }
    }
}