using StepUp.Domain.Enums;

namespace StepUp.Maui.Pages;

public partial class MealListPage : ContentPage, IQueryAttributable
{
	private readonly MealListViewModel _vm;
	private DateTime? _currentDate = null;
	private MealType? _mealType = null;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Date", out var dateObj) && dateObj is DateTime date)
        {
            _currentDate = date;
        }

        if (query.TryGetValue("MealType", out var mealObj) && mealObj is MealType meal)
        {
            _mealType = meal;
        }
    }
    public MealListPage(MealListViewModel vm)
	{
		_vm = vm;
		InitializeComponent();
        BindingContext = _vm;
	}
	protected override async void OnAppearing() 
	{
		base.OnAppearing();
		await _vm.InitializeAsync(_currentDate, _mealType);
	}
}