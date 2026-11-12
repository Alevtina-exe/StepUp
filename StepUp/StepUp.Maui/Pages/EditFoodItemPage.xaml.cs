using StepUp.Domain.Enums;

namespace StepUp.Maui.Pages;

public partial class EditFoodItemPage : ContentPage, IQueryAttributable
{
    private DateTime? _date = null;
    private MealType? _mealType = null;
    private Guid? _foodItemId = null;
    private readonly EditFoodItemViewModel _vm;
    public EditFoodItemPage(EditFoodItemViewModel vm)
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
        if (query.TryGetValue("FoodItemId", out var f) && f is Guid id) _foodItemId = id;
    }
    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _vm.InitializeAsync(_date, _mealType, _foodItemId);
    }
}