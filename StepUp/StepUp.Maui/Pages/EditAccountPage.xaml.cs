namespace StepUp.Maui.Pages;

public partial class EditAccountPage : ContentPage
{
    private readonly EditAccountViewModel _vm;

    public EditAccountPage(EditAccountViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.InitializeAsync();
    }
}