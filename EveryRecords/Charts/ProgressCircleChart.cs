using Android.Content.Res;
using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveryRecords.Charts
{
    public class ProgressCircleChart:IChart
    {
        public ProgressCircleChart(double value, double amount, string[] label, Color chartColor, Color labelColor)
        {
            Data=new []{value, amount};
            Label = label;
            ChartColor = chartColor;
            LabelColor = labelColor;
        }

        public void InitSize(float width, float height, float textSize)
        {
            Width = width;
            Height = height;
            TextSize = textSize;
        }

        public void Draw(Canvas canvas)
        {
            var paint = new Paint { AntiAlias = true };
            if (Data.Length <= 0)
            {
                return;
            }

            paint.Color=ChartColor;
            paint.TextSize = TextSize;
            var textHeight = paint.GetFontMetrics().Bottom - paint.GetFontMetrics().Ascent;

            var centerX = Width / 2;
            var centerY = Height / 2;
            var rectWidth = Math.Min(Width, Height - 2 * textHeight);
            var innerRectWidth = rectWidth - 20;
            var rect = new RectF(centerX - rectWidth / 2, centerY - rectWidth / 2, centerX + rectWidth / 2, centerY + rectWidth / 2);
            var innerRect = new RectF(centerX - innerRectWidth / 2, centerY - innerRectWidth / 2, centerX + innerRectWidth / 2, centerY + innerRectWidth / 2);
            DrawCircle(canvas, innerRect, 4, paint, 0, 360);

            var angle = Data[1] / Data[0] * 360;
            DrawCircle(canvas, rect, 10, paint, -90, (float)angle);

            paint.Color = LabelColor;
            var labelsHeight = textHeight * Label.Length;
            var labelTop=(Height-textHeight-labelsHeight)/2;
            for (int i = 0; i < Label.Length; i++)
            {
                var textWidth = paint.MeasureText(Label[i]);
                canvas.DrawText(Label[i],(Width - textWidth) / 2, labelTop + (i+1) * textHeight,  paint);
            }
        }

        public float Width
        {
            get;
            private set;
        }

        public float Height
        {
            get;
            private set;
        }

        public float TextSize
        {
            get;
            private set;
        }

        public double[] Data
        {
            get;
            set;
        }

        public string[] Label
        {
            get;
            set;
        }

        public Color ChartColor
        {
            get;
            set;
        }

        public Color LabelColor
        {
            get;
            set;
        }

        private void DrawCircle(Canvas canvas, RectF rect, float strokeWidth, Paint oldPaint, float start, float angle)
        {
            var paint = new Paint(oldPaint);
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = strokeWidth;

            canvas.Save();
            canvas.DrawArc(rect, start, angle, false, paint);
            canvas.Restore();
        }
    }
}
