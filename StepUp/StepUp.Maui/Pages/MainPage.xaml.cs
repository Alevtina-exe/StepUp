using Microsoft.Maui.Handlers;
using StepUp.Maui.Utils;

namespace StepUp.Maui.Pages;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _vm;
    public MainPage(MainViewModel vm)
    {
		InitializeComponent();
        _vm = vm;
        BindingContext = vm;
        _vm.PropertyChanged += ViewModel_PropertyChanged;
    }
	protected override async void OnAppearing() 
	{
		base.OnAppearing();
        await _vm.InitializeAsync();
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
        double targetEaten = _vm.KcalEaten;
        double targetBurnt = _vm.KcalBurnt;

        _vm.KcalEaten = 0;
        _vm.KcalBurnt = 0;

        new Animation(
            callback: v =>
            {
                _vm.KcalEaten = (int)Math.Round(v * targetEaten);
                _vm.KcalBurnt = (int)Math.Round(v * targetBurnt);
            },
            start: 0,
            end: 1,
            easing: Easing.CubicOut
        ).Commit(this, "AnimAll", 16, 1500);

        AnimateMacros();
        AnimateKcalCircle();
    }

    private void AnimateMacros()
    {
        double targetPr = _vm.PrProgress;
        double targetFat = _vm.FatProgress;
        double targetCb = _vm.CbProgress;

        _vm.PrProgress = 0;
        _vm.FatProgress = 0;
        _vm.CbProgress = 0;

        var animation = new Animation(
            callback: v =>
            {
                _vm.PrProgress = v * targetPr;
                _vm.FatProgress = v * targetFat;
                _vm.CbProgress = v * targetCb;
            },
            start: 0,
            end: 1,
            easing: Easing.CubicOut 
        );

        animation.Commit(this, "MacrosAnim", 16, 1000);
    }

    private void AnimateKcalCircle()
    {
        double targetProgress = _vm.KcalProgress;
        int targetKcalRes = _vm.KcalRes;

        progressBar.IsVisible = targetProgress != 0;
        Arc.Point = new Point(67, 0);
        Arc.IsLargeArc = false;
        progressBar.Stroke = new SolidColorBrush(Colors.Green);
        KcalResLabel.Text = "0";
        KcalLeft.TextColor = Colors.Green;


        var animation = new Animation(
            callback: v =>
            {
                double currentProgress = Math.Min(v * targetProgress, 0.9999);

                double angle = (Math.PI * 2 * currentProgress) - (Math.PI / 2);
                double x = 67 + 67 * Math.Cos(angle);
                double y = 67 + 67 * Math.Sin(angle);

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
        );

        animation.Commit(this, "KcalCircleAnim", 16, 1500);
    }
}