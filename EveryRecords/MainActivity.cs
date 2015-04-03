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
using System.IO;
namespace EveryRecords
{
    [Activity(Label = "花哪了", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        private Button _setting;
        private Button _exit;
        private Button _history;
        private string _verString;
        private DateTime _lastQuitTime;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            try
            {
                // Set our view from the "main" layout resource
                SetContentView(Resource.Layout.MainLayout);

                this.InitialActivity(null);

                var payment = FindViewById<Button>(Resource.Id.PaymentButton);
                payment.Click += payment_Click;

                var income = FindViewById<Button>(Resource.Id.IncomeButton);
                income.Click += income_Click;
                var reporting = FindViewById<Button>(Resource.Id.ViewReportButton);
                reporting.Click += reporting_Click;

                _setting = FindViewById<Button>(Resource.Id.SettingButton);
                _setting.Click += delegate
                {
                    StartActivity(typeof(SettingActivity));
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
                _verString = GetType().Assembly.GetName().Version.ToShortString();
                ver.Text = _verString;
                ver.Click += delegate
                {
                    StartActivity(typeof(AboutActivity));
                };

                //_sumText.Text = "数据加载中..";
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
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
            ShowIntroduction();
        }

        private void UpdatePercentage()
        {
            var percentUI = FindViewById<ChartPane>(Resource.Id.PercentChart);
            var parameters = new LinearLayout.LayoutParams(percentUI.LayoutParameters);
            var limit = SettingDataFactory.Instance.ExpensesLimit;

            var amount = RecordingDataFactory.Instance.GetCategorySummary("支出");

            if (limit <= 0)
            {
                limit = amount;
            }

            IChart chart = new ProgressCircleChart(limit, amount,
                new[]{string.Format(CultureInfo.InvariantCulture, "{0}年{1}月", DateTime.Now.Year, DateTime.Now.Month)
            ,"当月支出金额:" +amount}, this.StringToColor("#ffffff"), this.StringToColor("#ffffff"));

            percentUI.InitializeChart(chart);
        }

        private void UpdateRecents()
        {
            var recordType=RecordTypes.Payment.ToLabel()+"/日期";
            var now = DateTime.Now;
            var historic = RecordingDataFactory.Instance.GetDailySummaries(recordType,now, 7);
            var count = historic.Count;
            if (now.Day < 7)
            {
                var histRecords = RecordingDataFactory.CreateHistoryData(now.Year, now.Month - 1);
                histRecords.LoadData();
                var histList = histRecords.GetDailySummaries(recordType, now.AddDays(0 - now.Day), 7 - count);
                histList.Reverse().ToList().ForEach(h=>historic.Insert(0,h));
            }
                       
            var zhuxing = FindViewById<ChartPane>(Resource.Id.ZhuXingTu);
            var amounts = historic.Select(h => double.Parse(h.Split(':')[1])).ToList();
            var dates = historic.Select(h => DateTime.Parse(h.Split(':')[0])).ToList();
            var labels = dates.Select(h => h.Day.ToString()).ToList();

            for (int i = 0; i < 7; i++)
            {
                var dte = now.Date.AddDays(0 - i);

                var lbl = now.AddDays(0 - i).Day.ToString();
                var index = dates.BinarySearch(dte);
                if (index < 0)
                {
                    var pos = ~index;
                    labels.Insert(pos, lbl);
                    amounts.Insert(pos, 0);
                }
            }

            IChart chart = new Histogram(amounts.ToArray(), labels.ToArray(), this.StringToColor("#FF9900"), this.StringToColor("#4682B4"));

            zhuxing.InitializeChart(chart);
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
            CategoryDataFactory.Payment.LoadData();
            CategoryDataFactory.Income.LoadData();
            RecordingDataFactory.Instance.LoadData();
            SettingDataFactory.Instance.LoadData();
            HistoricDataFactory.Instance.LoadData();
        }

        private void LogException(Exception ex)
        {
            string logFile = "exception.txt";
            string folder=System.IO.Path.Combine(DataFactory.GetDataFolder(), "log");
            string path = System.IO.Path.Combine(folder, logFile);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var fileMode = FileMode.Append;
            if (!File.Exists(path))
            {
                fileMode = FileMode.Create;
            }

            using (var fs = new FileStream(path, fileMode, FileAccess.Write))
            {
                using (var ms = new BinaryWriter(fs))
                {
                    ms.Write(System.Environment.NewLine);
                    ms.Write("=====" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "======");
                    ms.Write(System.Environment.NewLine);
                    ms.Write(ex.Message);
                    ms.Write(System.Environment.NewLine);
                }
            }
        }

        private void income_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(StartRecordingActivity));
            intent.PutExtra(FrameElements.CategoryTypeTag, CategoryDataFactory.IncomeString);
            StartActivity(intent);
        }

        private void payment_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(StartRecordingActivity));
            intent.PutExtra(FrameElements.CategoryTypeTag, CategoryDataFactory.PaymentString);
            StartActivity(intent);
        }

        private void reporting_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(MonthSummaryActivity));
            var now = DateTime.Now;
            intent.PutExtra(FrameElements.RecordsYearMonthTag, string.Format(CultureInfo.InvariantCulture, "{0}-{1}", now.Year, now.Month));
            StartActivity(intent);
        }
    }
}

