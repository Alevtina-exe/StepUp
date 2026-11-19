using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using StepUp.Presentation.Models;
using System.Collections;
using System.Collections.Specialized;

namespace StepUp.Maui.Controls;

public partial class ColumnChartView : ContentView
{
    public ColumnChartView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty KcalEatenSeriesProperty = BindableProperty.Create(
        nameof(KcalEatenSeries), typeof(IEnumerable), typeof(ColumnChartView), null,
        propertyChanged: OnItemsSourceChanged);
    public IEnumerable? KcalEatenSeries
    {
        get => (IEnumerable?)GetValue(KcalEatenSeriesProperty);
        set => SetValue(KcalEatenSeriesProperty, value);
    }

    public static readonly BindableProperty KcalBurntSeriesProperty = BindableProperty.Create(
        nameof(KcalBurntSeries), typeof(IEnumerable), typeof(ColumnChartView), null,
        propertyChanged: OnItemsSourceChanged);
    public IEnumerable? KcalBurntSeries
    {
        get => (IEnumerable?)GetValue(KcalBurntSeriesProperty);
        set => SetValue(KcalBurntSeriesProperty, value);
    }

    public static readonly BindableProperty FirstColumnColorProperty = BindableProperty.Create(
        nameof(FirstColumnColor), typeof(Color), typeof(ColumnChartView), Colors.MediumPurple,
        propertyChanged: OnVisualChanged);
    public Color FirstColumnColor
    {
        get => (Color)GetValue(FirstColumnColorProperty);
        set => SetValue(FirstColumnColorProperty, value);
    }

    public static readonly BindableProperty SecondColumnColorProperty = BindableProperty.Create(
        nameof(FirstColumnColor), typeof(Color), typeof(ColumnChartView), Colors.MediumPurple,
        propertyChanged: OnVisualChanged);
    public Color SecondColumnColor
    {
        get => (Color)GetValue(FirstColumnColorProperty);
        set => SetValue(FirstColumnColorProperty, value);
    }

    public static readonly BindableProperty YLabelSuffixProperty = BindableProperty.Create(
        nameof(YLabelSuffix), typeof(string), typeof(ColumnChartView), "",
        propertyChanged: OnVisualChanged);
    public string YLabelSuffix
    {
        get => (string)GetValue(YLabelSuffixProperty);
        set => SetValue(YLabelSuffixProperty, value);
    }

    public static readonly BindableProperty DateFormatProperty = BindableProperty.Create(
        nameof(DateFormat), typeof(string), typeof(ColumnChartView), "dd.MM",
        propertyChanged: OnVisualChanged);
    public string DateFormat
    {
        get => (string)GetValue(DateFormatProperty);
        set => SetValue(DateFormatProperty, value);
    }

    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (ColumnChartView)bindable;

        if (oldValue is INotifyCollectionChanged oldColl)
            oldColl.CollectionChanged -= view.OnCollectionChanged;
        if (newValue is INotifyCollectionChanged newColl)
            newColl.CollectionChanged += view.OnCollectionChanged;

        view.Rebuild();
    }

    private static void OnVisualChanged(BindableObject bindable, object oldValue, object newValue)
        => ((ColumnChartView)bindable).Rebuild();

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => Rebuild();

    private void Rebuild()
    {
        if (Chart == null) return;
        if (KcalEatenSeries is null)
        {
            Chart.Series = [];
            return;
        }
        if (KcalBurntSeries is null)
        {
            Chart.Series = [];
            return;
        }

        var pointsOfEaten = KcalEatenSeries.OfType<IntChartPoint>().ToList();
        var pointsOfBurnt = KcalBurntSeries.OfType<IntChartPoint>().ToList();

        var valuesOfEaten = pointsOfEaten
            .Select(p => new DateTimePoint(p.Date, p.Value))
            .ToList();
        var valuesOfBurnt = pointsOfBurnt
            .Select(p => new DateTimePoint(p.Date, p.Value))
            .ToList();

        var colorOfEaten = new SKColor(
            (byte)(FirstColumnColor.Red * 255),
            (byte)(FirstColumnColor.Green * 255),
            (byte)(FirstColumnColor.Blue * 255));
        var colorOfBurnt = new SKColor(
            (byte)(SecondColumnColor.Red * 255),
            (byte)(SecondColumnColor.Green * 255),
            (byte)(SecondColumnColor.Blue * 255));

        Chart.Series =
        [
            new ColumnSeries<DateTimePoint>
            {
                Name = "потреблено",
                Values = valuesOfEaten,
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                Fill = new SolidColorPaint(colorOfEaten)
            },
            new ColumnSeries<DateTimePoint>
            {
                Name = "сожжено",
                Values = valuesOfBurnt,
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                Fill = new SolidColorPaint(colorOfBurnt)
            }
        ];

        Chart.XAxes =
        [
            new DateTimeAxis(TimeSpan.FromDays(1), d => d.ToString(DateFormat))
            {
                LabelsRotation = -45,
                TextSize = 11,
                SeparatorsPaint = null,
            }
        ];

        Chart.YAxes =
        [
            new Axis
            {
                TextSize = 11,
                Labeler = v => $"{v:F0}{YLabelSuffix}",
                SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 }
            }
        ];
    }
}