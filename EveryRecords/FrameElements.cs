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
    public class FrameElements
    {
        public const string RecordsYearMonthTag = "RecordsYearMonth";
        public const string RecordsCategoryTag = "RecordsCategory";
        public const string CategoryTypeTag = "CategoryType";
        public const string OutputRecordTag = "CreatedRecording";
        public const string ReturnToTag = "ReturnTo";

        public TextView Title { get; set; }

        public TextView SubTitle { get; set; }

        public Button BackButton { get; set; }

        public Button GraphicButton { get; set; }

        public Button ShareButton { get; set; }

        public ListView List { get; set; }

        public GridView Grid { get; set; }
    }
}