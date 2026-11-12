namespace StepUp.Maui.Controls;

public class MacroPieChart : GraphicsView
{
    public static readonly BindableProperty ProteinPercentProperty = BindableProperty.Create(
        nameof(ProteinPercent), typeof(double), typeof(MacroPieChart), 0.0,
        propertyChanged: OnPercentChanged);

    public static readonly BindableProperty FatPercentProperty = BindableProperty.Create(
        nameof(FatPercent), typeof(double), typeof(MacroPieChart), 0.0,
        propertyChanged: OnPercentChanged);

    public static readonly BindableProperty CarbonPercentProperty = BindableProperty.Create(
        nameof(CarbonPercent), typeof(double), typeof(MacroPieChart), 0.0,
        propertyChanged: OnPercentChanged);

    private readonly MacroPieChartDrawable _drawable = new();

    public MacroPieChart()
    {
        Drawable = _drawable;
    }

    public double ProteinPercent
    {
        get => (double)GetValue(ProteinPercentProperty);
        set => SetValue(ProteinPercentProperty, value);
    }

    public double FatPercent
    {
        get => (double)GetValue(FatPercentProperty);
        set => SetValue(FatPercentProperty, value);
    }

    public double CarbonPercent
    {
        get => (double)GetValue(CarbonPercentProperty);
        set => SetValue(CarbonPercentProperty, value);
    }

    private static void OnPercentChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var chart = (MacroPieChart)bindable;
        chart._drawable.ProteinPercent = (float)chart.ProteinPercent;
        chart._drawable.FatPercent = (float)chart.FatPercent;
        chart._drawable.CarbonPercent = (float)chart.CarbonPercent;
        chart.Invalidate();   // ← перерисовать
    }
}