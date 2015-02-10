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
    [Activity(Label = "HistoryActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class HistoryActivity : Activity
    {
        private ListView _historyList;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.HistoryLayout);

            this.InitialActivity(() => Finish());
            _historyList = FindViewById<ListView>(Resource.Id.HistoryList);
            _historyList.ItemClick += historyList_ItemClick;
            _historyList.ItemLongClick += historyList_ItemLongClick;
        }

        protected override void OnResume()
        {
            base.OnResume();

            _historyList.Adapter = new SimpleListAdapter(this, HistoricDataFactory.Instance.GetHistoryList());
        }

        protected override void OnPause()
        {
            base.OnPause();

            HistoricDataFactory.Instance.SaveData();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 0)
            {
                if (resultCode == Result.Ok)
                {
                    var item = data.GetStringExtra(ConfirmActivity.DataTag);
                    var bln = false;// RecordingDataFactory.Instance.DeleteHistory(item);
                    if (bln)
                    {
                        var list = ((SimpleListAdapter)_historyList.Adapter).GetSource();
                        list.Remove(item);
                        _historyList.Adapter = new SimpleListAdapter(this, list);
                    }
                    else
                    {
                        Toast.MakeText(this, "删除失败", ToastLength.Long).Show();
                    }
                }
            }
        }

        private void historyList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = _historyList.Adapter.GetItem(e.Position).ToString();
            if (item == HistoricDataFactory.NoHistoryList)
            {
                return;
            }
            
            var intent = new Intent(this, typeof(ReportingActivity));
            intent.PutExtra(ReportingActivity.RecordsYearMonthTag, item.Split(':')[0]);

            StartActivity(intent);
        }

        private void historyList_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            var item = ((ListView)sender).Adapter.GetItem(e.Position).ToString();
            if (item == HistoricDataFactory.NoHistoryList)
            {
                Toast.MakeText(this, "没有记录可以删除", ToastLength.Long).Show();
                return;
            }

            if (SettingDataFactory.Instance.AllowDeleteHistory)
            {
                var intent = new Intent(this, typeof(ConfirmActivity));
                intent.PutExtra(ConfirmActivity.MessageTag, string.Format(CultureInfo.InvariantCulture, "确定要删除历史记录[{0}]吗？", item));
                intent.PutExtra(ConfirmActivity.DataTag, item.Split(':')[0]);
                intent.PutExtra(ConfirmActivity.ReturnToTag, GetType().FullName);
                StartActivityForResult(intent, 0);
            }
            else
            {
                Toast.MakeText(this, "当前设置不允许删除", ToastLength.Long).Show();
            }
        }
    }
}