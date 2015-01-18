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

        public const string RootCategory = "分类";

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
            _datas.Add(RootCategory, new List<string>(new string[] { "成员", "收入支出" }));
            _datas.Add("成员", new List<string>(new string[] { "FirelyGong", "Qihui" }));
            _datas.Add("收入支出", new List<string>(new string[] { "支出", "收入" }));
            _datas.Add("支出",new List<string>( new string[] { "伙食", "服装", "交通","通迅", "其它" }));
            _datas.Add("收入",new List<string>( new string[] { "工资" }));
            _datas.Add("伙食",new List<string>( new string[] { "超市", "买菜","下馆子" }));
            _datas.Add("服装",new List<string>( new string[] { "衣服", "鞋子" }));
            _datas.Add("通迅",new List<string>( new string[] { "电话费", "网费","电视","手机" }));
            _datas.Add("交通",new List<string>( new string[] { "加油", "汽车保险","汽车配件" }));
            _datas.Add("其它",new List<string>( new string[] { "电费","水费","其它" }));
        }
    }
}