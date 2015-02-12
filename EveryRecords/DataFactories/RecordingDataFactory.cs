using EveryRecords.Persistence;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace EveryRecords.DataFactories
{
    public class RecordingDataFactory:DataFactory
    {
        public const string RecordFilePrefix = "recordings_";

        public static readonly RecordingDataFactory Instance = new RecordingDataFactory();

        public static RecordingDataFactory CreateHistoryData(int year, int month)
        {
            return new RecordingDataFactory(year, month);
        }

        private RecordingData _recordingData;

        private RecordingDataFactory()
            : this(DateTime.Now.Year, DateTime.Now.Month)
        {
        }

        private RecordingDataFactory(int year, int month)
        {
            _recordingData = new RecordingData();
            DataPath = ConstructDataPath(year, month);
        }

        protected override string DataPath
        {
            get;
            set;
        }

        public void LoadData()
        {
            if (DataLoaded)
            {
                return;
            }

            if (File.Exists(DataPath))
            {
                _recordingData = LoadData<RecordingData>();
                DataLoaded = true;
            }
            else
            {
                _recordingData = new RecordingData();
            }
        }

        public void SaveData()
        {
            if (DataChanged)
            {
                SaveData(_recordingData);
                DataChanged = false;
            }
        }

        public string AddRecord(IList<string> paths, string comments, double amount)
        {
            return AddRecord(paths, comments, amount, true);
        }

        public string AddRecord(IList<string> paths, string comments, double amount, bool includeInTotal)
        {
            DataChanged = true;

            MakeSureRecordInCurrentMonth();
            IList<string> categories = new List<string>();
            IList<string> lst = new List<string>();
            foreach (var path in paths)
            {
                var arr = path.Split('/').ToList();
                if (!includeInTotal)
                {
                    arr.Remove(CategoryDataFactory.RootCategory);
                }

                foreach (var node in arr)
                {
                    if (!lst.Contains(node))
                    {
                        lst.Add(node);
                        AddCategorySummary(node, amount);
                    }
                }

                categories.Add(arr[arr.Count - 1]);
            }

            var record = new RecordItem
                        {
                            Category = string.Join(",", categories),
                            Path = string.Join(";", paths),
                            Amount = amount,
                            Comments = comments,
                            RecordTime = DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss")
                        };
            _recordingData.Records.Add(record);
            UpdateHistoricData();
            return record.ToString();
        }

        public bool DeleteRecord(string recordString)
        {
            DataChanged = true;
            var record = _recordingData.Records.FirstOrDefault(r => r.ToString() == recordString);
            if (record == null)
            {
                return false;
            }
            else
            {
                var amount = 0 - record.Amount;
                string[] paths = record.Path.Split(';');
                IList<string> lst = new List<string>();
                foreach (var path in paths)
                {
                    var arr = path.Split('/');
                    foreach (var node in arr)
                    {
                        if (!lst.Contains(node))
                        {
                            lst.Add(node);
                            AddCategorySummary(node, amount);
                        }
                    }
                }
                UpdateHistoricData();
                return _recordingData.Records.Remove(record);
            }
        }

        public double GetCategorySummary(string category)
        {
            var result = _recordingData.Summaries.FirstOrDefault(l => l.Category == category);
            if(result!=null)
            {
                return result.Summary;
            }

            return 0;
        }

        public Dictionary<string, double> GetSubCategoriesSummary(string parent)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();
            var categories = CategoryDataFactory.Instance.GetSubCategories(parent);
            foreach (var item in categories)
            {
                result.Add(item, GetCategorySummary(item));
            }

            return result;
        }

        public IList<string> GetRecords(string category)
        {
            return new List<string>(_recordingData.Records.Where(r => r.Category.Contains(category)).Select(r => r.ToString()));
        }

        private void AddCategorySummary(string category, double amount)
        {
            var result = _recordingData.Summaries.FirstOrDefault(l => l.Category == category);
            if (result == null)
            {
                _recordingData.Summaries.Add(new SummaryItem { Category = category, Summary = amount });
            }
            else
            {
                result.Summary += amount;
            }
        }

        private string ConstructDataPath(int year, int month)
        {
            var fileName = string.Format(CultureInfo.InvariantCulture, "{0}{1}-{2}.xml",RecordFilePrefix, year.ToString("0000"), month.ToString("00"));
            return Path.Combine(BasePath, fileName);
        }
         
        private void MakeSureRecordInCurrentMonth()
        {
            var filePath = ConstructDataPath(DateTime.Now.Year, DateTime.Now.Month);
            if (filePath != DataPath)
            {
                SaveData();
                UpdateHistoricData();
                DataPath = filePath;
                LoadData();
            }
        }

        private void UpdateHistoricData()
        {
            string currentFile=Path.GetFileNameWithoutExtension(DataPath);
            string historicId = currentFile.Substring((currentFile.IndexOf("_") + 1));
            HistoricDataFactory.Instance.AddHistoric(historicId, GetCategorySummary(CategoryDataFactory.RootCategory));
        }
    }
}
