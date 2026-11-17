using System.Globalization;

namespace StepUp.Maui.Converters
{
    public class NovaGroupToImageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (!int.TryParse((string?)value, out var group) || group > 4 ||
                group < 1) return "nova_uk.svg";
            return $"nova_{group}.svg";
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}