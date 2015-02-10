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
    public static class ActivityExtensionMethods
    {
        public static void InitialActivity(this Activity context, Action backAction)
        {
            int sdkVer = 0;
            if (int.TryParse(Android.OS.Build.VERSION.Sdk, out sdkVer))
            {
                if (sdkVer >= 19)
                {
                    var title = context.FindViewById<LinearLayout>(Resource.Id.TitleInnerLayout);
                    if (title != null)
                    {
                        var margin = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
                        margin.TopMargin = (int)(25 * context.Resources.DisplayMetrics.Density);
                        title.LayoutParameters = margin;
                    }
                }
            }

            var back = context.FindViewById<Button>(Resource.Id.BackButton);
            if (back != null && backAction!=null)
            {
                back.Click += delegate
                {
                    backAction();
                };
            }
        }
    }
}