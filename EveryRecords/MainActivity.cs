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

        private TextView _sumText;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.MainLayout);

            this.InitialActivity(null);
            
            _sumText = FindViewById<TextView>(Resource.Id.SummaryText);
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
            ShowIntroduction();
            UpdateUiInfo();
        }

        protected override void OnResume()
        {
            base.OnResume();

            UpdateUiInfo();
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

        private void UpdateUiInfo()
        {
            UpdatePercentage();
            UpdateRecents();
            _sumText.Text = "当月记录金额:" + RecordingDataFactory.Instance.GetCategorySummary(CategoryDataFactory.RootCategory);
        }
        
        private void UpdatePercentage()
        {
            var percentUI = FindViewById<LinearLayout>(Resource.Id.PercentLayout);
            var parameters = new LinearLayout.LayoutParams(percentUI.LayoutParameters);
            var limit = SettingDataFactory.Instance.ExpensesLimit;
            if (limit <= 0)
            {
                parameters.Weight = 0.95f;
            }
            else
            {
                var amount = RecordingDataFactory.Instance.GetCategorySummary(CategoryDataFactory.RootCategory);
                var percent = amount / limit;
                if (percent > 1)
                {
                    percent = 1;
                }

                parameters.Weight = (float)(1 - percent);
            }

            percentUI.LayoutParameters = parameters;
        }

        private void UpdateRecents()
        {
            var historic = HistoricDataFactory.Instance.GetHistoryList();
            historic.Remove(HistoricDataFactory.NoHistoryList);
            var amounts = historic.Select(h => double.Parse(h.Split(':')[1])).ToList();
            double maxAmount = 0;
            if (amounts.Count > 0)
            {
                maxAmount = amounts.Max();
            }
            else
            {
                return;
            }

            for (int i = amounts.Count; i < 6; i++)
            {
                amounts.Add(0);
            }

            UpdateRecentUi(amounts[0], amounts[0] / maxAmount, FindViewById<TextView>(Resource.Id.Recent1));
            UpdateRecentUi(amounts[1], amounts[1] / maxAmount, FindViewById<TextView>(Resource.Id.Recent2));
            UpdateRecentUi(amounts[2], amounts[2] / maxAmount, FindViewById<TextView>(Resource.Id.Recent3));
            UpdateRecentUi(amounts[3], amounts[3] / maxAmount, FindViewById<TextView>(Resource.Id.Recent4));
            UpdateRecentUi(amounts[4], amounts[4] / maxAmount, FindViewById<TextView>(Resource.Id.Recent5));
            UpdateRecentUi(amounts[5], amounts[5] / maxAmount, FindViewById<TextView>(Resource.Id.Recent6));
        }

        private void UpdateRecentUi(double amount, double percent, TextView recentText)
        {
            recentText.Text = ((int)amount).ToString();
            var parameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            parameters.Weight = (float)percent;
            parameters.RightMargin = 10;
            parameters.LeftMargin = 10;
            parameters.Gravity = GravityFlags.Top | GravityFlags.CenterHorizontal;
            recentText.LayoutParameters = parameters;
        }
    }
}

