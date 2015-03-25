using Android.App;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace EveryRecords.ListAdapters
{
    public class GridListAdapter : StringListAdapter
    {
        public GridListAdapter(Activity context, IList<string> items)
            : base(context, items)
        {
        }

        /// <summary>
        /// 系统会呼叫 并且render.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="convertView"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        protected override View CreateItemView(int position, View convertView, ViewGroup parent)
        {
            var item = _items[position];
            var view = convertView;
            if (view == null)
            {
                view = _context.LayoutInflater.Inflate(Resource.Layout.GridviewItemLayout, parent, false);
            }

            var strings = item.Split(':');
            var text1 = view.FindViewById<TextView>(Resource.Id.textView1);
            var text2 = view.FindViewById<TextView>(Resource.Id.textView2);
            text1.Text = strings[0];
            if (strings.Length > 1)
            {
                text2.Visibility = ViewStates.Visible;
                text2.Text = strings[1];
            }
            else
            {
                text2.Text = "";
                text2.Visibility = ViewStates.Gone;
            }

            return view;
        }
    }
}
