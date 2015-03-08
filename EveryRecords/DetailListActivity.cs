using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using EveryRecords.DataFactories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace EveryRecords
{
    [Activity(Label = "DetailListActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class DetailListActivity:Activity
    {
        public const string RecordsYearMonthTag = "RecordsYearMonth";
        public const string RecordsCategoryTag = "RecordsCategory";
        public const string RecordsPathTag = "RecordsPath";
        private const string NoRecord = "没有记录！";
        private ListView _reportingList;
        private RecordingDataFactory _reocrdingData;
        private bool _recordsChanged;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.DetailListLayout);

            this.InitialActivity(() => OnBackPressed());

            var yearMonth = Intent.GetStringExtra(RecordsYearMonthTag);
            var category = Intent.GetStringExtra(RecordsCategoryTag);
            var recordsPath = Intent.GetStringExtra(RecordsPathTag);
            int year;
            int month;
            if (string.IsNullOrEmpty(yearMonth))
            {
                year = DateTime.Now.Year;
                month = DateTime.Now.Month;
                _reocrdingData = RecordingDataFactory.Instance;
            }
            else
            {
                string[] arr = yearMonth.Split('-');
                int.TryParse(arr[0], out year);
                int.TryParse(arr[1], out month);
                if (year == DateTime.Now.Year && month == DateTime.Now.Month)
                {
                    _reocrdingData = RecordingDataFactory.Instance;
                }
                else
                {
                    _reocrdingData = RecordingDataFactory.CreateHistoryData(year, month);
                    _reocrdingData.LoadData();
                }
            }

            var title = FindViewById<TextView>(Resource.Id.TitleText);
            title.Text = string.Format(CultureInfo.InvariantCulture, "{0}年{1}月", year, month);

            var subTitle = FindViewById<TextView>(Resource.Id.SubTitleText);
            subTitle.Text = recordsPath;
            _reportingList = FindViewById<ListView>(Resource.Id.ReportsList);
            _reportingList.ItemLongClick += reportingList_ItemLongClick;
            DisplayCategory(category);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 0)
            {
                if (resultCode == Result.Ok)
                {
                    var item = data.GetStringExtra(ConfirmActivity.DataTag);
                    var bln = _reocrdingData.DeleteRecord(item);
                    if (bln)
                    {
                        _reocrdingData.SaveData();
                        var list = ((SimpleListAdapter)_reportingList.Adapter).GetSource();
                        list.Remove(item);
                        _reportingList.Adapter = new SimpleListAdapter(this, list);
                        _recordsChanged = true;
                    }
                    else
                    {
                        Toast.MakeText(this, "删除失败", ToastLength.Long).Show();
                    }
                }
            }
        }

        public override void OnBackPressed()
        {
            var intent = new Intent(this, typeof(ReportingActivity));
            if (_recordsChanged)
            {
                SetResult(Result.Ok, intent);
            }
            else
            {
                SetResult(Result.Canceled, intent);
            }

            base.OnBackPressed();
            Finish();
        }
        
        private void reportingList_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            var item = ((ListView)sender).Adapter.GetItem(e.Position).ToString();
            if (item == NoRecord)
            {
                return;
            }

            if (SettingDataFactory.Instance.AllowDeleteRecord)
            {
                var intent = new Intent(this, typeof(ConfirmActivity));
                intent.PutExtra(ConfirmActivity.MessageTag, string.Format(CultureInfo.InvariantCulture, "确定要删除[{0}]吗？", item));
                intent.PutExtra(ConfirmActivity.DataTag, item);
                intent.PutExtra(ConfirmActivity.ReturnToTag, GetType().FullName);
                StartActivityForResult(intent, 0);
            }
            else
            {
                Toast.MakeText(this, "当前设置不允许删除", ToastLength.Long).Show();
            }
        }

        private void DisplayCategory(string parent)
        {
            string[] datas = new string[] { };
            if (!string.IsNullOrEmpty(parent))
            {
                datas = _reocrdingData.GetRecords(parent).ToArray();               
            }

            if (datas.Length == 0)
            {
                datas = new string[] { NoRecord };
            }
            _reportingList.Adapter = new SimpleListAdapter(this, datas.ToList());
        }

        private string FormatListItem(string category, double amount)
        {
            var item = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", category, amount);

            return item;
        }
    }
}
