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
using EveryRecords.DataFactories;
using EveryRecords.ListAdapters;

namespace EveryRecords
{
    [Activity(Label = "CategoryActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class CategoryActivity : Activity
    {
        TextView _subTitle;
        EditText _currentNode;
        Button _addButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "SettingLayout" layout resource
            SetContentView(Resource.Layout.CategoryLayout);

            this.InitialActivity(() => OnBackPressed());

            _currentNode = FindViewById<EditText>(Resource.Id.NodeText);
            _subTitle = FindViewById<TextView>(Resource.Id.SubTitleText);
            _subTitle.Text = "";

            _addButton = FindViewById<Button>(Resource.Id.AddButton);
            _addButton.Click += add_Click;

            var list = FindViewById<ListView>(Resource.Id.CategoryItems);
            list.ItemClick += list_ItemClick;
            list.ItemLongClick += list_ItemLongClick;
            DisplayCategory("");
        }

        public override void OnBackPressed()
        {
            if (string.IsNullOrEmpty(_subTitle.Text))
            {
                base.OnBackPressed();
            }
            else
            {
                var path = _subTitle.Text;
                AppExtension.BackToLastLevel(ref path);
                _subTitle.Text = path;
                DisplayCategory(_subTitle.Text);
            }
        }

        protected override void OnPause()
        {
            try
            {
                CategoryDataFactory.Income.SaveData();
                CategoryDataFactory.Payment.SaveData();
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
                    CategoryDataFactory.Payment.RemoveCategory(_subTitle.Text, item);
                    DisplayCategory(_subTitle.Text);
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

            if (!CategoryDataFactory.Payment.AddCategory(_subTitle.Text, _currentNode.Text))
            {
                Toast.MakeText(this, "分类已存在！", ToastLength.Long).Show();
            }
            else
            {
                DisplayCategory(_subTitle.Text);
            }
        }
        
        private void list_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var current = ((ListView)sender).Adapter.GetItem(e.Position).ToString();

            if (string.IsNullOrEmpty(_subTitle.Text))
            {
                _subTitle.Text = current;
            }
            else
            {
                _subTitle.Text = _subTitle.Text + "/" + current;
            }

            DisplayCategory(_subTitle.Text);
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
        
        private void DisplayCategory(string parent)
        {
            var list = FindViewById<ListView>(Resource.Id.CategoryItems);
            IList<string> data = null;
            if (string.IsNullOrEmpty(parent))
            {
                data = new List<string>(new string[] { "支出", "支出成员", "收入", "收入成员" }); 
                _addButton.Visibility = ViewStates.Invisible;
            }
            else
            {
                _addButton.Visibility = ViewStates.Visible;
                if (parent.Contains("支出"))
                {
                    data = CategoryDataFactory.Payment.GetSubCategories(parent).ToList();
                }
                else
                {
                    data = CategoryDataFactory.Income.GetSubCategories(parent).ToList();
                }
            }

            list.Adapter = new SimpleListAdapter(this, data);
            _currentNode.Text = "";
        }
    }
}