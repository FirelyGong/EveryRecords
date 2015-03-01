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
using System.Threading.Tasks;
using EveryRecords.DataFactories;

namespace EveryRecords
{
    [Activity(Label = "»¨ÄÄÁË", MainLauncher = true, Icon = "@drawable/hnl", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class IndexActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.IndexLayout);
        }

        protected override void OnStart()
        {
            base.OnStart();
            PrepareMainActivity();
        }
        
        protected override void OnPause()
        {
            base.OnPause();

            Finish();
        }

        private async void PrepareMainActivity()
        {
            var bln = await LoadDataAsync();

            StartActivity(typeof(MainActivity));
        }

        private Task<bool> LoadDataAsync()
        {
            return Task.Run(() =>
            {
                LoadData();
                return true;
            });
        }

        private void LoadData()
        {
            CategoryDataFactory.Instance.LoadData();
            RecordingDataFactory.Instance.LoadData();
            SettingDataFactory.Instance.LoadData();
            HistoricDataFactory.Instance.LoadData();
        }
    }
}