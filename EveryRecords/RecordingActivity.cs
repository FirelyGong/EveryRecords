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
using EveryRecords.DataFactories;
using EveryRecords.ListAdapters;

namespace EveryRecords
{
    [Activity(Label = "RecordingActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class RecordingActivity : Activity
    {
        private IList<string> _paths;
        private string _recordType;

        private TextView _amountLabel;
        private EditText _amountTextBox;
        private EditText _commentsText;
        private TextView _dateLabel;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
                        
            // Create your application here
            SetContentView(Resource.Layout.RecordingLayout);

            this.InitialActivity(() => Finish());
            _recordType = Intent.GetStringExtra(FrameElements.CategoryTypeTag);
            if (string.IsNullOrEmpty(_recordType))
            {
                _recordType = CategoryDataFactory.PaymentString;
            }

            _paths = new List<string>();
            _amountTextBox = FindViewById<EditText>(Resource.Id.AmountTextBox);
            _amountLabel = FindViewById<TextView>(Resource.Id.AmountLabel);
            _commentsText = FindViewById<EditText>(Resource.Id.CommentsText);
            _dateLabel = FindViewById<TextView>(Resource.Id.DateLabel);
            _dateLabel.Text = DateTime.Now.ToDateString();

            var save = FindViewById<Button>(Resource.Id.SaveButton);
            save.Click += save_Click;

            var addAmount = FindViewById<Button>(Resource.Id.AddAmountButton);
            addAmount.Click += addAmount_Click;

            var addDate = FindViewById<Button>(Resource.Id.AddDateButton);

            addDate.Click += addDate_Click;
            var minusDate = FindViewById<Button>(Resource.Id.MinusDateButton);
            minusDate.Click += minusDate_Click;
        }

        protected override void OnStart()
        {
            base.OnStart();

            var categories = CategoryDataFactory.GetInstance(_recordType).GetCategories();
            SelectDataFromCategories(categories);
            //TryContinueCategory();  
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode != Result.Ok)
            {
                Finish();
                return;
            }

            _paths = data.GetStringExtra(CategorySelectionActivity.SelectionTag).Split(';');
            var list = FindViewById<ListView>(Resource.Id.CategoryList);
            list.Adapter = new SimpleListAdapter(this, _paths);
        }

        private void save_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_amountTextBox.Text))
            {
                addAmount_Click(null, EventArgs.Empty);
            }

            double amount = double.Parse(_amountLabel.Text);
            if (amount == 0)
            {
                Toast.MakeText(this, "请输入正确的金额！", ToastLength.Long).Show();
                return;
            }

            DateTime dt = DateTime.Now;
            DateTime.TryParse(_dateLabel.Text, out dt);
            var recording = RecordingDataFactory.Instance.AddRecord(_paths, dt, _commentsText.Text, amount);
            var lastActivity = Intent.GetStringExtra(FrameElements.ReturnToTag);
            var intent = new Intent(this, Type.GetType(lastActivity, true));
            intent.PutExtra(FrameElements.OutputRecordTag, recording);
            SetResult(Result.Ok, intent);
            Finish();
        }

        private void addAmount_Click(object sender, EventArgs e)
        {
            double amt = 0;
            if(!double.TryParse(_amountTextBox.Text, out amt))
            {
                Toast.MakeText(this, "请输入正确的金额！", ToastLength.Long).Show();
                return;
            }
            else
            {
                amt += double.Parse(_amountLabel.Text);
                _amountLabel.Text = amt.ToString();
                _amountTextBox.Text = "";
            }
        }

        private void SelectDataFromCategories(IList<string> categories)
        {
            var intent = new Intent(this, typeof(CategorySelectionActivity));
            intent.PutExtra(FrameElements.CategoryTypeTag, _recordType);
            intent.PutExtra(FrameElements.ReturnToTag, this.GetType().FullName);
            intent.PutExtra(CategorySelectionActivity.ParentCategoryTag, string.Join(";",categories));
            StartActivityForResult(intent, 0);
        }

        private void minusDate_Click(object sender, EventArgs e)
        {
            DateTime dt;
            DateTime.TryParse(_dateLabel.Text, out dt);
            _dateLabel.Text = dt.AddDays(-1).ToDateString();
        }

        private void addDate_Click(object sender, EventArgs e)
        {
            DateTime dt;
            DateTime.TryParse(_dateLabel.Text, out dt);
            _dateLabel.Text = dt.AddDays(1).ToDateString();
        }
    }
}