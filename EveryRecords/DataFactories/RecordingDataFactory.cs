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
            DataLoaded = false;
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
                DataLoaded = true;
            }
        }

        public string AddRecord(IList<string> paths, DateTime recordDate, string comments, double amount)
        {
            DataChanged = true;
            DataLoaded = false;

            MakeSureRecordInCurrentMonth();
            var categories = GetRecordCategories(paths);
            UpdateSummaries(paths, recordDate, amount);

            var record = new RecordItem
                        {
                            Category = string.Join(",", categories),
                            Path = string.Join(";", paths),
                            Amount = amount,
                            Comments = comments,
                            RecordDate = recordDate.ToDateString()
                        };
            _recordingData.Records.Add(record);
            UpdateHistoricData();
            return record.ToString();
        }

        public bool DeleteRecord(string recordString)
        {
            var record = _recordingData.Records.FirstOrDefault(r => r.ToString() == recordString);
            if (record == null)
            {
                return false;
            }
            else
            {
                DataChanged = true;
                DataLoaded = false;

                var amount = 0 - record.Amount;
                string[] paths = record.Path.Split(';');
                UpdateSummaries(paths, DateTime.Parse(record.RecordDate), amount);
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
            var list = _recordingData.Summaries.Where(l => l.Parent == parent);

            foreach (var item in list)
            {
                result.Add(item.Category, item.Summary);
            }

            return result;
        }

        public IList<string> GetRecords(string category)
        {
            return new List<string>(_recordingData.Records.Where(r => r.Category.Contains(category)).OrderBy(r => r.RecordDate).Select(r => r.ToString()));
        }

        public IList<string> GetDailyRecords(string recordType, DateTime date)
        {
            return new List<string>(_recordingData.Records.Where(r => r.Path.Contains(recordType) &&  r.RecordDate==date.ToDateString()).OrderBy(r => r.RecordDate).Select(r => r.ToString()));
        }

        public IList<string> GetDailySummaries(string recordType, DateTime toDate, int days)
        {
            string end = toDate.ToDateString();
            string from = toDate.AddDays(0 - days).ToDateString();
            return new List<string>(_recordingData.Summaries.Where(r =>
            {
                DateTime dt;
                bool bln = DateTime.TryParse(r.Category, out dt);
                if (bln && (toDate - dt).Days < days && r.Parent == recordType)
                {
                    return true;
                }

                return false;
            }).OrderBy(r => r.Category).Select(r => r.ToString()));
        }

        private void AddCategorySummary(string parent, string category, double amount)
        {
            var result = _recordingData.Summaries.FirstOrDefault(l => l.Parent==parent && l.Category == category);
            if (result == null)
            {
                _recordingData.Summaries.Add(new SummaryItem { Parent=parent, Category = category, Summary = amount });
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
                DataChanged = true;
                SaveData();
                UpdateHistoricData();

                DataPath = filePath;
                DataLoaded = false;
                LoadData();
            }
        }

        private void UpdateHistoricData()
        {
            string currentFile=Path.GetFileNameWithoutExtension(DataPath);
            string historicId = currentFile.Substring((currentFile.IndexOf("_") + 1));
            HistoricDataFactory.Instance.AddHistoric(historicId, GetCategorySummary("支出"));
        }

        private string[] GetRecordCategories(IList<string> paths)
        {
            return paths.Select(p => p.Substring(p.LastIndexOf("/") + 1)).ToArray();
        }

        private void UpdateSummaries(IList<string> paths, DateTime recordDate, double amount)
        {
            var types = new[] { RecordTypes.Payment.ToLabel(), RecordTypes.Income.ToLabel() };
            string recordType = "";
            IList<string> lst = new List<string>();
            foreach (var path in paths)
            {
                var arr = path.Split('/').ToList();

                var parent = "";
                foreach (var node in arr)
                {
                    if (types.Contains(node))
                    {
                        recordType = node;
                    }

                    if (!lst.Contains(node))
                    {
                        lst.Add(node);
                        AddCategorySummary(parent, node, amount);
                        if (string.IsNullOrEmpty(parent))
                        {
                            parent = node;
                        }
                        else
                        {
                            parent += "/" + node;
                        }
                    }
                }
            }

            AddCategorySummary(recordType + "/日期", recordDate.ToDateString(), amount);
        }
    }
}
