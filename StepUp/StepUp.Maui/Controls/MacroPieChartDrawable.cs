namespace StepUp.Maui.Controls;

public class MacroPieChartDrawable : IDrawable
{
    public float ProteinPercent { get; set; }
    public float FatPercent { get; set; }
    public float CarbonPercent { get; set; }

    private float _centerX = 70;
    private float _centerY = 70;
    private float _radius;
    private ICanvas _canvas = null!;

    public Color ProteinColor { get; set; } = Color.FromArgb("#9fd5bd");
    public Color FatColor { get; set; } = Color.FromArgb("#fceda8");
    public Color CarbonColor { get; set; } = Color.FromArgb("#e79bc3");
    public float StrokeWidth { get; set; } = 20f;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        _canvas = canvas;
        _centerX = dirtyRect.Width / 2f;
        _centerY = dirtyRect.Height / 2f;
        _radius = Math.Min(_centerX, _centerY) - StrokeWidth / 2f;
        if (_radius <= 0) return;

        canvas.StrokeSize = StrokeWidth;
        canvas.StrokeLineCap = LineCap.Butt;

        var startAngle = -90f;

        DrawSlice(ref startAngle, ProteinPercent, ProteinColor);
        DrawSlice(ref startAngle, FatPercent, FatColor);
        DrawSlice(ref startAngle, CarbonPercent, CarbonColor);
    }

    private void DrawSlice(ref float startAngle, float percent, Color color)
    {
        if (percent <= 0) return;
        _canvas.StrokeColor = color;
        _canvas.StrokeSize = 15;
        if (percent == 100)
        {
            _canvas.DrawCircle(_centerX, _centerY, _radius);
            return;
        }

        var sweep = percent * 3.6f; 
        _canvas.DrawArc(
            _centerX - _radius, _centerY - _radius,
            _radius * 2, _radius * 2,
            startAngle, startAngle + sweep,
            clockwise: true,
            closed: false);

        startAngle += sweep;
    }
}