using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using StepUp.Presentation.Models;
using System.Collections;
using System.Collections.Specialized;

namespace StepUp.Maui.Controls;

public partial class LineChartView : ContentView
{
    public LineChartView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource), typeof(IEnumerable), typeof(LineChartView), null,
        propertyChanged: OnItemsSourceChanged);

    public IEnumerable? ItemsSource
    {
        get => (IEnumerable?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly BindableProperty LineColorProperty = BindableProperty.Create(
        nameof(LineColor), typeof(Color), typeof(LineChartView), Colors.MediumPurple,
        propertyChanged: OnVisualChanged);

    public Color LineColor
    {
        get => (Color)GetValue(LineColorProperty);
        set => SetValue(LineColorProperty, value);
    }

    public static readonly BindableProperty YLabelSuffixProperty = BindableProperty.Create(
        nameof(YLabelSuffix), typeof(string), typeof(LineChartView), "",
        propertyChanged: OnVisualChanged);

    public string YLabelSuffix
    {
        get => (string)GetValue(YLabelSuffixProperty);
        set => SetValue(YLabelSuffixProperty, value);
    }

    public static readonly BindableProperty DateFormatProperty = BindableProperty.Create(
        nameof(DateFormat), typeof(string), typeof(LineChartView), "dd.MM",
        propertyChanged: OnVisualChanged);

    public string DateFormat
    {
        get => (string)GetValue(DateFormatProperty);
        set => SetValue(DateFormatProperty, value);
    }

    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (LineChartView)bindable;

        if (oldValue is INotifyCollectionChanged oldColl)
            oldColl.CollectionChanged -= view.OnCollectionChanged;
        if (newValue is INotifyCollectionChanged newColl)
            newColl.CollectionChanged += view.OnCollectionChanged;

        view.Rebuild();
    }

    private static void OnVisualChanged(BindableObject bindable, object oldValue, object newValue)
        => ((LineChartView)bindable).Rebuild();

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => Rebuild();

    private void Rebuild()
    {
        if (Chart == null) return;
        if (ItemsSource is null)
        {
            Chart.Series = [];
            return;
        }

        var points = ItemsSource.OfType<DoubleChartPoint>().ToList();

        var values = points
            .Select(p => new DateTimePoint(p.Date, p.Value))
            .ToList();

        var skColor = new SKColor(
            (byte)(LineColor.Red * 255),
            (byte)(LineColor.Green * 255),
            (byte)(LineColor.Blue * 255));

        Chart.Series =
        [
            new LineSeries<DateTimePoint>
            {
                Values = values,
                Fill = null,
                GeometrySize = 0,
                GeometryStroke = null,
                GeometryFill = null,
                LineSmoothness = 0.3,
                Stroke = new SolidColorPaint(skColor) { StrokeThickness = 3 },
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