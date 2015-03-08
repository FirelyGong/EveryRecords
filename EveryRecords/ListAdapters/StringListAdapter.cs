using Android.App;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace EveryRecords.ListAdapters
{
    public class StringListAdapter : BaseAdapter<string>
    {
        /// <summary>
        /// 所有UserInof 的数据
        /// </summary>
        protected IList<string> _items;

        protected Activity _context;
        
        public event Action<int, string> ItemDeleted = delegate { };

        public StringListAdapter(Activity context, IList<string> items)
            :base()
        {
            _context = context;
            _items = items;
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

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            return CreateItemView(position, convertView, parent);
        }

        protected virtual View CreateItemView(int position, View convertView, ViewGroup parent)
        {
            return convertView;
        }
    }
}
