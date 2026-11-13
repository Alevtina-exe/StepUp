using StepUp.Application.Interfaces.Services;
using StepUp.Domain.Calculators;
using StepUp.Presentation.Constants;
using StepUp.Presentation.Enums;
using StepUp.Presentation.Interfaces;

namespace StepUp.Presentation.ViewModels;

public partial class MainViewModel(DayResultService dayService, IClock clock, ICurrentUserService user,
    INavigationService nav, IDialogService dialog, IPopupFactory popup) : ObservableObject
{

    private User _user = null!;
    public async Task InitializeAsync()
    {
        try
        {
            _user = await user.GetOrLoadAsync();
            CurrDayRes = await dayService.GetOrCreateTodayAsync();
            MinDate = _user.BirthDate.Date;
        }
        catch (Exception ex)
        {
            await dialog.ShowAlertAsync("Ошибка", ex.Message, "Выйти");
            await nav.GoToPageAsync(AppRoutes.Login);
        }
    }

    [ObservableProperty]
    private DayResult _currDayRes = null!;


    [ObservableProperty]
    private bool _stepCounterVisible = true;


    partial void OnCurrDayResChanged(DayResult value)
    {
        if (value == null) return;
        StepCounterVisible = value.Date == clock.Today;
        DateString = value.Date.ToString("ddd, dd MMMM yyyy", new System.Globalization.CultureInfo("ru-RU"));
        CaloriePlan = value.CaloriePlan;
        KcalBurnt = CurrDayRes.KcalBurnt;
        RefreshMeals();
    }

    [ObservableProperty]
    private string _dateString = string.Empty;
    [ObservableProperty]
    public DateTime _date = clock.Today;

    public DateTime MaxDate => clock.Today;
    [ObservableProperty]
    private DateTime _minDate = clock.Today;


    [ObservableProperty]
    private int _caloriePlan = 0;
    [ObservableProperty]
    public int _kcalBurnt = 0;
    [ObservableProperty]
    public int _kcalEaten = 0;

    [ObservableProperty]
    private int _kcalRes = 0;


    [ObservableProperty]
    private int _brKcal = 0;
    [ObservableProperty]
    private double _brPr = 0;
    [ObservableProperty]
    private double _brFat = 0;
    [ObservableProperty]
    private double _brCb = 0;

    [ObservableProperty]
    private int _lnKcal = 0;
    [ObservableProperty]
    private double _lnPr = 0;
    [ObservableProperty]
    private double _lnFat = 0;
    [ObservableProperty]
    private double _lnCb = 0;

    [ObservableProperty]
    private int _dnKcal = 0;
    [ObservableProperty]
    private double _dnPr = 0;
    [ObservableProperty]
    private double _dnFat = 0;
    [ObservableProperty]
    private double _dnCb = 0;

    [ObservableProperty]
    private int _snKcal = 0;
    [ObservableProperty]
    private double _snPr = 0;
    [ObservableProperty]
    private double _snFat = 0;
    [ObservableProperty]
    private double _snCb = 0;

    [ObservableProperty]
    public double _totalProtein = 0;
    [ObservableProperty]
    public double _totalFat = 0;
    [ObservableProperty]
    public double _totalCarbon = 0;

    private double MaxProtein => MacroCalculator.CalculateProteinGrams(CaloriePlan + KcalBurnt, _user?.ProteinPercent ?? 0);
    private double MaxFat => MacroCalculator.CalculateFatGrams(CaloriePlan + KcalBurnt, _user?.FatPercent ?? 0);
    private double MaxCarbon => MacroCalculator.CalculateCarbonGrams(CaloriePlan + KcalBurnt, _user?.CarbonPercent ?? 0);


    public string ProteinString => $"{TotalProtein:F1} / {MaxProtein:F1} г";
    public string FatString => $"{TotalFat:F1} / {MaxFat:F1} г";
    public string CarbonString => $"{TotalCarbon:F1} / {MaxCarbon:F1} г";


    [ObservableProperty]
    private double _prProgress = 0;
    [ObservableProperty]
    private double _fatProgress = 0;
    [ObservableProperty]
    private double _cbProgress = 0;
    public double KcalProgress => (double)(KcalEaten > KcalBurnt ? KcalEaten - KcalBurnt : 0) / CaloriePlan;

    private void RefreshMeals()
    {
        BrKcal = CurrDayRes.Breakfast.TotalKcal;
        BrPr = CurrDayRes.Breakfast.TotalProtein;
        BrFat = CurrDayRes.Breakfast.TotalFat;
        BrCb = CurrDayRes.Breakfast.TotalCarbon;

        LnKcal = CurrDayRes.Lunch.TotalKcal;
        LnPr = CurrDayRes.Lunch.TotalProtein;
        LnFat = CurrDayRes.Lunch.TotalFat;
        LnCb = CurrDayRes.Lunch.TotalCarbon;

        DnKcal = CurrDayRes.Dinner.TotalKcal;
        DnPr = CurrDayRes.Dinner.TotalProtein;
        DnFat = CurrDayRes.Dinner.TotalFat;
        DnCb = CurrDayRes.Dinner.TotalCarbon;

        SnKcal = CurrDayRes.Snack.TotalKcal;
        SnPr = CurrDayRes.Snack.TotalProtein;
        SnFat = CurrDayRes.Snack.TotalFat;
        SnCb = CurrDayRes.Snack.TotalCarbon;

        KcalEaten = CurrDayRes.KcalEaten;
        TotalProtein = CurrDayRes.TotalProtein;
        TotalFat = CurrDayRes.TotalFat;
        TotalCarbon = CurrDayRes.TotalCarbon;

        RefreshProgress();
    }

    private void RefreshProgress()
    {
        KcalRes = CaloriePlan - KcalEaten + KcalBurnt;
        OnPropertyChanged(nameof(KcalProgress));

        PrProgress = MaxProtein > 0 ? TotalProtein / MaxProtein : 0;
        FatProgress = MaxFat > 0 ? TotalFat / MaxFat : 0;
        CbProgress = MaxCarbon > 0 ? TotalCarbon / MaxCarbon : 0;

        OnPropertyChanged(nameof(MaxProtein));
        OnPropertyChanged(nameof(MaxFat));
        OnPropertyChanged(nameof(MaxCarbon));

        OnPropertyChanged(nameof(ProteinString));
        OnPropertyChanged(nameof(FatString));
        OnPropertyChanged(nameof(CarbonString));
    }

    partial void OnDateChanged(DateTime value)
    {
        _ = LoadDayAsync(value);
    }

    private async Task LoadDayAsync(DateTime date)
    {
        try
        {
            CurrDayRes = await dayService.GetOrCreateDayAsync(date);
        }
        catch (Exception ex)
        {
            await dialog.ShowAlertAsync("Ошибка", ex.Message);
        }
    }

    [RelayCommand]
    private async Task GoToGraphAsync() => await nav.GoToPageAsync(AppRoutes.Graph);

    [RelayCommand]
    private async Task GoToSettingsAsync() => await nav.GoToPageAsync(AppRoutes.Settings);

    [RelayCommand]
    private async Task GoToSportsAsync() => await nav.GoToPageAsync(AppRoutes.Sports);

    [RelayCommand]
    private async Task GoToMealAsync(MealType mealType)
    {
        var parameters = new Dictionary<string, object>
        {
            { "Date", CurrDayRes.Date },
            { "MealType", mealType }
        };
        await nav.GoToPageAsync(AppRoutes.ViewMeal, parameters);
    }

    private async Task GoToAddProductAsync(MealType mealType)
    {
        var parameters = new Dictionary<string, object>
        {
            { "Date", CurrDayRes },
            { "MealType", mealType }
        };
        await nav.GoToPageAsync(AppRoutes.SearchProduct, parameters);
    }
    [RelayCommand]
    private async Task GoToMenuAsync()
    {
        var action = await dialog.ShowPopupAsync(popup.CreateMenuBarPopup());
        switch (action)
        {
            case MenuAction.ChangeWeight:
                await ChangeWeightAsync();
                break;
            case MenuAction.Breakfast:
                await GoToAddProductAsync(MealType.Breakfast);
                break;
            case MenuAction.Lunch:
                await GoToAddProductAsync(MealType.Lunch);
                break;
            case MenuAction.Dinner:
                await GoToAddProductAsync(MealType.Dinner);
                break;
            case MenuAction.Snack:
                await GoToAddProductAsync(MealType.Snack);
                break;
            case MenuAction.Sport:
                await AddSportAsync();
                break;
            default:
                return;
        }
        await LoadDayAsync(Date);
    }
    private async Task ChangeWeightAsync()
    {
        var input = await dialog.ShowPromptAsync("Вес", "Введите ваш вес (30-300 кг)", _user.Weight.ToString());

        if (double.TryParse(input, out double newWeight) && newWeight >= 30 && newWeight <= 300)
        {
            try
            {
                CurrDayRes = await dayService.UpdateWeightAsync(newWeight);
            }
            catch (Exception ex)
            {
                await dialog.ShowAlertAsync("Ошибка", ex.Message);
            }
        }
        else if (!string.IsNullOrEmpty(input))
        {
            await dialog.ShowAlertAsync("Ошибка", "Недопустимый вес", "OK");
        }
    }
    private async Task AddSportAsync()
    {
        var newSport = await dialog.ShowPopupAsync(popup.CreateAddSportPopup());
        if (newSport != null)
        {
            try
            {
                CurrDayRes = await dayService.AddSportAsync(CurrDayRes.Date, newSport.Name, newSport.Calories);
            }
            catch (Exception ex)
            {
                await dialog.ShowAlertAsync("Ошибка", ex.Message);
            }
        }
    }
}
