namespace StepUp.Maui.Controls;

public class MacroPieChartDrawable : IDrawable
{
    public float ProteinPercent { get; set; }
    public float FatPercent { get; set; }
    public float CarbonPercent { get; set; }

    public Color ProteinColor { get; set; } = Color.FromArgb("#9fd5bd");
    public Color FatColor { get; set; } = Color.FromArgb("#fceda8");
    public Color CarbonColor { get; set; } = Color.FromArgb("#e79bc3");
    public float StrokeWidth { get; set; } = 20f;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var centerX = dirtyRect.Width / 2f;
        var centerY = dirtyRect.Height / 2f;
        var radius = Math.Min(centerX, centerY) - StrokeWidth / 2f;
        if (radius <= 0) return;

        canvas.StrokeSize = StrokeWidth;
        canvas.StrokeLineCap = LineCap.Butt;

        var startAngle = -90f; 

        DrawSlice(canvas, centerX, centerY, radius, ref startAngle, ProteinPercent, ProteinColor);
        DrawSlice(canvas, centerX, centerY, radius, ref startAngle, FatPercent, FatColor);
        DrawSlice(canvas, centerX, centerY, radius, ref startAngle, CarbonPercent, CarbonColor);
    }

    private static void DrawSlice(ICanvas canvas, float cx, float cy, float radius,
                                  ref float startAngle, float percent, Color color)
    {
        if (percent <= 0) return;

        var sweep = percent * 3.6f; 
        canvas.StrokeColor = color;
        canvas.DrawArc(
            cx - radius, cy - radius,
            radius * 2, radius * 2,
            startAngle, startAngle + sweep,
            clockwise: true,
            closed: false);

        startAngle += sweep;
    }
}