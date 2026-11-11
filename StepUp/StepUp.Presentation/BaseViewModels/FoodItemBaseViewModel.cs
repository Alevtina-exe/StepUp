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

        protected DateTime _date;
        protected MealType _mealType;

        public IAsyncRelayCommand SaveCommand { get; }
        public IAsyncRelayCommand? DeleteCommand { get; }

        protected FoodItemBaseViewModel(
        DayResultService dayService,
        IDialogService dialog,
        INavigationService nav)
        {
            _dayService = dayService;
            _dialog = dialog;
            _nav = nav;

            SaveCommand = new AsyncRelayCommand(SaveAsync);
            DeleteCommand = new AsyncRelayCommand(DeleteAsync);
        }


        [ObservableProperty]
        private string _imageUrl = string.Empty;
        [ObservableProperty]
        private string _name = string.Empty;
        [ObservableProperty]
        private int _totalKcal = 0;
        [ObservableProperty]
        private double _progress = 0;
        [ObservableProperty]
        private double _dailyPercent = 0;
        [ObservableProperty]
        private double _totalProtein = 0;
        [ObservableProperty]
        private double _totalFat = 0;
        [ObservableProperty]
        private double _totalCarbon = 0;
        [ObservableProperty]
        private string _brand = string.Empty;
        [ObservableProperty]
        private string _barcode = string.Empty;
        [ObservableProperty]
        private int _novaGroup = 0;
        [ObservableProperty]
        private string _nutriscoreGrade = string.Empty;
        [ObservableProperty]
        private string _amount = string.Empty;
        [ObservableProperty]
        private MealOption _selectedMeal = MealOption.Breakfast;
        [ObservableProperty]
        private Serving _selectedServing = Serving.Grams(100);

        public abstract IReadOnlyList<Serving> Servings { get; protected set; } 
        public IReadOnlyList<MealOption> MealOptions { get; } = MealOption.All;


        [ObservableProperty]
        private string _errorMessage = string.Empty;
        [ObservableProperty]
        private bool _isErrorMessageVisible = false;
        public abstract string SaveButtonText { get; }
        public abstract bool IsDeleteVisible { get; }


        protected void ShowError(string message)
        {
            ErrorMessage = message;
            IsErrorMessageVisible = true;
        }

        protected void HideError() => IsErrorMessageVisible = false;
        protected bool TryParseAmount(out double amount)
        {
            var normalized = Amount.Replace(',', '.');
            if (!double.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out amount)
                || amount <= 0)
            {
                ShowError("Введите корректное количество");
                return false;
            }
            return true;
        }

        protected Task<DayResult> LoadDayAsync(CancellationToken ct = default)
            => _dayService.GetOrCreateDayAsync(_date, ct);

        private async Task SaveAsync()
        {
            HideError();
            if (!TryParseAmount(out var amount)) return;

            try
            {
                await DoSaveAsync(amount);
                await _nav.GoBackAsync();
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
