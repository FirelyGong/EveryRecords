using System.Linq;

using Android.Graphics;
using Android.Content.Res;

namespace EveryRecords.Charts
{
    public class Histogram: IChart
    {

        public Color ChartColor { get;set; }

        public double[] Data { get; set; }

        public Histogram()
        {
            Data=new double[]{};
            Label=new string[]{};
            ChartColor = Color.LightGray;
            LabelColor = Color.LightGray;
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
            var singleChartWidth = Width / dataCount - margin * 2;
            var yMax = Data.Max();
            paint.Color = ChartColor;
            paint.TextSize = TextSize;
            var textHeight = paint.GetFontMetrics().Bottom - paint.GetFontMetrics().Ascent;
            var chartHeight = Height - textHeight* 2;
            for (int i = 0; i < dataCount; i++)
            {
                var left = (i * 2 + 1) * margin + singleChartWidth * i;
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
                    if (Label != null && Label.Length > i)
                    {
                        canvas.DrawText(Label[i], left, Height - margin, paint);
                    }
                }

                top = chartHeight - histogramHeight + textHeight;
                canvas.DrawRect(left, top, left + singleChartWidth, chartHeight + textHeight, paint);
                canvas.DrawText(Data[i].ToString(), left, top - margin, paint);
            }
            paint.Color = Color.Black;
            canvas.DrawLine(0, chartHeight + textHeight, Width, chartHeight + textHeight, paint);
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