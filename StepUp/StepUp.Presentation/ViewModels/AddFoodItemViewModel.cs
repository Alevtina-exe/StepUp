using StepUp.Application.Exceptions;
using StepUp.Application.Interfaces;
using StepUp.Application.Interfaces.Services;
using StepUp.Presentation.BaseViewModels;
using StepUp.Presentation.Interfaces;

namespace StepUp.Presentation.ViewModels
{
    public partial class AddFoodItemViewModel(DayResultService dayService,
        IDialogService dialog, INavigationService nav, 
        IFoodProductClient productClient, ICurrentUserService currentUser) : 
        FoodItemBaseViewModel(dayService, dialog, nav, productClient, currentUser)
    {
        private FoodProduct? _product;

        public override string SaveButtonText => "Добавить";
        public override bool IsDeleteVisible => false;

        public async Task InitializeAsync(DateTime? date, MealType? mealType, string? productId)
        {
            if (date == null || mealType == null || productId == null)
            {
                await _nav.GoBackAsync();
                return;
            }

            await SetContextAsync(date.Value, mealType.Value);

            try
            {
                _product = await FillInProductPropertiesAsync(productId);

            }
            catch (FoodProductNotFoundException ex)
            {
                await _dialog.ShowAlertAsync("Ошибка", ex.Message) ;
                await _nav.GoBackAsync();
                return;
            }    
            
            if(_product.MainServing != null)
            {
                SelectedServing = _product.MainServing;
                Servings.Add(SelectedServing);
                if(SelectedServing.IsLiquid)
                {
                    Servings.Add(Serving.Milliliters(1));
                    Servings.Add(Serving.Cup(250));
                }
                else
                {
                    Servings.Add(Serving.Grams(1));
                }
            }
            else 
            {
                SelectedServing = Serving.Grams(1);
                Servings.Add(SelectedServing);
                Amount = "100";
            }
        }

        protected async override Task DoSaveAsync(double amount)
        {
            if (_product == null) return;
            await _dayService.AddFoodAsync(_date, _mealType,
                _product, SelectedServing, amount);
        }
    }
}
