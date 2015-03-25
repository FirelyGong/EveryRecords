using System.Linq;

using Android.Graphics;
using Android.Content.Res;

namespace EveryRecords.Charts
{
    public class Histogram: IChart
    {

        public Color ChartColor { get;set; }

        public double[] Data { get; set; }

        public Histogram(double[] data, string[] label)
            : this(data, label, Color.LightGray, Color.LightGray)
        {
        }

        public Histogram(double[] data, string[] label, Color chartColor, Color labelColor)
        {
            Data = data;
            Label = label;
            ChartColor = chartColor;
            LabelColor = labelColor;
        }

        public void Draw(Canvas canvas)
        {
            var paint = new Paint { AntiAlias = true };
            int margin = 10;
            var dataCount = Data.Length;
            if (dataCount <= 0)
            {
                return;
            }

            var minHeight = 2 * Resources.System.DisplayMetrics.Density;
            var singleChartWidth = (Width - margin) / dataCount - margin * 2;
            var yMax = Data.Max();
            paint.TextSize = TextSize;
            var textHeight = paint.GetFontMetrics().Bottom - paint.GetFontMetrics().Ascent;
            var chartHeight = Height - textHeight* 3;
            for (int i = 0; i < dataCount; i++)
            {
                paint.Color = LabelColor;
                var left = (i * 2 + 2) * margin + singleChartWidth * i;
                float top = 0;
                float histogramHeight;
                if (Data[i] == 0)
                {
                    histogramHeight = minHeight;
                }
                else
                {
                    var hPercent = Data[i] / yMax;
                    histogramHeight = (float)hPercent * chartHeight;
                    if (histogramHeight < minHeight)
                    {
                        histogramHeight = minHeight;
                    }
                }
                if (Label != null && Label.Length > i)
                {
                    canvas.DrawText(Label[i], left, Height - margin, paint);
                }

                top = chartHeight - histogramHeight + textHeight * 2;
                canvas.DrawText(Data[i].ToString(), left, top - margin, paint);

                paint.Color = ChartColor;
                canvas.DrawRect(left, top, left + singleChartWidth, top + histogramHeight, paint);
            }
            paint.Color = Color.Gray;
            paint.StrokeWidth = 3;
            canvas.DrawLine(0, chartHeight + textHeight * 2, Width, chartHeight + textHeight * 2, paint);
            canvas.DrawLine(margin, textHeight, margin, chartHeight + textHeight * 2, paint);
        }

        public string[] Label { get; set; }

        public Color LabelColor { get; set; }

        public void InitSize(float width, float height, float textSize)
        {
            Width = width;
            Height = height;
            TextSize = textSize;
        }

        public float Width { get; private set; }

        public float Height { get; private set; }

        public float TextSize { get; private set; }
    }
}