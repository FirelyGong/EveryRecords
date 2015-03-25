using Android.App;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace EveryRecords.ListAdapters
{
    public class SimpleListAdapter : StringListAdapter
    {
        public SimpleListAdapter(Activity context, IList<string> items)
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
                view = _context.LayoutInflater.Inflate(Resource.Layout.ListviewItemLayout, parent, false);
            }
            var text = view.FindViewById<TextView>(Resource.Id.textView1);
            text.Text = item;
            var border = view.FindViewById<LinearLayout>(Resource.Id.linearLayout2);
            var margin = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
            int marginBotton = 0;
            if (position == Count - 1)
            {
                marginBotton = 2;
            }

            margin.SetMargins(2, 2, 2, marginBotton);
            border.LayoutParameters = margin;

            return view;
        }
    }
}
