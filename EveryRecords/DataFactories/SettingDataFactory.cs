using EveryRecords.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EveryRecords.DataFactories
{
    public class SettingDataFactory:DataFactory
    {
        public static readonly SettingDataFactory Instance = new SettingDataFactory();
        
        private SettingData _settingData;

        private SettingDataFactory()
        {
            DataPath = Path.Combine(BasePath, "configure.xml");
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
                _settingData = LoadData<SettingData>();
                DataLoaded = true;
            }
            else
            {
                _settingData = new SettingData();
            }
        }

        public void SaveData()
        {
            if (DataChanged)
            {
                SaveData(_settingData);
                DataChanged = false;
            }
        }

        public string AppVersion
        {
            get
            {
                return _settingData.AppVersion;
            }

            set
            {
                if (value != _settingData.AppVersion)
                {
                    DataChanged = true;
                    _settingData.AppVersion = value;
                }
            }
        }
        public bool AllowDeleteRecord
        {
            get
            {
                return _settingData.AllowDeleteRecord;
            }

            set
            {
                if (value != _settingData.AllowDeleteRecord)
                {
                    DataChanged = true;
                    _settingData.AllowDeleteRecord = value;
                }
            }
        }

        public bool AllowDeleteHistory
        {
            get
            {
                return _settingData.AllowDeleteHistory;
            }

            set
            {
                if (value != _settingData.AllowDeleteHistory)
                {
                    DataChanged = true;
                    _settingData.AllowDeleteHistory = value;
                }
            }
        }

        public double ExpensesLimit
        {
            get
            {
                return _settingData.ExpensesLimit;
            }

            set
            {
                if (value != _settingData.ExpensesLimit)
                {
                    DataChanged = true;
                    _settingData.ExpensesLimit = value;
                }
            }
        }
    }
}
