namespace StepUp.Maui.Utils;

public static class ColorInterpolator
{
    private static readonly Color[] UsedColors =
    {
        Colors.Green,
        Colors.Yellow,
        Colors.Orange,
        Colors.Red
    };

    public static Color GetColor(double progress)
    {
        progress = Math.Clamp(progress, 0, 1);

        int index = (int)(progress * (UsedColors.Length - 1));
        double localProgress = (progress * (UsedColors.Length - 1)) - index;

        if (index >= UsedColors.Length - 1) return UsedColors[^1];

        Color start = UsedColors[index];
        Color end = UsedColors[index + 1];

        return new Color(
            (float)(start.Red + (end.Red - start.Red) * localProgress),
            (float)(start.Green + (end.Green - start.Green) * localProgress),
            (float)(start.Blue + (end.Blue - start.Blue) * localProgress)
        );
    }
}