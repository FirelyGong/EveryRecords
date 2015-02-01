using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Globalization;

namespace EveryRecords
{
    [Activity(Label = "EveryRecords", MainLauncher = true, Icon = "@drawable/EveryRecords", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
    public class MainActivity : Activity
    {
        private TextView _title;
        private Button _recording;
        private Button _reporting;
        private Button _setting;
        private Button _exit;
        private Button _history;

        private TextView _sumText;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            LoadData();

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
                var now=DateTime.Now;
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
            ver.Text = GetType().Assembly.GetName().Version.ToString();
            ver.Click += delegate
            {
                StartActivity(typeof(AboutActivity));
            };
        }
        
        protected override void OnResume()
        {
            base.OnResume();

            _sumText.Text = "当月记录金额：" + RecordingDataFactory.Instance.GetCategorySummary(CategoryDataFactory.RootCategory);
        }

        private void LoadData()
        {
            CategoryDataFactory.Instance.LoadData();
            RecordingDataFactory.Instance.LoadData();
        }
    }
}

