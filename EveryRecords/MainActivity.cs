using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Globalization;
using EveryRecords.DataFactories;
using System.Linq;
using System.Threading.Tasks;
using EveryRecords.Charts;
using Android.Graphics;
namespace EveryRecords
{
    [Activity(Label = "花哪了", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        private Button _recording;
        private Button _reporting;
        private Button _setting;
        private Button _exit;
        private Button _history;
        private string _verString;
        private DateTime _lastQuitTime;
        private TextView _sumText;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.MainLayout);

            this.InitialActivity(null);
            
            _sumText = FindViewById<TextView>(Resource.Id.SummaryText);
            var yearMonthText = FindViewById<TextView>(Resource.Id.YearMonthText);
            yearMonthText.Text = string.Format(CultureInfo.InvariantCulture, "{0}年{1}月", DateTime.Now.Year, DateTime.Now.Month);
            _recording = FindViewById<Button>(Resource.Id.RecordButton);
            _recording.Click += delegate
            {
                StartActivity(typeof(StartRecordingActivity));
            };

            _reporting = FindViewById<Button>(Resource.Id.ViewReportButton);
            _reporting.Click += delegate
            {
                var intent = new Intent(this, typeof(ReportingActivity));
                var now = DateTime.Now;
                intent.PutExtra(ReportingActivity.RecordsYearMonthTag, string.Format(CultureInfo.InvariantCulture, "{0}-{1}", now.Year, now.Month));

                StartActivity(intent);
            };

            _setting = FindViewById<Button>(Resource.Id.SettingButton);
            _setting.Click += delegate
            {
                StartActivity(typeof(SettingActivity));
            };

            var cateogry = FindViewById<Button>(Resource.Id.CategoryButton);
            cateogry.Click += delegate
            {
                StartActivity(typeof(CategoryActivity));
            };

            _exit = FindViewById<Button>(Resource.Id.ExitButton);
            _exit.Click += delegate
            {
                Finish();
            };

            _history = FindViewById<Button>(Resource.Id.HistoryButton);
            _history.Click += delegate
            {
                StartActivity(typeof(HistoryActivity));
            };

            var ver = FindViewById<Button>(Resource.Id.VerButton);
            _verString = GetType().Assembly.GetName().Version.ToString();
            ver.Text = _verString;
            ver.Click += delegate
            {
                StartActivity(typeof(AboutActivity));
            };

            _sumText.Text = "数据加载中..";
        }

        protected override void OnStart()
        {
            base.OnStart();
            UpdateUiInfo();
        }

        protected override void OnResume()
        {
            base.OnResume();

            UpdateUiInfo();
        }

        public override void OnBackPressed()
        {
            if ((DateTime.Now - _lastQuitTime).TotalSeconds >= 2)
            {
                _lastQuitTime = DateTime.Now;
                Toast.MakeText(this, "再按一次返回键退出", ToastLength.Short).Show();
            }
            else
            {
                base.OnBackPressed();
            }
        }

        private void ShowIntroduction()
        {
            if (_verString != SettingDataFactory.Instance.AppVersion)
            {
                StartActivity(typeof(IntroductionActivity));
                SettingDataFactory.Instance.AppVersion = _verString;
                SettingDataFactory.Instance.SaveData();
            }
        }

        private async void UpdateUiInfo()
        {
            var bln = await LoadDataAsync();

            UpdatePercentage();
            UpdateRecents();
            _sumText.Text = "当月记录金额:" + RecordingDataFactory.Instance.GetCategorySummary(CategoryDataFactory.RootCategory);

            ShowIntroduction();
        }

        private void UpdatePercentage()
        {
            var percentUI = FindViewById<ChartPane>(Resource.Id.PercentChart);
            var parameters = new LinearLayout.LayoutParams(percentUI.LayoutParameters);
            var limit = SettingDataFactory.Instance.ExpensesLimit;

            var amount = RecordingDataFactory.Instance.GetCategorySummary(CategoryDataFactory.RootCategory);

            if (limit <= 0)
            {
                limit = amount;
            }

            percentUI.InitializeChart(ChartType.Progress, new[] { amount, limit }, null, this.StringToColor("#4682B4"));
        }

        private void UpdateRecents()
        {
            var historic = HistoricDataFactory.Instance.GetHistoryList(DateTime.Now.Year, DateTime.Now.Month);
            historic.Remove(HistoricDataFactory.NoHistoryList);
            var zhuxing = FindViewById<ChartPane>(Resource.Id.ZhuXingTu);

            var count = historic.Count;
            if (count > 6)
            {
                for (int i = 6; i < count; i++)
                {
                    historic.RemoveAt(0);
                }
            }
            else
            {
                for (int i = count; i < 6; i++)
                {
                    historic.Add("0:0");
                }
            }

            var amounts = historic.Select(h => double.Parse(h.Split(':')[1])).ToList();
            var labels = historic.Select(h => h.Split(':')[0]).ToList();

            zhuxing.InitializeChart(ChartType.Histogram, amounts.ToArray(), labels.ToArray(), Color.LightGray);
        }

        private Task<bool> LoadDataAsync()
        {
            return Task.Run(() =>
            {
                LoadData();
                return true;
            });
        }

        private void LoadData()
        {
            CategoryDataFactory.Instance.LoadData();
            RecordingDataFactory.Instance.LoadData();
            SettingDataFactory.Instance.LoadData();
            HistoricDataFactory.Instance.LoadData();
        }
    }
}

