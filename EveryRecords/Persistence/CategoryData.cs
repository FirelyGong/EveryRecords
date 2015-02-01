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
    [XmlRoot("CategoryData")]
    public class CategoryData
    {
        public CategoryData()
        {
            Categories = new List<CategoryItem>();
        }

        [XmlArray("Categories"), XmlArrayItem("CategoryItem")]
        public List<CategoryItem> Categories { get; set; }
    }
}