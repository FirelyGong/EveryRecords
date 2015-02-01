using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace EveryRecords.Persistence
{
    public class CategoryItem
    {
        [XmlAttribute("Category")]
        public string Category { get; set; }

        [XmlArray("SubCategories"), XmlArrayItem("SubCategory")]
        public List<string> SubCategories { get; set; }
    }
}
