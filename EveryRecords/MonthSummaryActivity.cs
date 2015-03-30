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
    [Activity(Label = "MonthSummaryActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MonthSummaryActivity: Activity
    {
        private RecordingDataFactory _reocrdingData;
        private int _year;
        private int _month;
        private ListView _paymentList;
        private ListView _incomeList;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.MonthSummaryLayout);

            this.InitialActivity(() => OnBackPressed());
            _paymentList = FindViewById<ListView>(Resource.Id.PaymentList);
            _paymentList.ItemClick += paymentList_ItemClick;
            _incomeList = FindViewById<ListView>(Resource.Id.IncomeList);
            _incomeList.ItemClick += incomeList_ItemClick;
            UpdateUi();
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

            var paymentAmount = _reocrdingData.GetCategorySummary(RecordTypes.Payment.ToLabel());
            var paymentUsers = _reocrdingData.GetSubCategoriesSummary("支出成员");
            IList<string> paymentList = new List<string>(paymentUsers.Select(r=>r.Key+":"+r.Value));
            paymentList.Insert(0, RecordTypes.Payment.ToLabel() + ":" + paymentAmount);
            _paymentList.Adapter = new ColumnListAdapter(this, paymentList, true);
            var paymentChart = FindViewById<ChartPane>(Resource.Id.PaymentChart);
            var paymentPie = new PieChart(paymentUsers.Select(r => r.Value).ToArray(), paymentUsers.Select(r => r.Key).ToArray());
            paymentChart.InitializeChart(paymentPie);

            var incomeAmount = _reocrdingData.GetCategorySummary(RecordTypes.Income.ToLabel());
            var incomeUsers = _reocrdingData.GetSubCategoriesSummary("收入成员");
            IList<string> incomeList = new List<string>(incomeUsers.Select(r => r.Key + ":" + r.Value));
            incomeList.Insert(0, RecordTypes.Income.ToLabel() + ":" + incomeAmount);
            _incomeList.Adapter = new ColumnListAdapter(this, incomeList, true);
            var incomeChart = FindViewById<ChartPane>(Resource.Id.IncomeChart);
            var incomePie = new PieChart(incomeUsers.Select(r => r.Value).ToArray(), incomeUsers.Select(r => r.Key).ToArray());
            incomeChart.InitializeChart(incomePie);
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

        private FrameElements Elements
        {
            get
            {
                return this.GetFrameElements();
            }
        }

        private void incomeList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var intent = new Intent(this, typeof(ReportingActivity));
            var item = _incomeList.Adapter.GetItem(e.Position).ToString().Split(':')[0];
            if (e.Position != 0)
            {
                item = "收入成员/" + item;
            }

            intent.PutExtra(FrameElements.RecordsCategoryTag, item);
            intent.PutExtra(FrameElements.CategoryTypeTag, RecordTypes.Income.ToLabel());
            intent.PutExtra(FrameElements.RecordsYearMonthTag, string.Format(CultureInfo.InvariantCulture, "{0}-{1}", _year, _month));
            StartActivity(intent);
        }

        private void paymentList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var intent = new Intent(this, typeof(ReportingActivity));
            var item = _paymentList.Adapter.GetItem(e.Position).ToString().Split(':')[0];
            if (e.Position != 0)
            {
                item = "支出成员/" + item;
            }

            intent.PutExtra(FrameElements.RecordsCategoryTag, item);
            intent.PutExtra(FrameElements.CategoryTypeTag, RecordTypes.Payment.ToLabel());
            intent.PutExtra(FrameElements.RecordsYearMonthTag, string.Format(CultureInfo.InvariantCulture, "{0}-{1}", _year, _month));
            StartActivity(intent);
        }
    }
}
