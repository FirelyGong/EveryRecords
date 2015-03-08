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
    public class SelectionActivity : Activity
    {
        public const string ParentCategoryTag = "DataCategory";
        public const string ReturnToTag = "ReturnTo";
        public const string SelectionTag = "Selection";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.SelectionLayout);

            // Create your application here
            var category = Intent.GetStringExtra(ParentCategoryTag);
            var datas = CategoryDataFactory.Instance.GetSubCategories(category);

            var title = FindViewById<TextView>(Resource.Id.TitleText);
            title.Text = "请选择分类";

            var list = FindViewById<GridView>(Resource.Id.SelectionList);
            list.Adapter = new GridListAdapter(this, datas.ToList());
            list.ItemClick += list_ItemClick;
        }

        void list_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var lastActivity = Intent.GetStringExtra(ReturnToTag);
            var intent = new Intent(this, Type.GetType(lastActivity, true));
            intent.PutExtra(SelectionTag, ((GridView)sender).Adapter.GetItem(e.Position).ToString());
            SetResult(Result.Ok, intent);
            Finish();
        }
    }
}