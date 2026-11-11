using StepUp.Domain.Extensions;
using StepUp.Presentation.Constants;
using StepUp.Presentation.Interfaces;
using System.Collections.ObjectModel;

namespace StepUp.Presentation.ViewModels
{
    public partial class MealListViewModel(DayResultService dayService,
        INavigationService nav) : ObservableObject
    {
        [ObservableProperty]
        private string _mealName = string.Empty;
        public MealType Meal { get; private set; }
        [ObservableProperty]
        private DayResult? _dayRes;
        [ObservableProperty]
        private ObservableCollection<FoodItem> _dishes = [];

        public async Task InitializeAsync(DateTime? date, MealType? meal)
        {
            if (date == null || meal == null)
            {
                await nav.GoBackAsync();
                return;
            }
            DayRes = await dayService.GetDayAsync(date.Value);
            MealName = meal.Value.GetDescription();
            Meal = meal.Value;
            if (DayRes == null)
            {
                await nav.GoBackAsync();
            }
        }

        partial void OnDayResChanged(DayResult? value)
        {
            if (value == null) return;
            Dishes = new ObservableCollection<FoodItem>(value.Meals.First(m => m.MealType == Meal).Items);
        }
        [RelayCommand]
        private async Task EditFoodItemAsync(FoodItem item)
        {
            if (item == null || DayRes == null) return;
            await nav.GoToPageAsync(AppRoutes.EditProduct,
                new Dictionary<string, object>
                {
                    { "Date", DayRes.Date},
                    { "MealType", Meal },
                    { "FoodItemId", item.Id }
                });
        }
    }
}
