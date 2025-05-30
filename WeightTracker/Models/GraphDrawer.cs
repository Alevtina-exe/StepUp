using Microsoft.Maui.Graphics;

public class GraphDrawer : IDrawable
{
    private List<PointF> _points;
    private ICanvas _canvas;
    public GraphDrawer(List<PointF> points, ICanvas canvas)
    {
        _points = points;
        _canvas = canvas;
    }
    public void Draw(ICanvas canvas, RectF rect)
    {
        _canvas.StrokeColor = Color.FromArgb("#512bd4");
        _canvas.StrokeSize = 2;
        for (int i = 0; i < _points.Count - 1; i++)
        {
            _canvas.DrawLine(_points[i].X, _points[i].Y, _points[i + 1].X, _points[i + 1].Y);
        }
    }
}