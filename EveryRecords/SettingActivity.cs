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

namespace EveryRecords
{
    [Activity(Label = "SettingActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SettingActivity : Activity
    {
        private CheckBox _allowDeleteRecord;
        private CheckBox _allowDeleteHistory;
        private EditText _expensesLimit;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "SettingLayout" layout resource
            SetContentView(Resource.Layout.SettingLayout);

            this.InitialActivity(() => Finish());
            _allowDeleteHistory = FindViewById<CheckBox>(Resource.Id.AllowDeleteHistoryCheck);
            _allowDeleteHistory.CheckedChange += allowDeleteHistory_CheckedChange;
            _allowDeleteRecord = FindViewById<CheckBox>(Resource.Id.AllowDeleteRecordCheck);
            _allowDeleteRecord.CheckedChange += allowDeleteRecord_CheckedChange;
            _expensesLimit = FindViewById<EditText>(Resource.Id.ExpensesLimitText);
            _expensesLimit.AfterTextChanged += expensesLimit_AfterTextChanged;

            var editCategory1 = FindViewById<TextView>(Resource.Id.EditCategory1);
            editCategory1.Click += editCategory_Click;
            var editCategory2 = FindViewById<TextView>(Resource.Id.EditCategory2);
            editCategory2.Click += editCategory_Click;
        }

        private void editCategory_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(CategoryActivity));
        }

        private void expensesLimit_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            double dbl;
            if(double.TryParse(_expensesLimit.Text, out dbl))
            {
                SettingDataFactory.Instance.ExpensesLimit = dbl;
            }
            else
            {
                Toast.MakeText(this, "请输入正确的支出提醒额", ToastLength.Long).Show();
            }
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
            _expensesLimit.Text = SettingDataFactory.Instance.ExpensesLimit.ToString();
        }

        protected override void OnPause()
        {
            base.OnPause();

            SettingDataFactory.Instance.SaveData();
        }
    }
}