
using Android.Graphics;

namespace EveryRecords.Charts
{
    public interface IChart
    {
        void InitSize(float width, float height, float textSize);

        void Draw(Canvas canvas);

        float Width { get; }

        float Height { get; }

        float TextSize { get; }

        double[] Data { get; set; }

        string[] Label { get; set; }

        Color ChartColor { get; set; }

        Color LabelColor { get; set; }
    }
}