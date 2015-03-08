﻿using Android.App;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace EveryRecords.ListAdapters
{
    public class ColumnListAdapter : StringListAdapter
    {
        public ColumnListAdapter(Activity context, IList<string> items)
            :base(context, items)
        {
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
                var strings = item.Split(':');
                view = _context.LayoutInflater.Inflate(Resource.Layout.ListviewColumnItemLayout, parent, false);
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
            }

            return view;
        }
    }
}
