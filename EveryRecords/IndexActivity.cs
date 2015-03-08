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
    [Activity(Label = "������", MainLauncher = true, Icon = "@drawable/hnl", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
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
            ShowMainActivity();
        }
        
        protected override void OnPause()
        {
            base.OnPause();

            Finish();
        }

        private async void ShowMainActivity(){
            await Task.Run(() =>
            {
                StartActivity(typeof(MainActivity));
            });
        }
    }
}