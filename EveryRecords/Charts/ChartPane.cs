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

namespace EveryRecords.Charts
{
    public class ChartPane : TextView
    {
        private IChart _chartDrawer;

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
                if (_chartDrawer == null)
                {
                    return new double[] { };
                }

                return _chartDrawer.Data;
            }
        }

        public string[] Label
        {
            get
            {
                if (_chartDrawer == null)
                {
                    return new string[] { };
                }

                return _chartDrawer.Label;
            }
        }

        public Color ChartColor
        {
            get
            {
                if (_chartDrawer == null)
                {
                    return Color.Black;
                }

                return _chartDrawer.ChartColor;
            }
        }

        public Color LabelColor
        {
            get
            {
                if (_chartDrawer == null)
                {
                    return Color.Black;
                }

                return _chartDrawer.LabelColor;
            }
        }

        public ChartType Chart { get; private set; }

        public void InitializeChart(ChartType chart, double[] data, string[] label)
        {
            Chart = chart;
            _chartDrawer = ConstructChart(chart);
            _chartDrawer.Data = data;
            _chartDrawer.Label = label;
            Invalidate();
        }

        public void InitializeChart(ChartType chart, double[] data, string[] label, Color chartColor)
        {
            InitializeChart(chart, data, label, chartColor, chartColor);
        }

        public void InitializeChart(ChartType chart, double[] data, string[] label, Color chartColor, Color labelColor)
        {
            Chart = chart;
            _chartDrawer = ConstructChart(chart);
            _chartDrawer.Data = data;
            _chartDrawer.Label = label;
            _chartDrawer.ChartColor = chartColor;
            _chartDrawer.LabelColor = labelColor;
            Invalidate();
        }

        public void Clear()
        {
            _chartDrawer = null;
            Invalidate();
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            if (_chartDrawer != null)
            {
                _chartDrawer.InitSize(Width, Height, TextSize);
                _chartDrawer.Draw(canvas);
            }
        }

        private IChart ConstructChart(ChartType chart)
        {
            switch (chart)
            {
                case ChartType.Histogram:
                    return new Histogram();
                case ChartType.PieChart:
                    return new PieChart();
                case ChartType.PercentageChart:
                    return new PercentageChart();
            }

            return null;
        }
    }
}