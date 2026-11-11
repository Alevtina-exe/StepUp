using System.Globalization;

namespace StepUp.Maui.Converters
{
    public class NovaGroupToImageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var grade = (int?)value;

            if (grade == null || grade.Value > 4 ||
                grade.Value < 1) return "nutriscore_uk.svg";
            return $"nutriscore_{grade}.svg";
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}