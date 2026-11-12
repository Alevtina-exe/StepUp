using System.Globalization;

namespace StepUp.Maui.Converters;

public class BoolToGreenRedConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isValid && !isValid)
            return Microsoft.Maui.Controls.Application.Current?.Resources["ErrorColor"] as Color ??
                Colors.Red;

        return Microsoft.Maui.Controls.Application.Current?.Resources["AcceptColor"] as Color ??
            Colors.Green;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}