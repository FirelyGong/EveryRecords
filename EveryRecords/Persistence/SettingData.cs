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
    [XmlRoot("SettingData")]
    public class SettingData
    {
        [XmlElement("AppVersion")]
        public string AppVersion { get; set; }

        [XmlElement("AllowDeleteRecord")]
        public bool AllowDeleteRecord { get; set; }

        [XmlElement("AllowDeleteHistory")]
        public bool AllowDeleteHistory { get; set; }

        [XmlElement("ExpensesLimit")]
        public double ExpensesLimit { get; set; }
    }
}