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
        public const string IncomeString="����";
        public const string PaymentString="֧��";

        public static readonly CategoryDataFactory Income = new CategoryDataFactory(IncomeString);
        public static readonly CategoryDataFactory Payment = new CategoryDataFactory(PaymentString);
        public const string RootCategory = "�ܼ�";   
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
                _categoryData.Categories.Add(new CategoryItem { Category = RootCategory, SubCategories = new List<string>(new string[] { "֧����Ա", "֧��" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "֧����Ա", SubCategories = new List<string>(new string[] { "��1��", "Ů1��" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "֧��", SubCategories = new List<string>(new string[] { "��", "ʳ", "ס", "��", "����", "ͨѸ", "�罻", "ѧϰ" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "֧��/��", SubCategories = new List<string>(new string[] { "����", "����", "��Ь��", "����", "����Ʒ", "�˶���Ʒ" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "֧��/ʳ", SubCategories = new List<string>(new string[] { "����", "���", "�ͷ�", "������Ʒ", "�ҵ�Ҿ�", "��ʳ" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "֧��/ס", SubCategories = new List<string>(new string[] { "ˮ��ú", "����Ʒ", "�ҵ�", "�Ҿ�" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "֧��/��", SubCategories = new List<string>(new string[] { "����", "��������", "�������", "��", "�˳�", "ͣ����", "��·��" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "֧��/����", SubCategories = new List<string>(new string[] { "��Ӱ", "����", "����", "����", "�˶�" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "֧��/ͨѸ", SubCategories = new List<string>(new string[] { "����", "����", "�ֻ�" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "֧��/�罻", SubCategories = new List<string>(new string[] { "��Ʒ", "����Ǯ" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "֧��/ѧϰ", SubCategories = new List<string>(new string[] { "ѧϰ��Ʒ", "�������", "��ѵ" }) });
            }
            else
            {
                _categoryData.Categories.Add(new CategoryItem { Category = RootCategory, SubCategories = new List<string>(new string[] { "�����Ա", "����" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "�����Ա", SubCategories = new List<string>(new string[] { "��1��", "Ů1��" }) });
                _categoryData.Categories.Add(new CategoryItem { Category = "����", SubCategories = new List<string>(new string[] { "����", "��˾����", "���" }) });
            }
        }
    }
}