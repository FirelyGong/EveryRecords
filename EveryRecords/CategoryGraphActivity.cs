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
using Android.Content.PM;
using Android.Graphics;
using EveryRecords.DataFactories;
using System.IO;
using EveryRecords.Charts;

namespace EveryRecords
{
    [Activity(Label = "CategoryGraphActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class CategoryGraphActivity : Activity
    {
        public const string RecordsYearMonthTag = "RecordsYearMonth";
        public const string RecordsCategoryTag = "CategoryRecordData";
        public const string RecordsDetailTag = "CategoryDetail";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.CategoryGraphLayout);

            this.InitialActivity(() => Finish());

            var share = FindViewById<Button>(Resource.Id.ShareButton);
            share.Click += share_Click;

            var yearMonth = FindViewById<TextView>(Resource.Id.YearMonthText);
            var ym=Intent.GetStringExtra(RecordsYearMonthTag);
            yearMonth.Text = ym;
            var cm = Intent.GetStringExtra(RecordsCategoryTag);
            var cateText = FindViewById<TextView>(Resource.Id.CategoryInfoText);
            var pieChart = FindViewById<ChartPane>(Resource.Id.PieChart);
            var detail = Intent.GetStringExtra(RecordsDetailTag).Split(';');

            if (detail[0].Contains(":"))
            {
                var data = detail.Select(d => double.Parse(d.Split(':')[1])).ToArray();
                var label = detail.Select(d => d.Split(':')[0]).ToArray();
                pieChart.InitializeChart(ChartType.Pie, data, label);
                cateText.Text = cm + " " + data.Sum() + "元";
            }
            else
            {
                pieChart.Clear();
                cateText.Text = "";
            }
        }

        private void share_Click(object sender, EventArgs e)
        {
            var view = FindViewById<LinearLayout>(Resource.Id.ContentLayout);
            var pic = CreateViewBitmap(view);
            var path = SaveBitmapToPng(pic, "CategoryGraph");
            if (string.IsNullOrEmpty(path))
            {
                Toast.MakeText(this, "生成分享图片失败", ToastLength.Long).Show();
                return;
            }

            Intent intent = new Intent(Intent.ActionSend);
            intent.SetType("image/png");
            intent.PutExtra(Intent.ExtraStream, Android.Net.Uri.FromFile(new Java.IO.File(path)));
            intent.SetFlags(ActivityFlags.NewTask);
            StartActivity(Intent.CreateChooser(intent, "分享图片到"));  
        }

        private Bitmap CreateViewBitmap(View v)
        {
            Bitmap bitmap = Bitmap.CreateBitmap(v.Width, v.Height,
            Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            v.Draw(canvas);
            return bitmap;
        }

        private string SaveBitmapToPng(Bitmap bmp, string fileNameWithoutExt)
        {
            try
            {
                var folder = DataFactory.GetDataFolder();
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                string filePath = System.IO.Path.Combine(folder, fileNameWithoutExt+".png");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                using (var sm = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    bmp.Compress(Bitmap.CompressFormat.Png, 90, sm);
                }

                return filePath;
            }
            catch (Exception e)
            {
                return string.Empty;
            }  
        }
        private string SaveBitmapToJpg(Bitmap bmp, string fileNameWithoutExt)
        {
            try
            {
                var folder = DataFactory.GetDataFolder();
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                string filePath = System.IO.Path.Combine(folder, fileNameWithoutExt + ".jpg");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                using (var sm = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    bmp.Compress(Bitmap.CompressFormat.Jpeg, 90, sm);
                }

                return filePath;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
    }
}