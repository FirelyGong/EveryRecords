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

        public const string RootCategory = "�ܼ�";
        
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
            _categoryData.Categories.Add(new CategoryItem { Category = RootCategory, SubCategories = new List<string>(new string[] { "��Ա", "����֧��" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "��Ա", SubCategories = new List<string>(new string[] { "FirelyGong", "Qihui" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "����֧��", SubCategories = new List<string>(new string[] { "֧��", "����" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "֧��", SubCategories = new List<string>(new string[] { "�ճ�", "����", "��ͨ", "ͨѸ", "����", "��������", "ѧϰ" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "����", SubCategories = new List<string>(new string[] { "����", "���" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "�ճ�", SubCategories = new List<string>(new string[] { "����", "���", "�ͷ�", "������Ʒ", "����Ʒ", "�ҵ�Ҿ�", "ˮ��ú","��ʳ" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "����", SubCategories = new List<string>(new string[] { "����", "Ь��", "����", "����Ʒ", "�˶���Ʒ" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "ͨѸ", SubCategories = new List<string>(new string[] { "����", "����", "����", "�ֻ�" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "��ͨ", SubCategories = new List<string>(new string[] { "����", "��������", "�������", "��", "�˳�", "ͣ����" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "����", SubCategories = new List<string>(new string[] { "��Ӱ", "����", "����", "����", "�˶�" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "ѧϰ", SubCategories = new List<string>(new string[] { "ѧϰ��Ʒ", "�������", "��ѵ" }) });
            _categoryData.Categories.Add(new CategoryItem { Category = "��������", SubCategories = new List<string>(new string[] { "��Ʒ", "����Ǯ" }) });
        }
    }
}