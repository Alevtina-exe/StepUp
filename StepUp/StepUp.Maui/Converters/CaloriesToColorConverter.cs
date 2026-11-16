using System.Globalization;

namespace StepUp.Maui.Converters;

public class CaloriesToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        double kcal = ToDouble(value);

        Color green = GetColor("AcceptColor", Colors.Green);
        Color yellow = GetColor("Warning", Colors.Gold);
        Color orange = GetColor("Orange", Colors.Orange);
        Color red = GetColor("ErrorColor", Colors.Red);

        return kcal switch
        {
            <= 150 => green,
            <= 250 => yellow,
            <= 350 => orange,
            _ => red
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();

    private static double ToDouble(object? value) => value switch
    {
        null => 0,
        int i => i,
        double d => d,
        float f => f,
        decimal m => (double)m,
        string s when double.TryParse(
            s.Replace(',', '.'),
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out var r) => r,
        _ => 0
    };

    private static Color GetColor(string key, Color fallback) =>
        Microsoft.Maui.Controls.Application.Current?.Resources[key] as Color ?? fallback;
}