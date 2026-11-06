namespace StepUp.Maui.Utils
{
    public class GraphDrawer(List<PointF> points) : IDrawable
    {
        private List<PointF> _points = points;

        public void Draw(ICanvas canvas, RectF rect)
        {
            canvas.StrokeColor = Color.FromArgb("#512bd4");
            canvas.StrokeSize = 2;
            for (int i = 0; i < _points.Count - 1; i++)
            {
                canvas.DrawLine(_points[i].X, _points[i].Y, _points[i + 1].X, _points[i + 1].Y);
            }
        }
    }
}