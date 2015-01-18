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
    [Activity(Label = "RecordingActivity")]
    public class RecordingActivity : Activity
    {
        private IList<string> _categories;
        private int _currentCategoryIndex;

        private TextView _pathText;
        private EditText _amountText;
        private EditText _commentsText;
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
                        
            // Create your application here
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.RecordingLayout);

            _categories = CategoryDataFactory.Instance.GetCategories();
            _pathText = FindViewById<TextView>(Resource.Id.PathString);
            _pathText.Text = CategoryDataFactory.RootCategory;
            _amountText = FindViewById<EditText>(Resource.Id.AmountText);
            _commentsText = FindViewById<EditText>(Resource.Id.CommentsText);

            var save = FindViewById<Button>(Resource.Id.SaveButton);
            save.Click += save_Click;

            var back = FindViewById<Button>(Resource.Id.BackButton);
            back.Click += delegate
            {
                Finish();
            };

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
                return;
            }

            var selection=data.GetStringExtra("Selection");
            _pathText.Text = _pathText.Text+"/"+selection;

            var sub = CategoryDataFactory.Instance.GetSubCategories(selection);
            if (sub.Count>0)
            {
                SelectDataFromCategory(selection);
            }
            else
            {
                TryContinueCategory();
            }
        }

        private void save_Click(object sender, EventArgs e)
        {
            int amount;
            if(!int.TryParse(_amountText.Text, out amount) || amount==0)
            {
                Toast.MakeText(this,"请输入正确的金额！",ToastLength.Long).Show();
                return;
            }

            RecordingDataFactory.Instance.AddRecord(_pathText.Text, _commentsText.Text, amount);
            Finish();
        }

        private void SelectDataFromCategory(string category)
        {
            var intent = new Intent(this, typeof(SelectionActivity));
            intent.PutExtra("ReturnTo", this.GetType().FullName);
            intent.PutExtra("DataCategory", category);
            StartActivityForResult(intent, 0);
        }

        private void TryContinueCategory()
        {
            if(_currentCategoryIndex>=_categories.Count)
            {
                _currentCategoryIndex = 0;
            }
            else
            {
                var item = _categories[_currentCategoryIndex];
                SelectDataFromCategory(item);
                _currentCategoryIndex++;
            }
        }
    }
}