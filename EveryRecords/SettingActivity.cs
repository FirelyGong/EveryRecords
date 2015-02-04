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
    [Activity(Label = "SettingActivity", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
    public class SettingActivity : Activity
    {
        private CheckBox _allowDeleteRecord;
        private CheckBox _allowDeleteHistory;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "SettingLayout" layout resource
            SetContentView(Resource.Layout.SettingLayout);

            _allowDeleteHistory = FindViewById<CheckBox>(Resource.Id.AllowDeleteHistoryCheck);
            _allowDeleteHistory.CheckedChange += allowDeleteHistory_CheckedChange;
            _allowDeleteRecord = FindViewById<CheckBox>(Resource.Id.AllowDeleteRecordCheck);
            _allowDeleteRecord.CheckedChange += allowDeleteRecord_CheckedChange;

            var back = FindViewById<Button>(Resource.Id.BackButton);
            back.Click += delegate
            {
                Finish();
            };            
        }

        private void allowDeleteRecord_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            SettingDataFactory.Instance.AllowDeleteRecord = _allowDeleteRecord.Checked;
        }

        private void allowDeleteHistory_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            SettingDataFactory.Instance.AllowDeleteHistory = _allowDeleteHistory.Checked;
        }

        protected override void OnResume()
        {
            base.OnResume();

            _allowDeleteRecord.Checked = SettingDataFactory.Instance.AllowDeleteRecord;
            _allowDeleteHistory.Checked = SettingDataFactory.Instance.AllowDeleteHistory;
        }

        protected override void OnPause()
        {
            base.OnPause();

            SettingDataFactory.Instance.SaveData();
        }
    }
}