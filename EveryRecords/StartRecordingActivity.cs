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
using EveryRecords.ListAdapters;

namespace EveryRecords
{
    [Activity(Label = "StartRecordingActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class StartRecordingActivity : Activity
    {
        private bool _needRefreshList;
        private bool _waitingForResult;
        private string _recordType;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.StartRecordingLayout);
            this.InitialActivity(() => Finish());
            _needRefreshList = true;
            _recordType = Intent.GetStringExtra(FrameElements.CategoryTypeTag);
            if (string.IsNullOrEmpty(_recordType))
            {
                _recordType = CategoryDataFactory.PaymentString;
                Elements.Title.Text = CategoryDataFactory.PaymentString;
            }
            else
            {
                Elements.Title.Text = _recordType;
            }
            
            Elements.List.ItemLongClick += recordList_ItemLongClick;
            var add = FindViewById<Button>(Resource.Id.AddButton);
            add.Click += add_Click;
        }

        protected override void OnResume()
        {
            base.OnResume();

            if ((!_waitingForResult) && _needRefreshList)
            {
                var records = RecordingDataFactory.Instance.GetDailyRecords(_recordType, DateTime.Now);
                if (records.Count == 0)
                {
                    records.Add("今天还没有" + _recordType + "记录");
                }

                Elements.List.Adapter = new ColumnListAdapter(this, records);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            _waitingForResult = false;
            if (requestCode == 0)
            {
                _needRefreshList = resultCode == Result.Ok;
            }

            if (requestCode == 1)
            {
                if (resultCode == Result.Ok)
                {
                    var item = data.GetStringExtra(ConfirmActivity.DataTag);
                    var bln = RecordingDataFactory.Instance.DeleteRecord(item);
                    if (bln)
                    {
                        _needRefreshList = true;
                    }
                    else
                    {
                        _needRefreshList = false;
                        Toast.MakeText(this, "删除失败", ToastLength.Long).Show();
                    }
                }
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (_waitingForResult)
            {
                return;
            }

            if (Elements.List.Adapter.Count > 0)
            {
                RecordingDataFactory.Instance.SaveData();
                HistoricDataFactory.Instance.SaveData();
            }

            this.RemoveFrameElements();
        }
                
        private void add_Click(object sender, EventArgs e)
        {
            _waitingForResult = true;
            var intent = new Intent(this, typeof(RecordingActivity));
            intent.PutExtra(FrameElements.ReturnToTag, GetType().FullName);
            intent.PutExtra(FrameElements.CategoryTypeTag, _recordType);
            StartActivityForResult(intent, 0);
        }

        private void recordList_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            _waitingForResult = true;
            var list = sender as ListView;
            var item = list.Adapter.GetItem(e.Position).ToString();

            var intent = new Intent(this, typeof(ConfirmActivity));
            intent.PutExtra(ConfirmActivity.MessageTag, string.Format(CultureInfo.InvariantCulture, "确定要删除[{0}]吗？", item));
            intent.PutExtra(ConfirmActivity.DataTag, item);
            intent.PutExtra(ConfirmActivity.ReturnToTag, GetType().FullName);
            StartActivityForResult(intent, 1);
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