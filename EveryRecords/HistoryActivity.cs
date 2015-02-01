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

namespace EveryRecords
{
    [Activity(Label = "HistoryActivity", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
    public class HistoryActivity : Activity
    {
        private ListView _historyList;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.HistoryLayout);

            _historyList = FindViewById<ListView>(Resource.Id.HistoryList);
            _historyList.ItemClick += historyList_ItemClick;
            var back = FindViewById<Button>(Resource.Id.BackButton);
            back.Click += delegate
            {
                Finish();
            };
        }

        protected override void OnResume()
        {
            base.OnResume();

            _historyList.Adapter = new SimpleListAdapter(this, RecordingDataFactory.Instance.GetHistoryList());
        }

        private void historyList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = _historyList.Adapter.GetItem(e.Position).ToString();
            if (item == RecordingDataFactory.NoHistoryList)
            {
                return;
            }
            
            var intent = new Intent(this, typeof(ReportingActivity));
            intent.PutExtra(ReportingActivity.RecordsYearMonthTag, item);

            StartActivity(intent);
        }
    }
}