using StepUp.Domain.Enums;

namespace StepUp.Maui.Pages;

public partial class AddFoodItemPage : ContentPage, IQueryAttributable
{
	private DateTime? _date = null;
	private MealType? _mealType = null;
	private string? _productId = null;
	private readonly AddFoodItemViewModel _vm;
	public AddFoodItemPage(AddFoodItemViewModel vm)
	{
		InitializeComponent();
		_vm = vm;
		BindingContext = _vm;
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Date", out var d) && d is DateTime date) 
			_date = date;
        if (query.TryGetValue("MealType", out var m) && m is MealType mt) 
			_mealType = mt;
        if (query.TryGetValue("FoodProductId", out var f) && f is string id) _productId = id;
    }
    protected async override void OnAppearing()
    {
        base.OnAppearing();
		await _vm.InitializeAsync(_date, _mealType, _productId);
    }
}