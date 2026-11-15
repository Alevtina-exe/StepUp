using StepUp.Domain.Enums;

namespace StepUp.Maui.Pages;

public partial class SearchProductPage : ContentPage, IQueryAttributable
{
    private MealType? _mealType = null;
    private DateTime? _date = null;
    private string? _barcode = null;
    private readonly SearchProductViewModel _vm;
	public SearchProductPage(SearchProductViewModel vm)
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
        if (query.TryGetValue("Barcode", out var b) && b is string barcode)
            _barcode = barcode;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.InitializeAsync(_date, _mealType, _barcode);
    }
}