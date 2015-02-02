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

        public const string RootCategory = "总计";
        
        private CategoryData _categoryData;
        
        private CategoryDataFactory()
        {
            _categoryData=new CategoryData();
            DataPath = Path.Combine(BasePath, "category.xml");
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
            var reslut = _categoryData.Categories.Where(l => l.Category == parent);
            if (reslut.Count() > 0)
            {
                reslut.First().SubCategories.Remove(categoryName);
            }
        }

        private void InitializeDatas()
        {
            _categoryData.Categories.Add(new CategoryItem { Category = RootCategory, SubCategories = new List<string>(new string[] { "成员", "收入支出" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "成员", SubCategories = new List<string>(new string[] { "FirelyGong", "Qihui" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "收入支出", SubCategories = new List<string>(new string[] { "支出", "收入" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "支出", SubCategories = new List<string>(new string[] { "日常", "服饰", "交通", "通迅", "娱乐", "人情往来", "学习" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "收入", SubCategories = new List<string>(new string[] { "工资", "红包" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "日常", SubCategories = new List<string>(new string[] { "超市", "买菜", "餐费", "厨房用品", "日用品", "家电家具", "水电煤","零食" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "服饰", SubCategories = new List<string>(new string[] { "外套", "鞋袜", "内衣", "护肤品", "运动用品" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "通迅", SubCategories = new List<string>(new string[] { "话费", "网费", "电视", "手机" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "交通", SubCategories = new List<string>(new string[] { "加油", "汽车保险", "汽车配件", "打车", "乘车", "停车费" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "娱乐", SubCategories = new List<string>(new string[] { "电影", "唱歌", "旅游", "节日", "运动" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "学习", SubCategories = new List<string>(new string[] { "学习用品", "书和资料", "培训" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "人情往来", SubCategories = new List<string>(new string[] { "礼品", "份子钱" }) });
        }
    }
}