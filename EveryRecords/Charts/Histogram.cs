using System.Linq;

using Android.Graphics;

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

            var singleChartWidth = Width / dataCount - margin * 2;
            var yMax = Data.Max();
            paint.Color = ChartColor;
            paint.TextSize = TextSize;
            var textHeight = paint.GetFontMetrics().Bottom - paint.GetFontMetrics().Ascent;
            var chartHeight = Height - textHeight - margin * 2;
            for (int i = 0; i < dataCount; i++)
            {
                if (Data[i] == 0)
                {
                    continue;
                }

                var left = (i * 2 + 1) * margin + singleChartWidth * i;
                var hPercent = Data[i] / yMax;
                var top = chartHeight - (float)hPercent * chartHeight + margin + textHeight;
                canvas.DrawRect(left, top, left + singleChartWidth, chartHeight + margin, paint);
                canvas.DrawText(Data[i].ToString(), left, top - margin, paint);
                if (Label != null && Label.Length > i)
                {
                    canvas.DrawText(Label[i], left, Height - margin / 2, paint);
                }
            }
            paint.Color = Color.Black;
            canvas.DrawLine(0, chartHeight + margin, Width, chartHeight + margin, paint);
            canvas.Save();
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