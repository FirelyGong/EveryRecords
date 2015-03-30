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
    [Activity(Label = "请选择分类", Theme = "@style/PopupTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class CategorySelectionActivity : Activity
    {
        public const string ParentCategoryTag = "DataCategory";
        public const string SelectionTag = "Selection";

        private string _categoryType;
        private GridView _mainGrid;
        private string _path;
        private TextView _title;
        private IList<string> _categories;
        private IList<string> _results;
        private int _currentCategoryIndex;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.CategorySelectionLayout);
            _results = new List<string>();
            _categoryType = Intent.GetStringExtra(FrameElements.CategoryTypeTag);
            if (string.IsNullOrEmpty(_categoryType))
            {
                _categoryType = CategoryDataFactory.PaymentString;
            }

            _categories = Intent.GetStringExtra(ParentCategoryTag).Split(';');
            _title = FindViewById<TextView>(Resource.Id.TitleText);
            _title.Text = "请选择分类";

            _mainGrid = FindViewById<GridView>(Resource.Id.SelectionList);
            _mainGrid.ItemClick += list_ItemClick;
            var lastLevel = FindViewById<Button>(Resource.Id.LastLevelButton);
            lastLevel.Click += lastLevel_Click;
            TryContinueCategory();
        }

        private void list_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = ((GridView)sender).Adapter.GetItem(e.Position).ToString();
            _path += ("/" + item);
            var sub = GetSubCategories(_path);
            if (sub.Count > 0)
            {
                _title.Text = _path;
                _mainGrid.Adapter = new GridListAdapter(this, sub.ToList());
            }
            else
            {
                _results.Add(_path);
                TryContinueCategory();
            }
        }

        private void TryContinueCategory()
        {
            if (_currentCategoryIndex >= _categories.Count)
            {
                _currentCategoryIndex = 0;

                var lastActivity = Intent.GetStringExtra(FrameElements.ReturnToTag);
                var intent = new Intent(this, Type.GetType(lastActivity, true));
                intent.PutExtra(SelectionTag, string.Join(";", _results));
                SetResult(Result.Ok, intent);
                Finish();
            }
            else
            {
                var item = _categories[_currentCategoryIndex];
                _path = item;
                _currentCategoryIndex++;

                var datas = GetSubCategories(item);
                _mainGrid.Adapter = new GridListAdapter(this, datas.ToList());
            }
        }

        private IList<string> GetSubCategories(string parent)
        {
            if (_categoryType == RecordTypes.Payment.ToLabel())
            {
                return CategoryDataFactory.Payment.GetSubCategories(parent);
            }

            return CategoryDataFactory.Income.GetSubCategories(parent);
        }

        private void lastLevel_Click(object sender, EventArgs e)
        {
            if (!_path.Contains("/"))
            {
                var lastActivity = Intent.GetStringExtra(FrameElements.ReturnToTag);
                var intent = new Intent(this, Type.GetType(lastActivity, true));
                intent.PutExtra(SelectionTag, "");
                SetResult(Result.Canceled, intent);
                Finish();
            }
            else
            {
                AppExtension.BackToLastLevel(ref _path);

                _title.Text = _path;
                var sub = GetSubCategories(_path);
                if (sub.Count > 0)
                {
                    _mainGrid.Adapter = new GridListAdapter(this, sub.ToList());
                }
            }
        }
    }
}