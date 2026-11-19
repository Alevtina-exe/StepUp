namespace StepUp.Maui.Pages;

public partial class GraphPage : ContentPage
{
	private readonly GraphViewModel _vm;
	public GraphPage(GraphViewModel vm)
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