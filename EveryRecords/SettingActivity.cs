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
    [Activity(Label = "˫����ʾ����")]
    public class SettingActivity : Activity
    {
        TextView _pathText;
        EditText _currentNode;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "SettingLayout" layout resource
            SetContentView(Resource.Layout.SettingLayout);

            _currentNode = FindViewById<EditText>(Resource.Id.NodeText);
            _pathText = FindViewById<TextView>(Resource.Id.PathText);
            _pathText.Text = CategoryDataFactory.RootCategory;
            var back = FindViewById<Button>(Resource.Id.BackButton);
            back.Click += delegate
            {
                Finish();
            };

            var uplevel = FindViewById<Button>(Resource.Id.UpLevelButton);
            uplevel.Click += uplevel_Click;

            var add = FindViewById<Button>(Resource.Id.AddButton);
            add.Click += add_Click;

            var delete = FindViewById<Button>(Resource.Id.DeleteButton);
            delete.Click+=delete_Click;
            var list = FindViewById<ListView>(Resource.Id.CategoryItems);
            list.ItemClick += list_ItemClick;

            DisplayCategory(CategoryDataFactory.RootCategory);
            //SetListViewHeightBasedOnChildren(list);
        }

        private void delete_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(_currentNode.Text))
            {
                Toast.MakeText(this, "����ɾ����ֵ��", ToastLength.Long).Show();
                return;
            }

            var current = GetCurrentLevel();
            CategoryDataFactory.Instance.RemoveCategory(current, _currentNode.Text);
            DisplayCategory(current);
        }

        private void add_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentNode.Text))
            {
                Toast.MakeText(this, "������ӿ�ֵ��", ToastLength.Long).Show();
                return;
            }

            var current = GetCurrentLevel();
            if (!CategoryDataFactory.Instance.AddCategory(current, _currentNode.Text))
            {
                Toast.MakeText(this,"�����Ѵ��ڣ�", ToastLength.Long).Show();
            }
            else
            {
                DisplayCategory(current);
            }
        }

        private void uplevel_Click(object sender, EventArgs e)
        {
            BackToLastLevel();
            DisplayCategory(GetCurrentLevel());
        }

        private void list_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var current=((ListView)sender).Adapter.GetItem(e.Position).ToString();
            if (_currentNode.Text != current)
            {
                _currentNode.Text = current;
            }
            else
            {
                _pathText.Text = _pathText.Text + "/" + current;
                DisplayCategory(current);
            }
        }

        private void BackToLastLevel()
        {
            string[] arr = _pathText.Text.Split('/');
            if (arr.Length <= 1)
            {
                return;
            }

            _pathText.Text = _pathText.Text.Substring(0, _pathText.Text.LastIndexOf("/"));
        }

        private string GetCurrentLevel()
        {
            string[] arr = _pathText.Text.Split('/');
            if (arr.Length <= 1)
            {
                return CategoryDataFactory.RootCategory;
            }
                        
            return arr[arr.Length - 1];
        }

        private void DisplayCategory(string parent)
        {
            var list = FindViewById<ListView>(Resource.Id.CategoryItems);
            list.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1, CategoryDataFactory.Instance.GetSubCategories(parent));
            _currentNode.Text = "";
        }
        private void SetListViewHeightBasedOnChildren(ListView listView)
        {
            // ��ȡListView��Ӧ��Adapter   
            var listAdapter = listView.Adapter;
            if (listAdapter == null)
            {
                return;
            }

            int totalHeight = 0;
            for (int i = 0, len = listAdapter.Count; i < len; i++)
            {
                // listAdapter.getCount()�������������Ŀ   
                View listItem = listAdapter.GetView(i, null, listView);
                // ��������View �Ŀ��   
                listItem.Measure(0, 0);
                // ͳ������������ܸ߶�   
                totalHeight += listItem.MeasuredHeight;
            }

            var para = listView.LayoutParameters;
            para.Height = totalHeight + (listView.DividerHeight * (listAdapter.Count - 1));
            // listView.getDividerHeight()��ȡ�����ָ���ռ�õĸ߶�   
            // params.height���õ�����ListView������ʾ��Ҫ�ĸ߶�   
            listView.LayoutParameters = para;
        }
    }
}