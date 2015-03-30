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
using Android.Graphics;
using System.Drawing;
using System.Globalization;

namespace EveryRecords
{
    public static class AppExtension
    {
        private static Dictionary<Activity, FrameElements> _frameElementsList = new Dictionary<Activity, FrameElements>();

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

        public static Android.Graphics.Color StringToColor(this Activity activity, string colorString)
        {
            try
            {
                var color = (System.Drawing.Color)new ColorConverter().ConvertFromString(colorString);
                return new Android.Graphics.Color(color.R, color.G, color.B);
            }
            catch
            {
                return Android.Graphics.Color.Black;
            }
        }

        public static FrameElements GetFrameElements(this Activity activity)
        {
            if (_frameElementsList.ContainsKey(activity))
            {
                return _frameElementsList[activity];
            }

            var fe = new FrameElements();
            fe.Title = activity.FindViewById<TextView>(Resource.Id.TitleText);
            fe.SubTitle = activity.FindViewById<TextView>(Resource.Id.SubTitleText);
            fe.BackButton = activity.FindViewById<Button>(Resource.Id.BackButton);
            fe.GraphicButton = activity.FindViewById<Button>(Resource.Id.GraphicButton);
            fe.ShareButton = activity.FindViewById<Button>(Resource.Id.ShareButton);
            fe.List = activity.FindViewById<ListView>(Resource.Id.MainList);
            fe.Grid = activity.FindViewById<GridView>(Resource.Id.MainGrid);

            _frameElementsList.Add(activity, fe);

            return fe;
        }

        public static void RemoveFrameElements(this Activity activity)
        {
            if (_frameElementsList.ContainsKey(activity))
            {
                _frameElementsList.Remove(activity);
            }
        }

        public static string ToShortString(this Version ver){
            var format = "{0}.{1}{2}{3}";
            return string.Format(CultureInfo.InvariantCulture, format, ver.Major, ver.Minor, ver.Build, ver.Revision);
        }

        public static string ToDateString(this DateTime dt)
        {
            return dt.ToString("yyyy/MM/dd");
        }
        
        public static string ToDateTimeString(this DateTime dt)
        {
            return dt.ToString("yyyy/MM/dd-HH:mm:ss");
        }

        public static string ToLabel(this RecordTypes r)
        {
            if (r == RecordTypes.Payment)
            {
                return "支出";
            }

            return "收入";
        }

        public static void BackToLastLevel(ref string path)
        {
            string[] arr = path.Split('/');
            if (arr.Length <= 1)
            {
                path = "";
            }
            else
            {
                path = path.Substring(0, path.LastIndexOf("/"));
            }
        }
    }
}