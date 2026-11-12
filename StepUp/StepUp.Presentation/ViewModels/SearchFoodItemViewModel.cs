using StepUp.Application.Interfaces;
using StepUp.Presentation.Constants;
using StepUp.Presentation.Interfaces;
using System.Collections.ObjectModel;

namespace StepUp.Presentation.ViewModels
{
    public partial class SearchFoodItemViewModel(IFoodProductClient productClient,
        INavigationService nav, IDialogService dialog) : ObservableObject
    {
        private DateTime _date;
        private MealType _meal;

        [ObservableProperty]
        private string _query = string.Empty;
        [ObservableProperty]
        private bool _searchErrorVisible = false;

        public ObservableCollection<FoodProduct> Products { get; private set; } = [];

        public async Task InitializeAsync(DateTime? date, MealType? mealType)
        {
            if (date == null || mealType == null)
            {
                await dialog.ShowAlertAsync("Ошибка", "Не удалось получить дату или приём пищи.");
                await nav.GoBackAsync();
                return;
            }
            _date = date.Value;
            _meal = mealType.Value;
        }
        partial void OnQueryChanged(string value)
        {
            SearchErrorVisible = false;
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            Products.Clear();
            var products = await productClient.SearchByNameAsync(Query);
            if (products == null || products.Count == 0)
                SearchErrorVisible = true;
            else Products = new ObservableCollection<FoodProduct>(products);
        }

        [RelayCommand]
        private async Task GoToBarcodeScannerAsync()
        {
            await nav.GoToPageAsync(AppRoutes.BarcodeScanner,
                new Dictionary<string, object>
                {
                    {"Meal", _meal },
                    {"Date", _date }
                });
        }
        [RelayCommand]
        private async Task GoToAddProductAsync(FoodProduct? product)
        {
            if (product == null) return;
            await nav.GoToPageAsync(AppRoutes.AddProduct, new Dictionary<string, object>
            {
                { "Meal", _meal },
                { "Date", _date },
                { "FoodProductId", product.Id }
            });
        }
    }
}
