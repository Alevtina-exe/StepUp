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
        AnimateInt(
            targetValue: _vm.KcalEaten,
            setter: v => _vm.KcalEaten = v,
            animationName: "AnimKcalEaten"
        );

        AnimateInt(
            targetValue: _vm.KcalBurnt,
            setter: v => _vm.KcalBurnt = v,
            animationName: "AnimKcalBurnt"
        );

        AnimateInt(
            targetValue: _vm.KcalRes,
            setter: v => _vm.KcalRes = v,
            animationName: "AnimKcalRes"
        );

        AnimateMacros();
        AnimateKcalCircle();
    }
    private void AnimateInt(int targetValue, Action<int> setter, string animationName, uint duration = 1500)
    {
        this.AbortAnimation(animationName);

        if (targetValue == 0)
        {
            setter(0);
            return;
        }

        setter(0);

        var animation = new Animation(
            callback: v =>
            {
                int currentValue = (int)Math.Round(v * targetValue);
                setter(currentValue);
            },
            start: 0,
            end: 1,
            easing: Easing.CubicOut 
        );

        animation.Commit(
            owner: this,
            name: animationName,
            rate: 16, 
            length: duration
        );
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

        Arc.Point = new Point(70, 0);
        Arc.IsLargeArc = false;
        progressBar.Stroke = new SolidColorBrush(Colors.Green);
        KcalResLabel.Text = "0";
        KcalLeft.TextColor = Colors.Green;

        var animation = new Animation(
            callback: v =>
            {
                double currentProgress = v * targetProgress;

                double angle = (Math.PI * 2 * currentProgress) - (Math.PI / 2);
                double x = 70 + 70 * Math.Cos(angle);
                double y = 70 + 70 * Math.Sin(angle);

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