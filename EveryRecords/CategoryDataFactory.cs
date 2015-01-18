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

namespace EveryRecords
{
    public class CategoryDataFactory
    {
        public static readonly CategoryDataFactory Instance = new CategoryDataFactory();

        public const string RootCategory = "����";

        private Dictionary<string, IList<string>> _datas;

        private CategoryDataFactory()
        {
            _datas = new Dictionary<string, IList<string>>();

            InitializeDatas();
        }

        public IList<string> GetCategories()
        {
            var list = new List<string>(_datas[RootCategory]);

            return list;
        }

        public IList<string> GetSubCategories(string category)
        {
            if (!_datas.ContainsKey(category))
            {
                return new string[]{};
            }

            return new List<string>(_datas[category]);
        }

        public bool AddCategory(string parent, string categoryName)
        {
            if (!_datas.ContainsKey(parent))
            {
                _datas.Add(parent, new List<string>());
            }

            if(_datas[parent].Contains(categoryName))
            {
                return false;
            }

            _datas[parent].Add(categoryName);
            return true;
        }

        public void RemoveCategory(string parent, string categoryName)
        {
            if (_datas.ContainsKey(parent))
            {
                _datas[parent].Remove(categoryName);
                if (_datas[parent].Count == 0)
                {
                    _datas.Remove(parent);
                }
            }
        }

        private void InitializeDatas()
        {
            _datas.Add(RootCategory, new List<string>(new string[] { "��Ա", "����֧��" }));
            _datas.Add("��Ա", new List<string>(new string[] { "FirelyGong", "Qihui" }));
            _datas.Add("����֧��", new List<string>(new string[] { "֧��", "����" }));
            _datas.Add("֧��",new List<string>( new string[] { "��ʳ", "��װ", "��ͨ","ͨѸ", "����" }));
            _datas.Add("����",new List<string>( new string[] { "����" }));
            _datas.Add("��ʳ",new List<string>( new string[] { "����", "���","�¹���" }));
            _datas.Add("��װ",new List<string>( new string[] { "�·�", "Ь��" }));
            _datas.Add("ͨѸ",new List<string>( new string[] { "�绰��", "����","����","�ֻ�" }));
            _datas.Add("��ͨ",new List<string>( new string[] { "����", "��������","�������" }));
            _datas.Add("����",new List<string>( new string[] { "���","ˮ��","����" }));
        }
    }
}