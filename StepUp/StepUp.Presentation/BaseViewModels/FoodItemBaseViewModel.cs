using StepUp.Application.Exceptions;
using StepUp.Application.Interfaces;
using StepUp.Application.Interfaces.Services;
using StepUp.Presentation.Constants;
using StepUp.Presentation.Interfaces;
using StepUp.Presentation.Models;
using System.Globalization;

namespace StepUp.Presentation.BaseViewModels
{
    public abstract partial class FoodItemBaseViewModel : ObservableObject
    {
        protected readonly DayResultService _dayService;
        protected readonly IDialogService _dialog;
        protected readonly INavigationService _nav;
        protected readonly ICurrentUserService _currentUser;
        protected readonly IFoodProductClient _productClient;


        protected DateTime _date;
        protected MealType _mealType;
        protected int _caloriePlan = 0;
        protected Nutrients _nutrientsPer100g = new(0,0,0,0);

        public IAsyncRelayCommand SaveCommand { get; }
        public IAsyncRelayCommand? DeleteCommand { get; }

        protected FoodItemBaseViewModel(
        DayResultService dayService,
        IDialogService dialog,
        INavigationService nav,
        IFoodProductClient productClient,
        ICurrentUserService currentUser)
        {
            _dayService = dayService;
            _dialog = dialog;
            _nav = nav;
            _productClient = productClient;
            _currentUser = currentUser;

            SaveCommand = new AsyncRelayCommand(SaveAsync);
            DeleteCommand = new AsyncRelayCommand(DeleteAsync);
        }

        [ObservableProperty]
        private string _imageUrl = string.Empty;
        [ObservableProperty]
        private string _name = string.Empty;
        [ObservableProperty]
        private string _brand = string.Empty;
        [ObservableProperty]
        private string _barcode = string.Empty;
        [ObservableProperty]
        private string _novaGroup = string.Empty;
        [ObservableProperty]
        private string _nutriscoreGrade = string.Empty;


        [ObservableProperty]
        private Nutrients _totalNutrients = new(0,0,0,0);
        [ObservableProperty]
        private double _progress = 0;
        [ObservableProperty]
        private double _dailyPercent = 0;
        [ObservableProperty]
        private double _proteinPercent = 0;
        [ObservableProperty]
        private double _fatPercent = 0;
        [ObservableProperty]
        private double _carbonPercent = 0;
        


        [ObservableProperty]
        private string _amount = string.Empty;
        [ObservableProperty]
        private MealOption _selectedMeal = MealOption.Breakfast;
        [ObservableProperty]
        private Serving _selectedServing = Serving.Grams(100);
        public List<Serving> Servings { get; protected set; } = [];
        public IReadOnlyList<MealOption> MealOptions { get; } = MealOption.All;


        [ObservableProperty]
        private string _errorMessage = string.Empty;
        [ObservableProperty]
        private bool _isErrorMessageVisible = false;
        [ObservableProperty]
        private bool _isSaveButtonActive = false;
        public abstract string SaveButtonText { get; }
        public abstract bool IsDeleteVisible { get; }


        protected void ShowError(string message)
        {
            ErrorMessage = message;
            IsErrorMessageVisible = true;
        }

        protected void HideError()
        {
            IsErrorMessageVisible = false;
            IsSaveButtonActive = true;
        }

        partial void OnAmountChanged(string value)
        {
            if (!TryParseAmount(out var x)) return;
            TotalNutrients = _nutrientsPer100g.CalculatePerAmount(SelectedServing * x);
            Progress = (double)TotalNutrients.Calories / _caloriePlan;
            DailyPercent = Progress * 100;
        }
        protected bool TryParseAmount(out double amount)
        {
            var normalized = Amount.Replace(',', '.');
            if (!double.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out amount)
                || amount <= 0)
            {
                ShowError("Введите корректное количество");
                IsSaveButtonActive = false;
                return false;
            }
            HideError();
            return true;
        }

        protected async Task SetContextAsync(DateTime date, MealType mealType)
        {
            _date = date;
            _mealType = mealType;
            try
            {
                var user = await _currentUser.GetOrLoadAsync();
                _caloriePlan = user.CaloriePlan;
            }
            catch (UserNotFoundException ex)
            {
                await _dialog.ShowAlertAsync("Ошибка", ex.Message);
                await _nav.GoBackAsync();
                return;
            }
        }

        protected async Task<FoodProduct> FillInProductPropertiesAsync(string productId)
        {
            var product = await _productClient.GetByBarcodeAsync(productId) ??
                throw new FoodProductNotFoundException();

            ImageUrl = product.ImageUrl ?? "no_image.jpeg";
            Name = product.Name;
            Brand = product.Brands;
            Barcode = product.Barcode;
            NovaGroup = product.NovaGroup ?? string.Empty;
            NutriscoreGrade = product.NutriscoreGrade ?? string.Empty;
            SelectedMeal = MealOptions.First(m => m.Type == _mealType);
            return product;
        }

        private async Task SaveAsync()
        {
            HideError();
            if (!TryParseAmount(out var amount)) return;

            try
            {
                await DoSaveAsync(amount);
                await _nav.GoToPageAsync(AppRoutes.Main);
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        private async Task DeleteAsync()
        {
            if (!IsDeleteVisible) return;
            if (!await _dialog.ShowConfirmationAsync("Удалить?", "Удалить продукт из приёма пищи?")) return;

            try
            {
                await DoDeleteAsync();
                await _nav.GoBackAsync();
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }


        protected abstract Task DoSaveAsync(double amount);
        protected virtual Task DoDeleteAsync() => throw new NotSupportedException();
    }
}
