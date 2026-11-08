using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StepUp.Application.Interfaces.Services;
using StepUp.Application.Services;
using StepUp.Domain.Calculators;
using StepUp.Domain.Entities;
using StepUp.Domain.Enums;
using StepUp.Presentation.Constants;
using StepUp.Presentation.Interfaces;

namespace StepUp.Presentation.ViewModels;

public partial class MainViewModel(DayResultService dayService, IClock clock, ICurrentUserService user,
    INavigationService nav, IDialogService dialog) : ObservableObject
{

    private User _user = null!;
    public async Task InitializeAsync()
    {
        try
        {
            _user = await user.GetOrLoadAsync();
            CurrDayRes = await dayService.GetOrCreateTodayAsync();
            MinDate = _user.RegistrationDate.Date;
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
        DateString = CurrDayRes.Date.ToString("ddd, dd MMMM yyyy", new System.Globalization.CultureInfo("ru-RU"));

        MaxProtein = MacroCalculator.CalculateProteinGrams(CurrDayRes.CaloriePlan, _user.ProteinPercent);
        MaxFat = MacroCalculator.CalculateFatGrams(CurrDayRes.CaloriePlan, _user.FatPercent);
        MaxCarbon = MacroCalculator.CalculateCarbonGrams(CurrDayRes.CaloriePlan, _user.CarbonPercent);

        RefreshMeals();
        RefreshTotalNutrients();
        RefreshSport();
    }

    [ObservableProperty]
    private string _dateString = string.Empty;
    [ObservableProperty]
    public DateTime _date = clock.Today;

    public DateTime MaxDate => clock.Today;
    [ObservableProperty]
    private DateTime _minDate = clock.Today;

    [ObservableProperty]
    private double _maxProtein = 1;
    [ObservableProperty]
    private double _maxCarbon = 1;
    [ObservableProperty]
    private double _maxFat = 1;

    [ObservableProperty]
    public int _kcalBurnt = 0;
    [ObservableProperty]
    public int _kcalEaten = 0;
    [ObservableProperty]
    public int _kcalRes = 0;
    [ObservableProperty] 
    private double _kcalProgress;

    //'{0} ккал' 'Б: {0}' 'Ж: {0}' 'У: {0}'

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

    public string ProteinString => $"{TotalProtein:F1} / {MaxProtein:F1} г";
    public string FatString => $"{TotalFat:F1} / {MaxFat:F1} г";
    public string CarbonString => $"{TotalCarbon:F1} / {MaxCarbon:F1} г";

    [ObservableProperty]
    private double _prProgress = 0;
    [ObservableProperty]
    private double _fatProgress = 0;
    [ObservableProperty]
    public double _cbProgress = 0;


    public void RefreshTotalNutrients()
    {
        TotalProtein = CurrDayRes.TotalProtein;
        TotalFat = CurrDayRes.TotalFat;
        TotalCarbon = CurrDayRes.TotalCarbon;

        PrProgress = MaxProtein > 0 ? TotalProtein / MaxProtein : 0;
        FatProgress = MaxFat > 0 ? TotalFat / MaxFat : 0;
        CbProgress = MaxCarbon > 0 ? TotalCarbon / MaxCarbon : 0;

        KcalEaten = CurrDayRes.KcalEaten;
        KcalRes = CurrDayRes.KcalRes;

        KcalProgress = (double)KcalEaten / CurrDayRes.CaloriePlan;
    }
    public void RefreshMeals ()
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

        OnPropertyChanged(nameof(ProteinString));
        OnPropertyChanged(nameof(FatString));
        OnPropertyChanged(nameof(CarbonString));
    }
    public void RefreshSport()
    {
        KcalBurnt = CurrDayRes.KcalBurnt;
    }

    partial void OnDateChanged(DateTime value)
    {
        _ = LoadDayAsync(value); 
    }

    private async Task LoadDayAsync(DateTime date)
    {
        CurrDayRes = await dayService.GetOrCreateDayAsync(date);
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
            { "MealType", mealType }
        };
        await nav.GoToPageAsync(AppRoutes.ViewMeal, parameters);
    }

    [RelayCommand]
    private async Task HandleMenuSelectionAsync(int type)
    {
        switch (type)
        {
            case 1:
                await ChangeWeightAsync();
                break;
            case 2:
                await nav.GoToPageAsync(AppRoutes.AddProduct, new Dictionary<string, object> { { "MealType", MealType.Breakfast } });
                break;
            case 3:
                await nav.GoToPageAsync(AppRoutes.AddProduct, new Dictionary<string, object> { { "MealType", MealType.Lunch } });
                break;
            case 4:
                await nav.GoToPageAsync(AppRoutes.AddProduct, new Dictionary<string, object> { { "MealType", MealType.Dinner } });
                break;
            case 5:
                await nav.GoToPageAsync(AppRoutes.AddProduct, new Dictionary<string, object> { { "MealType", MealType.Snack } });
                break;
            case 6:
                await dialog.ShowAlertAsync("Спорт", "Здесь будет окно добавления спорта");
                break;
        }

        await LoadDayAsync(Date);
    }
    private async Task ChangeWeightAsync()
    {
        var input = await dialog.ShowPromptAsync("Вес", "Введите ваш вес (30-300 кг)", _user.Weight.ToString());

        if (double.TryParse(input, out double newWeight) && newWeight >= 30 && newWeight <= 300)
        {
            await dayService.UpdateWeightAsync(newWeight);
        }
        else if (!string.IsNullOrEmpty(input))
        {
            await dialog.ShowAlertAsync("Ошибка", "Недопустимый вес", "OK");
        }
    }
}
