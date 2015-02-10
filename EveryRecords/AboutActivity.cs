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
    [Activity(Label = "AboutActivity", ScreenOrientation=Android.Content.PM.ScreenOrientation.Portrait)]
    public class AboutActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.AboutLayout);

            this.InitialActivity(() => Finish());

            var introduce = FindViewById<Button>(Resource.Id.IntroduceButton);
            introduce.Click += delegate { StartActivity(typeof(IntroductionActivity)); };
        }
    }
}