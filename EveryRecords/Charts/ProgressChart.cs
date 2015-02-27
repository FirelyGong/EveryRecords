using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveryRecords.Charts
{
    public class ProgressChart:IChart
    {
        public ProgressChart()
        {
            Data=new double[]{};
            Label=new string[]{};
            ChartColor = Color.White;
            LabelColor = Color.Black;
        }

        public void InitSize(float width, float height, float textSize)
        {
            Width = width;
            Height = height;
            TextSize = textSize;
        }

        public void Draw(Canvas canvas)
        {
            if (Data.Length != 2)
            {
                return;
            }

            LinearGradient shader = new LinearGradient(0, 0, Width, Height, new int[] { Color.LightGreen, Color.Orange, Color.Red }, null, Shader.TileMode.Mirror);
            var paint = new Paint { AntiAlias = true };
            paint.SetShader(shader);
            canvas.DrawRect(0, 0, Width, Height, paint);

            var percent = Data[0] / Data[1];
            if (percent > 1)
            {
                percent = 1;
            }

            var posX = Width * percent;
            paint.SetShader(null);
            paint.Color = ChartColor;
            canvas.DrawRect((float)posX, 0, Width, Height, paint);
            canvas.Save();
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
    }
}
