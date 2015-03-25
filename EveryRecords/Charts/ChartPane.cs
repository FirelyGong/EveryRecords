using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Content.Res;

namespace EveryRecords.Charts
{
    public class ChartPane : TextView
    {
        protected ChartPane(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public ChartPane(Context context)
            : base(context)
        {
        }

        public ChartPane(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        public ChartPane(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
 
        }

        public double[] Data
        {
            get
            {
                if (Chart == null)
                {
                    return new double[] { };
                }

                return Chart.Data;
            }
        }

        public string[] Label
        {
            get
            {
                if (Chart == null)
                {
                    return new string[] { };
                }

                return Chart.Label;
            }
        }

        public Color ChartColor
        {
            get
            {
                if (Chart == null)
                {
                    return Color.Black;
                }

                return Chart.ChartColor;
            }
        }

        public Color LabelColor
        {
            get
            {
                if (Chart == null)
                {
                    return Color.Black;
                }

                return Chart.LabelColor;
            }
        }

        public IChart Chart
        {
            get;
            private set;
        }

        public void InitializeChart(IChart chart)
        {
            Chart = chart;
            Invalidate();
        }

        //public void InitializeChart(double[] data, string[] label, Color chartColor)
        //{
        //    InitializeChart(chart, data, label, chartColor, chartColor);
        //}

        //public void InitializeChart(double[] data, string[] label, Color chartColor, Color labelColor)
        //{
        //    Chart = chart;
        //    _chartDrawer = ConstructChart(chart);
        //    _chartDrawer.Data = data;
        //    _chartDrawer.Label = label;
        //    _chartDrawer.ChartColor = chartColor;
        //    _chartDrawer.LabelColor = labelColor;
        //    Invalidate();
        //}

        public void Clear()
        {
            Chart = null;
            Invalidate();
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            if (Chart != null)
            {
                Chart.InitSize(Width, Height, TextSize);
                Chart.Draw(canvas);
            }
        }

        //public static IChart ConstructChart(ChartType chart)
        //{
        //    switch (chart)
        //    {
        //        case ChartType.Histogram:
        //            return new Histogram();
        //        case ChartType.Pie:
        //            return new PieChart();
        //        case ChartType.Progress:
        //            return new ProgressChart();
        //    }

        //    return null;
        //}
    }
}