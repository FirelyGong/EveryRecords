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

namespace EveryRecords.DataFactories
{
    public class CategoryDataFactory:DataFactory
    {
        public const string IncomeString="收入";
        public const string PaymentString="支出";

        public static readonly CategoryDataFactory Income = new CategoryDataFactory(IncomeString);
        public static readonly CategoryDataFactory Payment = new CategoryDataFactory(PaymentString);
        public const string RootCategory = "总计";   
        private CategoryData _categoryData;
        
        private CategoryDataFactory(string incomeOrPayment)
        {
            CategoryName = incomeOrPayment;
            _categoryData=new CategoryData();
            DataPath = Path.Combine(BasePath, incomeOrPayment + "_category.xml");
        }

        public static CategoryDataFactory GetInstance(string incomeOrPayment)
        {
            if (incomeOrPayment == IncomeString)
            {
                return Income;
            }
            else
            {
                return Payment;
            }
        }

        public string CategoryName { get; private set; }

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
                _categoryData = LoadData<CategoryData>();
                DataLoaded = true;
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
            if (CategoryName == PaymentString)
            {
                _categoryData.Categories.Add(new CategoryItem { Category = RootCategory, SubCategories = new List<string>(new string[] { "支出成员", "支出" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "支出成员", SubCategories = new List<string>(new string[] { "男1号", "女1号" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "支出", SubCategories = new List<string>(new string[] { "衣", "食", "住", "行", "娱乐", "通迅", "社交", "学习" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "支出/衣", SubCategories = new List<string>(new string[] { "外套", "上衣", "裤鞋袜", "内衣", "护肤品", "运动用品" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "支出/食", SubCategories = new List<string>(new string[] { "超市", "买菜", "餐费", "厨房用品", "家电家具", "零食" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "支出/住", SubCategories = new List<string>(new string[] { "水电煤", "日用品", "家电", "家具" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "支出/行", SubCategories = new List<string>(new string[] { "加油", "汽车保险", "汽车配件", "打车", "乘车", "停车费", "过路费" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "支出/娱乐", SubCategories = new List<string>(new string[] { "电影", "唱歌", "旅游", "节日", "运动" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "支出/通迅", SubCategories = new List<string>(new string[] { "话费", "网费", "手机" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "支出/社交", SubCategories = new List<string>(new string[] { "礼品", "份子钱" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "支出/学习", SubCategories = new List<string>(new string[] { "学习用品", "书和资料", "培训" }) });
            }
            else
            {
                _categoryData.Categories.Add(new CategoryItem { Category = RootCategory, SubCategories = new List<string>(new string[] { "收入成员", "收入" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "收入成员", SubCategories = new List<string>(new string[] { "男1号", "女1号" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "收入", SubCategories = new List<string>(new string[] { "工资", "公司福利", "红包" }) });
            }
        }
    }
}