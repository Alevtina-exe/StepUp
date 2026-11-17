using StepUp.Application.Interfaces;
using StepUp.Presentation.Constants;
using StepUp.Presentation.Interfaces;
using System.Collections.ObjectModel;

namespace StepUp.Presentation.ViewModels
{
    public partial class SearchProductViewModel(IFoodProductClient productClient,
        INavigationService nav, IDialogService dialog) : ObservableObject
    {
        private DateTime _date;
        private MealType _meal;

        [ObservableProperty]
        private string _query = string.Empty;
        [ObservableProperty]
        private bool _searchErrorVisible = false;
        [ObservableProperty]
        private FoodProduct? _selectedItem; 

        public ObservableCollection<FoodProduct> Products { get; private set; } = [];

        public async Task InitializeAsync(DateTime? date, MealType? mealType, string? barcode = null)
        {
            if (date == null || mealType == null)
            {
                await dialog.ShowAlertAsync("Ошибка", "Не удалось получить дату или приём пищи.");
                await nav.GoBackAsync();
                return;
            }
            if (!string.IsNullOrEmpty(barcode))
            {
                Query = barcode;
                await SearchAsync();   
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
            var products = await productClient.SearchByNameAsync(Query.Trim());
            if (products == null || products.Count == 0)
                SearchErrorVisible = true;
            else
            {
                SearchErrorVisible = false;
                Products = new ObservableCollection<FoodProduct>(products);
                OnPropertyChanged(nameof(Products));
            }
        }

        [RelayCommand]
        private async Task GoToBarcodeScannerAsync()
        {
            await nav.GoToPageAsync(AppRoutes.BarcodeScanner,
                new Dictionary<string, object>
                {
                    {"MealType", _meal },
                    {"Date", _date }
                });
        }
        [RelayCommand]
        private async Task GoToAddProductAsync(FoodProduct? product)
        {
            if (product == null) return;
            await nav.GoToPageAsync(AppRoutes.AddProduct, new Dictionary<string, object>
            {
                { "MealType", _meal },
                { "Date", _date },
                { "FoodProductId", product.Id }
            });
        }
    }
}
