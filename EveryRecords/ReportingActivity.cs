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
    [Activity(Label = "ReportingActivity", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
    public class ReportingActivity : Activity
    {
        public const string RecordsYearMonthTag = "RecordsYearMonth";

        private const string NoRecord = "没有记录！";
        private TextView _pathText;
        private ListView _reportingList;
        private bool _displayingDetail;
        private RecordingDataFactory _reocrdingData;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.ReportingLayout);

            var yearMonth = Intent.GetStringExtra(RecordsYearMonthTag);
                int year;
                int month;
            if (string.IsNullOrEmpty(yearMonth))
            {
                year = DateTime.Now.Year;
                month = DateTime.Now.Month;
                _reocrdingData = RecordingDataFactory.Instance;
            }
            else
            {
                string[] arr=yearMonth.Split('-');
                int.TryParse(arr[0], out year);
                int.TryParse(arr[1], out month);
                _reocrdingData = RecordingDataFactory.CreateHistoryData(year, month);
                _reocrdingData.LoadData();
            }
            var subTitle = FindViewById<TextView>(Resource.Id.SubTitleText);
            subTitle.Text = string.Format(CultureInfo.InvariantCulture, "{0}年{1}月", year, month);

            _pathText = FindViewById<TextView>(Resource.Id.PathText);
            _pathText.Click += uplevel_Click;
            var back = FindViewById<Button>(Resource.Id.BackButton);
            back.Click += delegate
            {
                Finish();
            };
            
            _reportingList = FindViewById<ListView>(Resource.Id.ReportsList);
            _reportingList.ItemClick += list_ItemClick;
            DisplayCategory("");
        }

        private void list_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = ((ListView)sender).Adapter.GetItem(e.Position).ToString();
            if (item == NoRecord || _displayingDetail)
            {
                return;
            }

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
                _displayingDetail = false;
                var item = FormatListItem(CategoryDataFactory.RootCategory, _reocrdingData.GetCategorySummary(CategoryDataFactory.RootCategory));
                datas=new string[]{item};
            }
            else
            {
                var subs = CategoryDataFactory.Instance.GetSubCategories(parent);
                if (subs.Count == 0)
                {
                    _displayingDetail = true;
                    datas = _reocrdingData.GetRecords(parent).ToArray();
                }
                else
                {
                    _displayingDetail = false;
                    var items = _reocrdingData.GetSubCategoriesSummary(parent);
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

            if(datas.Length==0)
            {
                datas = new string[] { NoRecord };
            }
            _reportingList.Adapter = new SimpleListAdapter(this, datas.ToList());
        }

        private string FormatListItem(string category, double amount)
        {
            var item = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", category, amount);

        return item;
        }
    }
}