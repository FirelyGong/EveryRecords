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
    [Activity(Label = "SettingActivity", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
    public class SettingActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "SettingLayout" layout resource
            SetContentView(Resource.Layout.SettingLayout);

            var back = FindViewById<Button>(Resource.Id.BackButton);
            back.Click += delegate
            {
                Finish();
            };            
        }
    }
}