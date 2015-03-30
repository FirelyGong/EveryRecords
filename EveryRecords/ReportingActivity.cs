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
using System.Threading.Tasks;

namespace EveryRecords
{
    [Activity(Label = "ReportingActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ReportingActivity : Activity
    {
        private const string NoRecord = "没有记录！";
        
        private RecordingDataFactory _reocrdingData;
        private ChartPane _chartPane;
        private int _year;
        private int _month;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.ReportingLayout);

            this.InitialActivity(() => OnBackPressed());

            Elements.GraphicButton.Click += share_Click;
            _chartPane = FindViewById<ChartPane>(Resource.Id.PieChart);
            Elements.Grid.ItemClick += list_ItemClick;
            UpdateUi();
        }

        public override void OnBackPressed()
        {
            BackToLastLevel();
            if (string.IsNullOrEmpty(Elements.SubTitle.Text))
            {
                base.OnBackPressed();
                Finish();
            }
            else
            {
                DisplayCategory(Elements.SubTitle.Text);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 0)
            {
                BackToLastLevel();
                if (string.IsNullOrEmpty(Elements.SubTitle.Text))
                {
                    Finish();
                }

                if (resultCode == Result.Ok)
                {
                    _reocrdingData.LoadData();
                }

                DisplayCategory(Elements.SubTitle.Text);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();

            this.RemoveFrameElements();
        }

        private async void UpdateUi()
        {
            await LoadDataAsync();

            Elements.Title.Text = string.Format(CultureInfo.InvariantCulture, "{0}年{1}月", _year, _month);

            var category = Intent.GetStringExtra(FrameElements.RecordsCategoryTag);
            Elements.SubTitle.Text = category;
            DisplayCategory(category);
        }

        private Task<bool> LoadDataAsync()
        {
            return Task.Run(() =>
            {
                var yearMonth = Intent.GetStringExtra(FrameElements.RecordsYearMonthTag);
                if (string.IsNullOrEmpty(yearMonth))
                {
                    _year = DateTime.Now.Year;
                    _month = DateTime.Now.Month;
                    _reocrdingData = RecordingDataFactory.Instance;
                }
                else
                {
                    string[] arr = yearMonth.Split('-');
                    int.TryParse(arr[0], out _year);
                    int.TryParse(arr[1], out _month);
                    _reocrdingData = RecordingDataFactory.CreateHistoryData(_year, _month);
                    _reocrdingData.LoadData();
                }

                return true;
            });
        }

        private void list_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = ((GridView)sender).Adapter.GetItem(e.Position).ToString();
            if (item == NoRecord)
            {
                return;
            }

            var current = item.Split(':')[0];
            Elements.SubTitle.Text += ("/" + current);
            DisplayCategory(Elements.SubTitle.Text);
        }
        
        private void BackToLastLevel()
        {
            var path = Elements.SubTitle.Text;
            string[] arr = path.Split('/');
            if (arr.Length <= 1)
            {
                Elements.SubTitle.Text = "";
            }
            else
            {
                Elements.SubTitle.Text = path.Substring(0, path.LastIndexOf("/"));
            }
        }
        
        private void DisplayCategory(string parent)
        {
            if (string.IsNullOrEmpty(parent))
            {
                return;
            }
            else
            {
                var items = _reocrdingData.GetSubCategoriesSummary(parent);
                if (items.Count > 0)
                {
                    var list = new List<string>();
                    foreach (var item in items)
                    {
                        list.Add(FormatListItem(item.Key, item.Value));
                    }

                    Elements.Grid.Adapter = new GridListAdapter(this, list);
                    if (list[0].Contains(":"))
                    {
                        var data = list.Select(d => double.Parse(d.Split(':')[1])).ToArray();
                        var label = list.Select(d => d.Split(':')[0]).ToArray();
                        IChart chart = new PieChart(data, label);
                        _chartPane.InitializeChart(chart);
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
                    intent.PutExtra(FrameElements.RecordsYearMonthTag, string.Format(CultureInfo.InvariantCulture, "{0}-{1}", _year, _month));
                    intent.PutExtra(FrameElements.RecordsCategoryTag, parent);
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
            var list = ((StringListAdapter)Elements.Grid.Adapter).GetSource();
            if (list.Count <= 1)
            {
                return;
            }

            var intent = new Intent(this, typeof(CategoryGraphActivity));
            intent.PutExtra(FrameElements.RecordsYearMonthTag, Elements.Title.Text);
            var category = Elements.SubTitle.Text;
            intent.PutExtra(FrameElements.RecordsCategoryTag, category);
            intent.PutExtra(CategoryGraphActivity.RecordsDetailTag, string.Join(";", list));
            StartActivity(intent);
        }

        private void SetShareButtonVisible()
        {
            var list = ((StringListAdapter)Elements.Grid.Adapter).GetSource();
            if (list.Count > 1 && list[0].Contains(":"))
            {
                Elements.GraphicButton.Visibility = ViewStates.Visible;
            }
            else
            {
                Elements.GraphicButton.Visibility = ViewStates.Invisible;
            }
        }

        private FrameElements Elements
        {
            get
            {
                return this.GetFrameElements();
            }
        }
    }
}