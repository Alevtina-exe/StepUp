using StepUp.Application.Exceptions;
using StepUp.Application.Interfaces;
using StepUp.Application.Interfaces.Services;
using StepUp.Presentation.BaseViewModels;
using StepUp.Presentation.Interfaces;

namespace StepUp.Presentation.ViewModels
{
    public partial class EditFoodItemViewModel(DayResultService dayService,
        IDialogService dialog, INavigationService nav,
        IFoodProductClient productClient, ICurrentUserService currentUser) 
        : FoodItemBaseViewModel(dayService, dialog, nav, productClient, currentUser)
    {
        private FoodProduct? _product;
        private Guid _foodItemId;

        public override string SaveButtonText => "Добавить";
        public override bool IsDeleteVisible => false;

        public async Task InitializeAsync(DateTime? date, MealType? mealType, Guid? foodItemId)
        {
            if (date == null || mealType == null || foodItemId == null)
            {
                await _nav.GoBackAsync();
                return;
            }

            await SetContextAsync(date.Value, mealType.Value);

            _foodItemId = foodItemId.Value;
            var dayRes = await _dayService.GetOrCreateDayAsync(_date);
            var foodItem = dayRes.GetItemFromMeal(_mealType, _foodItemId);
            if(foodItem == null) {
                await _dialog.ShowAlertAsync("Ошибка", "Продукт не найден");
                await _nav.GoBackAsync();
                return;
            }

            try
            {
                _product = await FillInProductPropertiesAsync(
                    foodItem.Barcode ?? foodItem.ProductId);

            }
            catch (FoodProductNotFoundException ex)
            {
                await _dialog.ShowAlertAsync("Ошибка", ex.Message);
                await _nav.GoBackAsync();
                return;
            }

            SelectedServing = foodItem.Serving;
            Servings.Add(SelectedServing);
            if (SelectedServing.IsLiquid)
            {
                Servings.Add(Serving.Milliliters(1));
                Servings.Add(Serving.Cup(250));
            }
            else
            {
                Servings.Add(Serving.Grams(1));
            }
            Amount = foodItem.Amount.ToString("0.#");
        }

        protected override async Task DoSaveAsync(double amount)
        {
            if (_product == null) return;
            await _dayService.ChangeFoodAmountAsync(_date, _mealType,
                _foodItemId, amount, SelectedServing);
        }
        protected override async Task DoDeleteAsync()
        {
            await _dayService.RemoveFoodAsync(_date, _mealType, _foodItemId); ;
        }
    }
}
