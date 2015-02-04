using Android.App;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveryRecords
{
    public class SimpleListAdapter : BaseAdapter<string>
    {
        /// <summary>
        /// 所有UserInof 的数据
        /// </summary>
        private IList<string> _items;

        private Activity _context;

        private bool _deleteEndaled;

        public event Action<int, string> ItemDeleted = delegate { };

        public SimpleListAdapter(Activity context, IList<string> items)
            : this(context, items, false)
        {
        }

        public SimpleListAdapter(Activity context, IList<string> items, bool enableDel)
            :base()
        {
            _context = context;
            _items = items;
            _deleteEndaled = enableDel;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public IList<string> GetSource()
        {
            return new List<string>(_items);
        }

        public override string this[int position]
        {
            get { return _items[position]; }
        }

        public override int Count
        {
            get { return _items.Count; }
        }

        /// <summary>
        /// 系统会呼叫 并且render.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="convertView"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = _items[position];
            var view = convertView;
            if (view == null)
            {
                //使用自订的UserListItemLayout
                view = _context.LayoutInflater.Inflate(Resource.Layout.ListviewItemLayout, null);
            }

            var text = view.FindViewById<TextView>(Resource.Id.textView1);
            text.Text = item;
            var border = view.FindViewById<LinearLayout>(Resource.Id.linearLayout2);
            var margin = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
            int marginTop = 0;
            int marginBotton = 0;
            if (position == Count - 1)
            {
                marginBotton = 2;
            }            
            if (position == 0)
            {
                marginTop = 2;
            }

            margin.SetMargins(2, marginTop, 2, marginBotton);
            border.LayoutParameters = margin;

            return view;
        }
    }
}
