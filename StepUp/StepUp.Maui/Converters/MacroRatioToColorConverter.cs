using System.Globalization;

namespace StepUp.Maui.Converters;

public class MacroRatioToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not double ratio)
            return Colors.Gray;

        if (ratio < 0) ratio = 0;

        Color green = Colors.Green;
        Color yellow = Colors.Yellow;
        Color orange = Colors.Orange;
        Color red = Colors.Red;

        return ratio switch
        {
            <= 0.5 => Interpolate(green, yellow, ratio / 0.5),            
            <= 0.85 => Interpolate(yellow, orange, (ratio - 0.5) / 0.35), 
            <= 1.0 => Interpolate(orange, red, (ratio - 0.85) / 0.15),    
            _ => Colors.DarkRed                                       
        };
    }

    private Color Interpolate(Color start, Color end, double progress)
    {
        return new Color(
            (float)(start.Red + (end.Red - start.Red) * progress),
            (float)(start.Green + (end.Green - start.Green) * progress),
            (float)(start.Blue + (end.Blue - start.Blue) * progress)
        );
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}