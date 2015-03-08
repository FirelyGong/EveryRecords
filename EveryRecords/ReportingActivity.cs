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
using EveryRecords.Charts;
using EveryRecords.ListAdapters;

namespace EveryRecords
{
    [Activity(Label = "ReportingActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ReportingActivity : Activity
    {
        public const string RecordsYearMonthTag = "RecordsYearMonth";
        private const string NoRecord = "没有记录！";
        private TextView _subTitle;
        private TextView _title;
        private GridView _reportingList;
        private RecordingDataFactory _reocrdingData;
        private ChartPane _chartPane;
        int _year;
        int _month;
        Button _share;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.ReportingLayout);

            this.InitialActivity(() => OnBackPressed());

            var yearMonth = Intent.GetStringExtra(RecordsYearMonthTag);
            if (string.IsNullOrEmpty(yearMonth))
            {
                _year = DateTime.Now.Year;
                _month = DateTime.Now.Month;
                _reocrdingData = RecordingDataFactory.Instance;
            }
            else
            {
                string[] arr=yearMonth.Split('-');
                int.TryParse(arr[0], out _year);
                int.TryParse(arr[1], out _month);
                _reocrdingData = RecordingDataFactory.CreateHistoryData(_year, _month);
                _reocrdingData.LoadData();
            }
            _title = FindViewById<TextView>(Resource.Id.TitleText);
            _title.Text = string.Format(CultureInfo.InvariantCulture, "{0}年{1}月", _year, _month);
            _subTitle = FindViewById<TextView>(Resource.Id.SubTitleText);
            _share = FindViewById<Button>(Resource.Id.ShareButton);
            _share.Click += share_Click;
            _chartPane = FindViewById<ChartPane>(Resource.Id.PieChart);
            _reportingList = FindViewById<GridView>(Resource.Id.ReportsList);
            _reportingList.ItemClick += list_ItemClick;
            DisplayCategory("");
        }

        public override void OnBackPressed()
        {
            if (string.IsNullOrEmpty(_subTitle.Text))
            {
                base.OnBackPressed();
                Finish();
            }
            else
            {
                var parent = BackToLastLevel();
                DisplayCategory(parent);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 0)
            {
                var item = BackToLastLevel();
                if (resultCode == Result.Ok)
                {
                    _reocrdingData.LoadData();
                    DisplayCategory(item);
                }
            }
        }

        private void list_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = ((GridView)sender).Adapter.GetItem(e.Position).ToString();
            if (item == NoRecord)
            {
                return;
            }

            var current = item.Split(':')[0];
            _subTitle.Text = _subTitle.Text + "/" + current;
            DisplayCategory(current);
        }
        
        private string BackToLastLevel()
        {
            string[] arr = _subTitle.Text.Split('/');
            if (arr.Length <= 1)
            {
                return "";
            }

            _subTitle.Text = _subTitle.Text.Substring(0, _subTitle.Text.LastIndexOf("/"));
            return arr[arr.Length - 2];
        }
        
        private void DisplayCategory(string parent)
        {
            string[] datas = new string[] { };
            if (string.IsNullOrEmpty(parent))
            {
                var item = FormatListItem(CategoryDataFactory.RootCategory, _reocrdingData.GetCategorySummary(CategoryDataFactory.RootCategory));
                datas = new string[] { item };
                _reportingList.Adapter = new GridListAdapter(this, datas.ToList());
                SetShareButtonVisible();
            }
            else
            {
                var subs = CategoryDataFactory.Instance.GetSubCategories(parent);
                if (subs.Count > 0)
                {
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
                    _reportingList.Adapter = new GridListAdapter(this, datas.ToList());
                    if (datas[0].Contains(":"))
                    {
                        var data = datas.Select(d => double.Parse(d.Split(':')[1])).ToArray();
                        var label = datas.Select(d => d.Split(':')[0]).ToArray();
                        _chartPane.InitializeChart(ChartType.Pie, data, label);
                    }
                    else
                    {
                        _chartPane.Clear();
                    }

                    SetShareButtonVisible();
                }
                else
                {
                    var intent = new Intent(this, typeof(DetailListActivity));
                    intent.PutExtra(DetailListActivity.RecordsYearMonthTag, string.Format(CultureInfo.InvariantCulture,"{0}-{1}", _year, _month));
                    intent.PutExtra(DetailListActivity.RecordsPathTag, _subTitle.Text);
                    intent.PutExtra(DetailListActivity.RecordsCategoryTag, parent);
                    StartActivityForResult(intent, 0);
                }
            }                       
        }

        private string FormatListItem(string category, double amount)
        {
            var item = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", category, amount);

            return item;
        }

        private void share_Click(object sender, EventArgs e)
        {
            var list = ((StringListAdapter)_reportingList.Adapter).GetSource();
            string[] arr = _subTitle.Text.Split('/');
            if (arr.Length <= 1)
            {
                return;
            }

            var intent = new Intent(this, typeof(CategoryGraphActivity));
            intent.PutExtra(CategoryGraphActivity.RecordsYearMonthTag, _title.Text);
            var category = arr[arr.Length - 1];
            intent.PutExtra(CategoryGraphActivity.RecordsCategoryTag, category);
            intent.PutExtra(CategoryGraphActivity.RecordsDetailTag, string.Join(";", list));
            StartActivity(intent);
        }

        private void SetShareButtonVisible()
        {
            var list = ((StringListAdapter)_reportingList.Adapter).GetSource();
            if (list.Count > 1 && list[0].Contains(":"))
            {
                _share.Visibility = ViewStates.Visible;
            }
            else
            {
                _share.Visibility = ViewStates.Invisible;
            }
        }
    }
}