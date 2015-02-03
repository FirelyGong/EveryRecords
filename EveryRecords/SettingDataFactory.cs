﻿using EveryRecords.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EveryRecords
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
            if (File.Exists(DataPath))
            {
                _settingData = LoadData<SettingData>();
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
    }
}
