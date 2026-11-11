using System.Globalization;

namespace StepUp.Maui.Converters
{
    public class NutriscoreGradeToImageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var grade = (value as string)?.ToLowerInvariant();

            return grade switch
            {
                "a" => "nutriscore_a.svg",
                "b" => "nutriscore_b.svg",
                "c" => "nutriscore_c.svg",
                "d" => "nutriscore_d.svg",
                "e" => "nutriscore_e.svg",
                _ => "nutriscore_uk.svg",
            };
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}