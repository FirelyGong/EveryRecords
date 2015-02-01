using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace EveryRecords
{
    public class DataFactory
    {
        protected bool DataChanged;

        protected string BasePath;

        protected virtual string DataPath { get; set; }

        public DataFactory()
        {
            BasePath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, "Android/data/com.ziwish.EveryRecord");
        }

        protected T LoadData<T>()
        {
            try
            {
                if (!Directory.Exists(BasePath))
                {
                    Directory.CreateDirectory(BasePath);
                }

                if (!File.Exists(DataPath))
                {
                    return default(T);
                }

                XmlSerializer xs = new XmlSerializer(typeof(T));
                using (var fs = new FileStream(DataPath, FileMode.Open, FileAccess.Read))
                {
                    var data = (T)xs.Deserialize(fs);
                    return data;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        protected void SaveData<T>(T data)
        {
            try
            {
                if (!Directory.Exists(BasePath))
                {
                    Directory.CreateDirectory(BasePath);
                }

                if (File.Exists(DataPath))
                {
                    File.Delete(DataPath);
                }

                XmlSerializer xs = new XmlSerializer(typeof(T));
                using (var fs = new FileStream(DataPath, FileMode.Create, FileAccess.Write))
                {
                    xs.Serialize(fs, data);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
