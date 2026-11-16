using Microsoft.Maui.Handlers;
using StepUp.Maui.Utils;

namespace StepUp.Maui.Pages;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _vm;
    private bool _isAnimating;

    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _vm.PropertyChanged -= ViewModel_PropertyChanged;
        _vm.PropertyChanged += ViewModel_PropertyChanged;

        await _vm.InitializeAsync();

        AnimateAll();
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.CurrDayRes))
        {
            AnimateAll();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _vm.PropertyChanged -= ViewModel_PropertyChanged;
    }

    private void DateButtonClicked(object sender, EventArgs e)
    {
        var handler = datePicker.Handler as IDatePickerHandler;
        handler?.PlatformView?.PerformClick();
    }

    private void DateButtonTapped(object sender, TappedEventArgs e)
    {
        DateButtonClicked(sender, e);
    }

    private void AnimateAll()
    {
        if (_isAnimating) return;  
        _isAnimating = true;

        double targetEaten = _vm.KcalEaten;
        double targetBurnt = _vm.KcalBurnt;

        KcalBurntLabel.Text = "0";
        KcalEatenLabel.Text = "0";

        new Animation(
            callback: v =>
            {
                KcalEatenLabel.Text = ((int)Math.Round(v * targetEaten)).ToString();
                KcalBurntLabel.Text = ((int)Math.Round(v * targetBurnt)).ToString();
            },
            start: 0,
            end: 1,
            easing: Easing.CubicOut
        ).Commit(this, "AnimAll", 16, 1500,
                 finished: (_, _) => _isAnimating = false);

        AnimateMacros();
        AnimateKcalCircle();
    }

    private void AnimateMacros()
    {
        double targetPr = _vm.PrProgress;
        double targetFat = _vm.FatProgress;
        double targetCb = _vm.CbProgress;

        _vm.PrProgressBar = 0;
        _vm.FatProgressBar = 0;
        _vm.CbProgressBar = 0;

        new Animation(
            callback: v =>
            {
                _vm.PrProgressBar = v * targetPr;
                _vm.FatProgressBar = v * targetFat;
                _vm.CbProgressBar = v * targetCb;
            },
            start: 0,
            end: 1,
            easing: Easing.CubicOut
        ).Commit(this, "MacrosAnim", 16, 1500);
    }

    private const double CenterXY = 80;
    private const double Radius = 72;

    private void AnimateKcalCircle()
    {
        double targetProgress = _vm.KcalProgress;
        int targetKcalRes = _vm.KcalRes;

        progressBar.IsVisible = targetProgress != 0;
        progressBar.BackgroundColor = Colors.Transparent;
        Arc.Point = new Point(CenterXY, 0);
        Arc.IsLargeArc = false;
        Arc.SweepDirection = SweepDirection.Clockwise;
        progressBar.Stroke = new SolidColorBrush(Colors.Green);
        KcalResLabel.Text = "0";
        KcalLeft.TextColor = Colors.Green;

        new Animation(
            callback: v =>
            {
                double currentProgress = Math.Min(v * targetProgress, 0.9999);

                double angle = (Math.PI * 2 * currentProgress) - (Math.PI / 2);
                double x = CenterXY + Radius * Math.Cos(angle);
                double y = CenterXY + Radius * Math.Sin(angle);

                Arc.Point = new Point(x, y);
                Arc.IsLargeArc = currentProgress > 0.5;
                Arc.SweepDirection = SweepDirection.Clockwise;

                var currentColor = ColorInterpolator.GetColor(currentProgress);
                progressBar.Stroke = new SolidColorBrush(currentColor);
                KcalResLabel.TextColor = currentColor;
                KcalLeft.TextColor = currentColor;

                KcalResLabel.Text = ((int)(v * targetKcalRes)).ToString();
            },
            start: 0,
            end: 1,
            easing: Easing.CubicOut
        ).Commit(this, "KcalCircleAnim", 16, 1500);
    }
}