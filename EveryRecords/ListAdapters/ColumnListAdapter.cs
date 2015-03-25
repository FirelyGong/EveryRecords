using Android.App;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace EveryRecords.ListAdapters
{
    public class ColumnListAdapter : StringListAdapter
    {
        private bool _useSmallItem;

        public ColumnListAdapter(Activity context, IList<string> items)
            :this(context, items, false)
        {
            _context = context;
            _items = items;
        }
        public ColumnListAdapter(Activity context, IList<string> items, bool useSmallItem)
            : base(context, items)
        {
            _useSmallItem = useSmallItem;
            _context = context;
            _items = items;
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
                if (_useSmallItem)
                {
                    view = _context.LayoutInflater.Inflate(Resource.Layout.ListviewColumnSmallItemLayout, parent, false);
                }
                else
                {
                    view = _context.LayoutInflater.Inflate(Resource.Layout.ListviewColumnItemLayout, parent, false);
                }
            }
            var strings = item.Split(':');

            var text = view.FindViewById<TextView>(Resource.Id.textView1);
            var text2 = view.FindViewById<TextView>(Resource.Id.textView2);
            text.Text = strings[0];
            if (strings.Length > 1)
            {
                text2.Text = strings[1];
            }
            else
            {
                text2.Text = "";
            }

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
