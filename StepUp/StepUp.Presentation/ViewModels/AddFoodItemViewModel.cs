using StepUp.Application.Interfaces;
using StepUp.Presentation.BaseViewModels;
using StepUp.Presentation.Interfaces;

namespace StepUp.Presentation.ViewModels
{
    public partial class AddFoodItemViewModel(DayResultService dayService,
        IDialogService dialog, INavigationService nav, 
        IFoodProductClient productClient) : FoodItemBaseViewModel(dayService, dialog, nav)
    {
        private FoodProduct? _product;
        private readonly IFoodProductClient _productClient = productClient;

        public override string SaveButtonText => "Добавить";
        public override bool IsDeleteVisible => false;

        public async Task InitializeAsync(DateTime? date, MealType? mealType, string? productId) =>
            throw new NotImplementedException();
    }
}
