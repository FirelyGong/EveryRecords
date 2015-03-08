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
        private ListView _recordList;
        private IList<string> _records;
        private bool _waitingForResult;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.StartRecordingLayout);
            this.InitialActivity(() => Finish());
            _records = new List<string>();

            _recordList = FindViewById<ListView>(Resource.Id.RecordList);
            _recordList.Adapter = new ColumnListAdapter(this, new List<string>(new []{"ÇëÌí¼Ó¼ÇÂ¼"}));
            _recordList.ItemLongClick += recordList_ItemLongClick;
            var add = FindViewById<Button>(Resource.Id.AddButton);
            add.Click += add_Click;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            _waitingForResult = false;
            if (requestCode == 0)
            {
                if (resultCode == Result.Ok)
                {
                    var recording = data.GetStringExtra(RecordingActivity.OutputRecordTag);
                    _records.Add(recording);
                    _recordList.Adapter = new ColumnListAdapter(this, _records);
                }
            }

            if (requestCode == 1)
            {
                if (resultCode == Result.Ok)
                {
                    var item = data.GetStringExtra(ConfirmActivity.DataTag);
                    var bln = RecordingDataFactory.Instance.DeleteRecord(item);
                    if (bln)
                    {
                        _records.Remove(item);
                        _recordList.Adapter = new ColumnListAdapter(this, _records);
                    }
                    else
                    {
                        Toast.MakeText(this, "É¾³ýÊ§°Ü", ToastLength.Long).Show();
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

            if (_recordList.Adapter.Count > 0)
            {
                RecordingDataFactory.Instance.SaveData();
                HistoricDataFactory.Instance.SaveData();
                _records.Clear();
            }
        }
                
        private void add_Click(object sender, EventArgs e)
        {
            _waitingForResult = true;
            var intent = new Intent(this, typeof(RecordingActivity));
            intent.PutExtra(RecordingActivity.ReturnToTag, GetType().FullName);
            StartActivityForResult(intent, 0);
        }

        private void recordList_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            _waitingForResult = true;
            var list = sender as ListView;
            var item = list.Adapter.GetItem(e.Position).ToString();

            var intent = new Intent(this, typeof(ConfirmActivity));
            intent.PutExtra(ConfirmActivity.MessageTag, string.Format(CultureInfo.InvariantCulture, "È·¶¨ÒªÉ¾³ý[{0}]Âð£¿", item));
            intent.PutExtra(ConfirmActivity.DataTag, item);
            intent.PutExtra(ConfirmActivity.ReturnToTag, GetType().FullName);
            StartActivityForResult(intent, 1);
        }
    }
}