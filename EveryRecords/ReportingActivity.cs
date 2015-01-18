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
    [Activity(Label = "ReportingActivity")]
    public class ReportingActivity : Activity
    {
        TextView _pathText;
        ListView _reportingList;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.ReportingLayout);
            _pathText = FindViewById<TextView>(Resource.Id.PathText);
            var back = FindViewById<Button>(Resource.Id.BackButton);
            back.Click += delegate
            {
                Finish();
            };

            var uplevel = FindViewById<Button>(Resource.Id.UpLevelButton);
            uplevel.Click += uplevel_Click;

            _reportingList = FindViewById<ListView>(Resource.Id.ReportsList);
            _reportingList.ItemClick += list_ItemClick;
            DisplayCategory("");
        }

        private void list_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = ((ListView)sender).Adapter.GetItem(e.Position).ToString();
            var current = item.Split(':')[0];
            _pathText.Text = _pathText.Text + "/" + current;
            DisplayCategory(current);
        }

        private void uplevel_Click(object sender, EventArgs e)
        {
            BackToLastLevel();
            DisplayCategory(GetCurrentLevel());
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
                return "";
            }

            return arr[arr.Length - 1];
        }

        private void DisplayCategory(string parent)
        {
            string[] datas = new string[] { };
            if (string.IsNullOrEmpty(parent))
            {
                var item=FormatListItem(CategoryDataFactory.RootCategory, RecordingDataFactory.Instance.GetCategorySummary(CategoryDataFactory.RootCategory));
                datas=new string[]{item};
            }
            else
            {
                var subs = CategoryDataFactory.Instance.GetSubCategories(parent);
                if (subs.Count == 0)
                {
                    datas = RecordingDataFactory.Instance.GetRecords(parent).ToArray();
                }
                else
                {
                    var items = RecordingDataFactory.Instance.GetSubCategoriesSummary(parent);
                    var list = new List<string>();
                    foreach (var item in items)
                    {
                        if (parent == CategoryDataFactory.RootCategory)
                        {
                            list.Add(item.Key);
                        }
                        else
                        {
                            list.Add(FormatListItem(item.Key, item.Value));
                        }
                    }

                    datas = list.ToArray();
                }
            }

            _reportingList.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1,datas);
        }

        private string FormatListItem(string category, int amount)
        {
            var item = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", category, amount);

        return item;
        }
    }
}