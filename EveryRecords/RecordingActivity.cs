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
    [Activity(Label = "RecordingActivity", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
    public class RecordingActivity : Activity
    {
        public const string OutputRecordTag = "CreatedRecording";
        public const string ReturnToTag = "ReturnTo";

        private IList<string> _categories;
        private IList<string> _paths;
        private string _currentPath;
        private int _currentCategoryIndex;

        private TextView _amountLabel;
        private EditText _amountTextBox;
        private EditText _commentsText;
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
                        
            // Create your application here
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.RecordingLayout);
            _paths = new List<string>();
            _categories = CategoryDataFactory.Instance.GetCategories();
            _amountTextBox = FindViewById<EditText>(Resource.Id.AmountTextBox);
            _amountLabel = FindViewById<TextView>(Resource.Id.AmountLabel);
            _commentsText = FindViewById<EditText>(Resource.Id.CommentsText);

            var save = FindViewById<Button>(Resource.Id.SaveButton);
            save.Click += save_Click;

            var back = FindViewById<Button>(Resource.Id.BackButton);
            back.Click += delegate
            {
                Finish();
            };

            var addAmount = FindViewById<Button>(Resource.Id.AddAmountButton);
            addAmount.Click += addAmount_Click;
            TryContinueCategory();          
        }

        protected override void OnStart()
        {
            base.OnStart();

            _categories = CategoryDataFactory.Instance.GetCategories();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode != Result.Ok)
            {
                Finish();
                return;
            }

            var selection=data.GetStringExtra(SelectionActivity.SelectionTag);
            _currentPath += "/" + selection;

            var sub = CategoryDataFactory.Instance.GetSubCategories(selection);
            if (sub.Count>0)
            {
                SelectDataFromCategory(selection);
            }
            else
            {
                _paths.Add(_currentPath);
                TryContinueCategory();
            }
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

            var recording = RecordingDataFactory.Instance.AddRecord(_paths, _commentsText.Text, amount);
            var lastActivity = Intent.GetStringExtra(ReturnToTag);
            var intent = new Intent(this, Type.GetType(lastActivity, true));
            intent.PutExtra(OutputRecordTag, recording);
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

        private void SelectDataFromCategory(string category)
        {
            var intent = new Intent(this, typeof(SelectionActivity));
            intent.PutExtra(SelectionActivity.ReturnToTag, this.GetType().FullName);
            intent.PutExtra(SelectionActivity.ParentCategoryTag, category);
            StartActivityForResult(intent, 0);
        }

        private void TryContinueCategory()
        {
            if(_currentCategoryIndex>=_categories.Count)
            {
                _currentCategoryIndex = 0;

                var list = FindViewById<ListView>(Resource.Id.CategoryList);
                list.Adapter = new SimpleListAdapter(this, _paths);
            }
            else
            {
                _currentPath = CategoryDataFactory.RootCategory;
                var item = _categories[_currentCategoryIndex];
                SelectDataFromCategory(item);
                _currentCategoryIndex++;
            }
        }
    }
}