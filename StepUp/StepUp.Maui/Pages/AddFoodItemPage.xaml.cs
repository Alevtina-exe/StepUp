namespace StepUp.Maui.Pages;

public partial class AddFoodItemPage : ContentPage
{
	private readonly AddFoodItemViewModel _vm;
	public AddFoodItemPage(AddFoodItemViewModel vm)
	{
		InitializeComponent();
		_vm = vm;
		BindingContext = _vm;
	}
}