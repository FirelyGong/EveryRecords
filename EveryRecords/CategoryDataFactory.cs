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
using EveryRecords.Persistence;
using System.IO;

namespace EveryRecords
{
    public class CategoryDataFactory:DataFactory
    {
        public static readonly CategoryDataFactory Instance = new CategoryDataFactory();

        public const string RootCategory = "分类";

        //private Dictionary<string, IList<string>> _datas;

        private CategoryData _categoryData;
        
        private CategoryDataFactory()
        {
            _categoryData=new CategoryData();
            DataPath = Path.Combine(BasePath, "config.xml");
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
                _categoryData = LoadData<CategoryData>();
            }
            else
            {
                InitializeDatas();
            }
        }

        public void SaveData()
        {
            if (DataChanged)
            {
                SaveData(_categoryData);
                DataChanged = false;
            }
        }

        public IList<string> GetCategories()
        {
            return GetSubCategories(RootCategory);
        }

        public IList<string> GetSubCategories(string category)
        {
            var reslut = _categoryData.Categories.Where(l => l.Category == category);
            if(reslut.Count()==0)
            {
                return new string[]{};
            }

            return new List<string>(reslut.First().SubCategories);
        }

        public bool AddCategory(string parent, string categoryName)
        {
            DataChanged = true;
            var reslut = _categoryData.Categories.Where(l => l.Category == parent);
            if (reslut.Count() == 0)
            {
                _categoryData.Categories.Add(new CategoryItem { Category = parent, SubCategories = new List<string>(new string[]{categoryName}) });
            }
            else
            {
                reslut.First().SubCategories.Add(categoryName);
            }

            return true;
        }

        public void RemoveCategory(string parent, string categoryName)
        {
            DataChanged = true;
            var reslut = _categoryData.Categories.Where(l => l.Category == RootCategory);
            if (reslut.Count() > 0)
            {
                reslut.First().SubCategories.Remove(categoryName);
            }
        }

        private void InitializeDatas()
        {
            _categoryData.Categories.Add(new CategoryItem { Category = RootCategory, SubCategories = new List<string>(new string[] { "成员", "收入支出" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "成员", SubCategories =  new List<string>(new string[] { "FirelyGong", "Qihui" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "收入支出", SubCategories = new List<string>(new string[] { "支出", "收入" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "支出", SubCategories = new List<string>( new string[] { "伙食", "服装", "交通","通迅", "其它" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "收入", SubCategories = new List<string>( new string[] { "工资"}) });
            _categoryData.Categories.Add(new CategoryItem { Category = "伙食", SubCategories = new List<string>( new string[] { "超市", "买菜","下馆子"}) });
            _categoryData.Categories.Add(new CategoryItem { Category = "服装", SubCategories = new List<string>( new string[] { "衣服", "鞋子" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "通迅", SubCategories = new List<string>( new string[] { "电话费", "网费","电视","手机"}) });
            _categoryData.Categories.Add(new CategoryItem { Category = "交通", SubCategories = new List<string>( new string[] { "加油", "汽车保险","汽车配件" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "其它", SubCategories = new List<string>(new string[] { "电费", "水费" }) });
        }
    }
}