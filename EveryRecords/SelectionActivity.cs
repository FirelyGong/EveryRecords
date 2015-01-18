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
    [Activity(Label = "SelectionActivity")]
    public class SelectionActivity : ListActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            var category = Intent.GetStringExtra("DataCategory");
            var datas = CategoryDataFactory.Instance.GetSubCategories(category);

            ListAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1, datas);
            
        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            base.OnListItemClick(l, v, position, id);

            var lastActivity=Intent.GetStringExtra("ReturnTo");
            var intent = new Intent(this, Type.GetType(lastActivity, true));
            intent.PutExtra("Selection", l.Adapter.GetItem(position).ToString());
            SetResult(Result.Ok, intent);
            Finish();
        }
    }
}