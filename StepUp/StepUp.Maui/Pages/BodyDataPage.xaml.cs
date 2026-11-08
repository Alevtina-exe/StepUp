namespace StepUp.Maui.Views;

public partial class BodyDataPage : ContentPage
{
    private readonly BodyDataViewModel _viewModel;

    public BodyDataPage(BodyDataViewModel vm)
    {
        InitializeComponent();
        _viewModel = vm;
        BindingContext = _viewModel;
        NavigationPage.SetHasNavigationBar(this, false);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        UpdateImtIndicatorPosition();
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(BodyDataViewModel.BmiResult) ||
            e.PropertyName == nameof(BodyDataViewModel.IsFormValid))
        {
            UpdateImtIndicatorPosition();
        }
    }

    private void UpdateImtIndicatorPosition()
    {
        double imt = _viewModel.BmiResult?.Value ?? 0;

        if (MyGrid.Width <= 0 || !_viewModel.IsFormValid)
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