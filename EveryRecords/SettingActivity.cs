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
    [Activity(Label = "双击显示子项", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
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
            _pathText.Click += uplevel_Click;
            _pathText.Text = CategoryDataFactory.RootCategory;
            var back = FindViewById<Button>(Resource.Id.BackButton);
            back.Click += delegate
            {
                Finish();
            };
            
            var add = FindViewById<Button>(Resource.Id.AddButton);
            add.Click += add_Click;

            var delete = FindViewById<Button>(Resource.Id.DeleteButton);
            delete.Click+=delete_Click;
            var list = FindViewById<ListView>(Resource.Id.CategoryItems);
            list.ItemClick += list_ItemClick;

            DisplayCategory(CategoryDataFactory.RootCategory);
        }

        protected override void OnPause()
        {
            try
            {
                CategoryDataFactory.Instance.SaveData();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }

            base.OnPause();
        }
        
        private void delete_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(_currentNode.Text))
            {
                Toast.MakeText(this, "不能删除空值！", ToastLength.Long).Show();
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
                Toast.MakeText(this, "不能添加空值！", ToastLength.Long).Show();
                return;
            }

            var current = GetCurrentLevel();
            if (!CategoryDataFactory.Instance.AddCategory(current, _currentNode.Text))
            {
                Toast.MakeText(this,"分类已存在！", ToastLength.Long).Show();
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
            list.Adapter = new SimpleListAdapter(this, CategoryDataFactory.Instance.GetSubCategories(parent).ToList());
            _currentNode.Text = "";
        }
    }
}