using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace EveryRecords
{
    [Activity(Label = "EveryRecords", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button recording = FindViewById<Button>(Resource.Id.RecordButton);

            recording.Click += delegate {
                StartActivity(typeof(RecordingActivity));
            };

            var reporting = FindViewById<Button>(Resource.Id.ViewReportButton);
            reporting.Click += delegate
            {
                StartActivity(typeof(ReportingActivity));
            };

            var setting = FindViewById<Button>(Resource.Id.SettingButton);
            setting.Click += delegate
            {
                StartActivity(typeof(SettingActivity));
            };

            var exit = FindViewById<Button>(Resource.Id.ExitButton);
            exit.Click += delegate
            {
                Finish();
            };
        }
    }
}

