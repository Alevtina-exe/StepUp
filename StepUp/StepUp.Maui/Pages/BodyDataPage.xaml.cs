using StepUp.Application.Models;

namespace StepUp.Maui.Pages;

public partial class BodyDataPage : ContentPage, IQueryAttributable
{
    private readonly BodyDataViewModel _vm;

    public BodyDataPage(BodyDataViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
        NavigationPage.SetHasNavigationBar(this, false);
    }
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Request", out var r) && r is RegisterRequest request)
        {
            _vm.ReceiveParameter(request);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.PropertyChanged += ViewModel_PropertyChanged;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _vm.PropertyChanged -= ViewModel_PropertyChanged;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        UpdateBmiIndicatorPosition();
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(BodyDataViewModel.BmiResult) ||
            e.PropertyName == nameof(BodyDataViewModel.IsFormValid))
        {
            UpdateBmiIndicatorPosition();
        }
    }

    private void UpdateBmiIndicatorPosition()
    {
        double imt = _vm.BmiResult?.Value ?? 0;

        if (MyGrid.Width <= 0 || !_vm.IsFormValid)
        {
            IMTIndicator.IsVisible = false;
            return;
        }

        double relativeOffset;
        if (imt <= 8.9) relativeOffset = 0;
        else if (imt >= 43.9) relativeOffset = MyGrid.Width;
        else relativeOffset = (imt - 8.9) / (43.9 - 8.9) * MyGrid.Width;

        IMTIndicator.Margin = new Thickness(relativeOffset, 0, 0, 0);
        IMTIndicator.IsVisible = true;
    }
}