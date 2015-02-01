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
using System.Globalization;

namespace EveryRecords
{
    [Activity(Label = "CategoryActivity", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
    public class CategoryActivity : Activity
    {
        TextView _pathText;
        EditText _currentNode;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "SettingLayout" layout resource
            SetContentView(Resource.Layout.CategoryLayout);

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

            var list = FindViewById<ListView>(Resource.Id.CategoryItems);
            list.ItemClick += list_ItemClick;
            list.ItemLongClick += list_ItemLongClick;
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

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 0)
            {
                if (resultCode == Result.Ok)
                {
                    var item = data.GetStringExtra(ConfirmActivity.DataTag);
                    var current = GetCurrentLevel();
                    CategoryDataFactory.Instance.RemoveCategory(current, item);
                    DisplayCategory(current);
                }
            }
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
                Toast.MakeText(this, "分类已存在！", ToastLength.Long).Show();
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
            var current = ((ListView)sender).Adapter.GetItem(e.Position).ToString();

            _pathText.Text = _pathText.Text + "/" + current;
            DisplayCategory(current);
        }

        private void list_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            var list = sender as ListView;
            var item = list.Adapter.GetItem(e.Position).ToString();

            var intent = new Intent(this, typeof(ConfirmActivity));
            intent.PutExtra(ConfirmActivity.MessageTag, string.Format(CultureInfo.InvariantCulture, "确定要删除[{0}]吗？", item));
            intent.PutExtra(ConfirmActivity.DataTag, item);
            intent.PutExtra(ConfirmActivity.ReturnToTag, GetType().FullName);
            StartActivityForResult(intent, 0);
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