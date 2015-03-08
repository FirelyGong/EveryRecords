using EveryRecords.Persistence;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace EveryRecords.DataFactories
{
    public class HistoricDataFactory : DataFactory
    {
        public const string NoHistoryList = "没有历史记录";

        public static readonly HistoricDataFactory Instance = new HistoricDataFactory();
        
        private HistoricData _historicData;

        private HistoricDataFactory()
        {
            DataPath = Path.Combine(BasePath, "historic.xml");
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
                _historicData = LoadData<HistoricData>();
                DataLoaded = true;
            }
            else
            {
                _historicData = new HistoricData();
            }
        }

        public void SaveData()
        {
            if (DataChanged)
            {
                SaveData(_historicData);
                DataChanged = false;
            }
        }

        public IList<string> GetHistoryList(int fromYear, int fromMonth)
        {
            var yearMonth = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", fromYear.ToString("0000"), fromMonth.ToString("00"));
            var result = _historicData.HistoricItems.Where(h => string.Compare(h.HistoricId, yearMonth) <= 0).Select(h => h.ToString()).ToList();
            if (result.Count == 0)
            {
                result.Add(NoHistoryList);
            }

            return result.OrderBy(h => h).ToList();
        }

        public IList<string> GetHistoryList()
        {
            return GetHistoryList(int.MaxValue, 13);
        }

        public void AddHistoric(string historicId, double amount)
        {
            DataChanged = true;
            var historic = _historicData.HistoricItems.FirstOrDefault(h => h.HistoricId == historicId);
            if (historic == null)
            {
                _historicData.HistoricItems.Add(new HistoricItem { HistoricId = historicId, Summary = amount });
            }
            else
            {
                historic.Summary = amount;
            }
        }

        public bool RemoveHistoric(string historicId)
        {
            var historic = _historicData.HistoricItems.FirstOrDefault(h => h.HistoricId == historicId);
            if (historic != null)
            {
                DataChanged = true;
                _historicData.HistoricItems.Remove(historic);
                return true;
            }

            return false;
        }
    }
}
