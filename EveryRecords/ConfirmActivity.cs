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
    [Activity(Label = "确认", Theme = "@style/PopupTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ConfirmActivity : Activity
    {
        public const string DataTag = "Data";
        public const string MessageTag = "Message";
        public const string ReturnToTag = "ReturnTo";

        private string _data;
        private string _message;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.ConfirmLayout);

            var title = FindViewById<TextView>(Resource.Id.TitleText);
            title.Text = "确认";
            _data = Intent.GetStringExtra(DataTag);
            _message = Intent.GetStringExtra(MessageTag);

            var text = FindViewById<TextView>(Resource.Id.ContentText);
            text.Text = _message;

            var ok = FindViewById<Button>(Resource.Id.OkButton);
            ok.Click += ok_Click;
            var cancel = FindViewById<Button>(Resource.Id.CancelButton);
            cancel.Click += cancel_Click;
        }

        void cancel_Click(object sender, EventArgs e)
        {
            var lastActivity = Intent.GetStringExtra(ReturnToTag);
            var intent = new Intent(this, Type.GetType(lastActivity, true));
            intent.PutExtra(DataTag, _data);
            SetResult(Result.Canceled, intent);
            Finish();
        }

        void ok_Click(object sender, EventArgs e)
        {
            var lastActivity = Intent.GetStringExtra(ReturnToTag);
            var intent = new Intent(this, Type.GetType(lastActivity, true));
            intent.PutExtra(DataTag, _data);
            SetResult(Result.Ok, intent);
            Finish();
        }
    }
}