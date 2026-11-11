using StepUp.Domain.Enums;
using System.Globalization;

namespace StepUp.Maui.Converters;

public class BmiCategoryToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not BmiCategory category)
            return Colors.Black; 

        var resources = Microsoft.Maui.Controls.Application.Current?.Resources ??
            throw new ArgumentNullException("App is not initiaized.");

        return category switch
        {
            BmiCategory.Deficit => resources["BmiDeficit"] as Color ?? Colors.RoyalBlue,
            BmiCategory.Normal => resources["BmiNormal"] as Color ?? Colors.Green,
            BmiCategory.Overweight => resources["BmiOverweight"] as Color ?? Colors.Yellow,
            BmiCategory.Obese1 => resources["BmiObese1"] as Color ?? Colors.Orange,
            BmiCategory.Obese2 => resources["BmiObese2"] as Color ?? Colors.Red,
            BmiCategory.Obese3 => resources["BmiObese3"] as Color ?? Colors.DarkRed,
            _ => Colors.Black
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}