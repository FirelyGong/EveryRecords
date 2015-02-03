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
    [Activity(Label = "StartRecordingActivity", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
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
            _records = new List<string>();
            var back = FindViewById<Button>(Resource.Id.BackButton);
            back.Click += delegate
            {
                Finish();
            };

            _recordList = FindViewById<ListView>(Resource.Id.RecordList);
            _recordList.Adapter = new SimpleListAdapter(this, new List<string>(new []{"请添加记录"}));
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
                    _recordList.Adapter = new SimpleListAdapter(this, _records);
                }
            }

            if (requestCode == 1)
            {
                if (resultCode == Result.Ok)
                {
                    var item = data.GetStringExtra(ConfirmActivity.DataTag);
                    _records.Remove(item);
                    _recordList.Adapter = new SimpleListAdapter(this, _records);
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
            intent.PutExtra(ConfirmActivity.MessageTag, string.Format(CultureInfo.InvariantCulture, "确定要删除[{0}]吗？", item));
            intent.PutExtra(ConfirmActivity.DataTag, item);
            intent.PutExtra(ConfirmActivity.ReturnToTag, GetType().FullName);
            StartActivityForResult(intent, 1);
        }
    }
}