using StepUp.Domain.Enums;

namespace StepUp.Maui.Pages;

public partial class SearchProductPage : ContentPage, IQueryAttributable
{
    private MealType? _mealType = null;
    private DateTime? _date = null;
    private readonly SearchFoodItemViewModel _vm;
	public SearchProductPage(SearchFoodItemViewModel vm)
	{
		InitializeComponent();
        _vm = vm;
		BindingContext = vm;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Date", out var d) && d is DateTime date)
            _date = date;
        if (query.TryGetValue("MealType", out var m) && m is MealType mt)
            _mealType = mt;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.InitializeAsync(_date, _mealType);
    }
}