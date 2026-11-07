using System.Globalization;
using StepUp.Presentation.Models;

namespace StepUp.Maui.Converters;

public class ImtCategoryToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not ImtCategory category)
            return Colors.Black; 

        var resources = Microsoft.Maui.Controls.Application.Current?.Resources ??
            throw new ArgumentNullException("App is not initiaized.");

        return category switch
        {
            ImtCategory.Deficit => resources["ImtDeficit"] as Color ?? Colors.RoyalBlue,
            ImtCategory.Normal => resources["ImtNormal"] as Color ?? Colors.Green,
            ImtCategory.Overweight => resources["ImtOverweight"] as Color ?? Colors.Yellow,
            ImtCategory.Obese1 => resources["ImtObese1"] as Color ?? Colors.Orange,
            ImtCategory.Obese2 => resources["ImtObese2"] as Color ?? Colors.Red,
            ImtCategory.Obese3 => resources["ImtObese3"] as Color ?? Colors.DarkRed,
            _ => Colors.Black
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}