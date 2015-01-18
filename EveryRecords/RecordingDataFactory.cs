using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveryRecords
{
    public class RecordingDataFactory
    {
        public static readonly RecordingDataFactory Instance = new RecordingDataFactory();

        private Dictionary<string, int> _summaries;

        private IList<RecordItem> _records;

        private RecordingDataFactory()
        {
            _summaries = new Dictionary<string, int>();
            _records = new List<RecordItem>();
        }

        public void AddRecord(string path, string comments, int amount)
        {
            string[] categories = path.Split('/');
            if (string.IsNullOrEmpty(categories[0]))
            {
                categories[0] = CategoryDataFactory.RootCategory;
            }

            foreach (var item in categories)
            {
                AddCategorySummary(item, amount);
            }

            _records.Add(new RecordItem
            {
                Category = categories[categories.Length - 1],
                Path = path,
                Amount = amount,
                Comments = comments,
                RecordTime = DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss")
            });
        }

        public int GetCategorySummary(string category)
        {
            if(_summaries.ContainsKey(category))
            {
                return _summaries[category];
            }

            return 0;
        }

        public Dictionary<string, int> GetSubCategoriesSummary(string parent)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            var categories = CategoryDataFactory.Instance.GetSubCategories(parent);
            foreach (var item in categories)
            {
                result.Add(item, GetCategorySummary(item));
            }

            return result;
        }

        public IList<string> GetRecords(string category)
        {
            return new List<string>(_records.Where(r => r.Category == category).Select(r => r.ToString()));
        }

        private void AddCategorySummary(string category, int amount)
        {
            if (!_summaries.ContainsKey(category))
            {
                _summaries.Add(category, 0);
            }

            var total = _summaries[category];
            _summaries[category] = total + amount;
        }
    }
}
