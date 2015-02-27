using System;
using System.Drawing;
using System.Linq;

using Android.Graphics;

using Color = Android.Graphics.Color;
using Android.Text;

namespace EveryRecords.Charts
{
    public class PieChart:IChart
    {
        private readonly string[] _chartColors =
        {
            "#FF6666", "#996699", "#0099CC", "#FF9966", "#FFCCCC", "#CC9966",
            "#666666", "#003366", "#993333", "#993333", "#990033", "#CC0033", "#003300", "#0066CC", "#993333", "#CC3333",
            "#99CC00", "#336633", "#993333", "#333300",
        };
        
        public PieChart()
        {
            Data = new double[] { };
            Label = new string[] { };
            ChartColor = Color.LightGray;
            LabelColor = Color.LightGray;
        }

        public double[] Data { get; set; }
        public string[] Label { get; set; }
        
        public void Draw(Canvas canvas)
        {
            var paint = new Paint { AntiAlias = true };
            if (Data.Length <= 0)
            {
                return;
            }
            var total = Data.Sum();
            if (total == 0)
            {
                return;
            }
            paint.TextSize = TextSize;
            var labelLen = paint.MeasureText(FormatLabel(Data[0], total, Label[0]));
            var centerX = Width / 2;
            var centerY = Height / 2;
            var textHeight = paint.GetFontMetrics().Bottom - paint.GetFontMetrics().Ascent;
            var availableWidth = Width - labelLen * 1.5f;
            if (availableWidth <= 0)
            {
                availableWidth = Width / 2;
            }

            var rectWidth = Math.Min(availableWidth, Height - textHeight * 2);
            var rect = new RectF(centerX - rectWidth / 2, centerY - rectWidth / 2, centerX + rectWidth / 2, centerY + rectWidth / 2);
            var current = 0f;

            for (int i = 0; i < Data.Length; i++)
            {
                var value = (Data[i] / total) * 360;
                var color = (System.Drawing.Color)new ColorConverter().ConvertFromString(_chartColors[i]);
                paint.Color = new Color(color.R, color.G, color.B);
                canvas.DrawArc(rect, current, (float)value, true, paint);
                if (value != 0 && Label.Length>i)
                {
                    DrawLabel(
                        canvas,
                        current + value / 2,
                        centerX,
                        centerY,
                        rectWidth,
                        textHeight,
                        paint,
                        Data[i],
                        total,
                        Label[i]);
                }
                current += (float)value;
            }
        }

        public void InitSize(float width, float height, float textSize)
        {
            Width = width;
            Height = height;
            TextSize = textSize;
        }
        
        public float Width { get; private set; }

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

        public Color ChartColor { get; set; }

        public Color LabelColor { get; set; }

        private void DrawLabel(Canvas canvas, double angle, float centerX, float centerY, float rectWidth, float textHeight, Paint paint, double data, double amount, string label)
        {
            string text = FormatLabel(data, amount, label);
            if (angle > 270)
            {
                var offsetX = Math.Sin((angle - 270) * Math.PI / 180) * rectWidth / 2;
                var offsetY = Math.Cos((angle - 270) * Math.PI / 180) * rectWidth / 2;

                //canvas.DrawText(text, (int)(centerX + offsetX), (int)(centerY - offsetY), paint);
                var textX=centerX + (float)offsetX;
                var textY=centerY - (float)offsetY;
                DrawText(canvas, textX, textY - textHeight, (int)(Width - textX), text, paint);
            }
            else if (angle > 180)
            {
                var offsetX = Math.Cos((angle - 180) * Math.PI / 180) * rectWidth / 2;
                var offsetY = Math.Sin((angle - 180) * Math.PI / 180) * rectWidth / 2;
                var textLen = paint.MeasureText(text);
                var x = Width / 2 - offsetX - textLen;
                if (x < 0)
                {
                    DrawText(canvas, 0, centerY - (float)offsetY - (float)textHeight, (int)(Width / 2 - offsetX), text, paint);
                }
                else
                {
                    DrawText(canvas, (float)x, centerY - (float)offsetY - (float)textHeight, (int)textLen, text, paint);
                }
                //canvas.DrawText(text, (int)x, (int)(centerY - offsetY), paint);
            }
            else if (angle > 90)
            {
                var offsetX = Math.Sin((angle - 90) * Math.PI / 180) * rectWidth / 2;
                var offsetY = Math.Cos((angle - 90) * Math.PI / 180) * rectWidth / 2;
                var textLen = paint.MeasureText(text);
                var x = Width / 2 - offsetX - textLen;
                if (x < 0)
                {
                    DrawText(canvas, 0, (float)(centerY + offsetY), (int)(Width / 2 - offsetX), text, paint);
                }
                else
                {
                    DrawText(canvas, (float)x, (float)(centerY + offsetY), (int)textLen, text, paint);
                }
                //canvas.DrawText(text, (int)x, (int)(centerY + offsetY + textHeight), paint);
            }
            else
            {
                var offsetY = Math.Sin(angle * Math.PI / 180) * rectWidth / 2;
                var offsetX = Math.Cos(angle * Math.PI / 180) * rectWidth / 2;

                //canvas.DrawText(
                //    text,
                //    (int)(centerX + offsetX),
                //    (int)(centerY + offsetY + textHeight),
                //    paint);
                var textX = centerX + (float)offsetX;
                var textY = (float)(centerY + offsetY);
                DrawText(canvas, textX, textY, (int)(Width-offsetX), text, paint);
            }
        }

        private string FormatLabel(double data, double amount, string label)
        {
            string text = string.Format("{0}:{1},{2}% ", label, data, (int)(data / amount * 100 + 0.5));
            return text;
        }

        private void DrawText(Canvas canvas, float x, float y, int textWidth, string text, Paint paint)
        {
            var textPaint = new TextPaint(paint);
            StaticLayout layout = new StaticLayout(text, textPaint, textWidth, Layout.Alignment.AlignNormal, 1.0F, 0.0F, true);
            canvas.Save();
            canvas.Translate(x, y);
            layout.Draw(canvas);
            canvas.Restore();
        }
    }
}