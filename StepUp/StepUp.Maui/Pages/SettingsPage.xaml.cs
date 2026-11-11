namespace StepUp.Maui.Pages;

public partial class SettingsPage : ContentPage
{
	private readonly SettingsViewModel _vm;
    public SettingsPage(SettingsViewModel vm)
	{
		_vm = vm;
		InitializeComponent();
		BindingContext = _vm;
	}
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _vm.InitializeAsync();
	}
}