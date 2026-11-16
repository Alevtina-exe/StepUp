using CommunityToolkit.Mvvm.Messaging;
using StepUp.Application.Interfaces.Services;
using StepUp.Domain.Calculators;
using StepUp.Presentation.Constants;
using StepUp.Presentation.Enums;
using StepUp.Presentation.Interfaces;
using StepUp.Presentation.Messages;

namespace StepUp.Presentation.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly DayResultService _dayService;
    private readonly IClock _clock;
    private readonly ICurrentUserService _userService;
    private readonly INavigationService _nav;
    private readonly IDialogService _dialog;
    private readonly IPopupFactory _popup;
    public MainViewModel(DayResultService dayService, IClock clock, ICurrentUserService user,
        INavigationService nav, IDialogService dialog, IPopupFactory popup)
    {
        _dayService = dayService;
        _clock = clock;
        _userService = user;
        _nav = nav;
        _dialog = dialog;
        _popup = popup;
        _date = clock.Today;
        _minDate = clock.Today;

        WeakReferenceMessenger.Default.Register<DayChangedMessage>(this, (r, m) =>
        {
            if (m.Value.Date == Date.Date)
                _ = InitializeAsync();
        });
    }


    private User _user = null!;
    [ObservableProperty]
    private DayResult _currDayRes = null!;
    [ObservableProperty]
    private string _dateString = string.Empty;
    [ObservableProperty]
    public DateTime _date;
    public DateTime MaxDate => _clock.Today;
    [ObservableProperty]
    private DateTime _minDate;


    public int BrKcal => CurrDayRes?.Breakfast.TotalKcal ?? 0;
    public double BrPr => CurrDayRes?.Breakfast.TotalProtein ?? 0;
    public double BrFat => CurrDayRes?.Breakfast.TotalFat ?? 0;
    public double BrCb => CurrDayRes?.Breakfast.TotalCarbon ?? 0;

    public int LnKcal => CurrDayRes?.Lunch.TotalKcal ?? 0;
    public double LnPr => CurrDayRes?.Lunch.TotalProtein ?? 0;
    public double LnFat => CurrDayRes?.Lunch.TotalFat ?? 0;   
    public double LnCb => CurrDayRes?.Lunch.TotalCarbon ?? 0;

    public int DnKcal => CurrDayRes?.Dinner.TotalKcal ?? 0;
    public double DnPr => CurrDayRes?.Dinner.TotalProtein ?? 0;
    public double DnFat => CurrDayRes?.Dinner.TotalFat ?? 0;
    public double DnCb => CurrDayRes?.Dinner.TotalCarbon ?? 0;

    public int SnKcal => CurrDayRes?.Snack.TotalKcal ?? 0;
    public double SnPr => CurrDayRes?.Snack.TotalProtein ?? 0;
    public double SnFat => CurrDayRes?.Snack.TotalFat ?? 0;
    public double SnCb => CurrDayRes?.Snack.TotalCarbon ?? 0;

    public int CaloriePlan => CurrDayRes?.CaloriePlan ?? 0;
    public int KcalEaten => CurrDayRes?.KcalEaten ?? 0;
    public int KcalBurnt =>  CurrDayRes?.KcalBurnt ?? 0;
    public int KcalRes => CaloriePlan - KcalEaten + KcalBurnt;
    public double KcalProgress => (double)(KcalEaten > KcalBurnt ? KcalEaten - KcalBurnt : 0) / CaloriePlan;

    public double TotalProtein => CurrDayRes?.TotalProtein ?? 0;
    public double TotalFat => CurrDayRes?.TotalFat ?? 0;
    public double TotalCarbon => CurrDayRes?.TotalCarbon ?? 0;

    public double MaxProtein => 
        MacroCalculator.CalculateProteinCarbonGrams(CaloriePlan + KcalBurnt, _user?.ProteinPercent ?? 0);
    public double MaxFat => 
        MacroCalculator.CalculateFatGrams(CaloriePlan + KcalBurnt, _user?.FatPercent ?? 0);
    public double MaxCarbon => 
        MacroCalculator.CalculateProteinCarbonGrams(CaloriePlan + KcalBurnt, _user?.CarbonPercent ?? 0);

    public double PrProgress =>  MaxProtein > 0 ? TotalProtein / MaxProtein : 0;
    public double FatProgress => MaxFat > 0 ? TotalFat / MaxFat : 0;
    public double CbProgress => MaxCarbon > 0 ? TotalCarbon / MaxCarbon : 0;

    [ObservableProperty]
    private double _prProgressBar;
    [ObservableProperty]
    private double _fatProgressBar;
    [ObservableProperty]
    private double _cbProgressBar;


    public string ProteinString => $"{TotalProtein:F1} / {MaxProtein:F1} г";
    public string FatString => $"{TotalFat:F1} / {MaxFat:F1} г";
    public string CarbonString => $"{TotalCarbon:F1} / {MaxCarbon:F1} г";


    public async Task InitializeAsync()
    {
        try
        {
            _user = await _userService.GetOrLoadAsync();
            CurrDayRes = await _dayService.GetOrCreateDayAsync(Date);
            MinDate = _user.RegistrationDate.Date;
        }
        catch (Exception ex)
        {
            await _dialog.ShowAlertAsync("Ошибка", ex.Message, "Выйти");
            await _nav.GoToPageAsync(AppRoutes.Login);
        }
    }

    partial void OnCurrDayResChanged(DayResult value)
    {
        if (value == null) return;
        DateString = value.Date.ToString("ddd, dd MMMM yyyy", new System.Globalization.CultureInfo("ru-RU"));
        RefreshAll();
    }

    private void RefreshAll()
    {
        OnPropertyChanged(nameof(BrKcal));
        OnPropertyChanged(nameof(BrPr));
        OnPropertyChanged(nameof(BrFat));
        OnPropertyChanged(nameof(BrCb));

        OnPropertyChanged(nameof(LnKcal));
        OnPropertyChanged(nameof(LnPr));
        OnPropertyChanged(nameof(LnFat));
        OnPropertyChanged(nameof(LnCb));

        OnPropertyChanged(nameof(DnKcal));
        OnPropertyChanged(nameof(DnPr));
        OnPropertyChanged(nameof(DnFat));
        OnPropertyChanged(nameof(DnCb));

        OnPropertyChanged(nameof(SnKcal));
        OnPropertyChanged(nameof(SnPr));
        OnPropertyChanged(nameof(SnFat));
        OnPropertyChanged(nameof(SnCb));

        OnPropertyChanged(nameof(CaloriePlan));
        OnPropertyChanged(nameof(KcalEaten));
        OnPropertyChanged(nameof(KcalBurnt));
        OnPropertyChanged(nameof(KcalRes));
        OnPropertyChanged(nameof(KcalProgress));

        OnPropertyChanged(nameof(TotalProtein));
        OnPropertyChanged(nameof(TotalFat));
        OnPropertyChanged(nameof(TotalCarbon));

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
            CurrDayRes = await _dayService.GetOrCreateDayAsync(date);
        }
        catch (Exception ex)
        {
            await _dialog.ShowAlertAsync("Ошибка", ex.Message);
        }
    }


    [RelayCommand]
    private async Task GoToGraphAsync() => await _nav.GoToPageAsync(AppRoutes.Graph);

    [RelayCommand]
    private async Task GoToSettingsAsync() => await _nav.GoToPageAsync(AppRoutes.Settings);

    [RelayCommand]
    private async Task GoToSportsAsync() 
        => await _nav.GoToPageAsync(AppRoutes.Sports, new Dictionary<string, object>
        {
            {"Date", CurrDayRes.Date }
        });

    [RelayCommand]
    private async Task GoToMealAsync(MealType mealType)
    {
        var parameters = new Dictionary<string, object>
        {
            { "Date", CurrDayRes.Date },
            { "MealType", mealType }
        };
        await _nav.GoToPageAsync(AppRoutes.ViewMeal, parameters);
    }

    [RelayCommand]
    private async Task GoToMenuAsync()
    {
        var action = await _dialog.ShowPopupAsync(_popup.CreateMenuBarPopup());
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
    }

    private async Task ChangeWeightAsync()
    {
        var input = await _dialog.ShowPromptAsync("Вес", "Введите ваш вес (30-300 кг)", _user.Weight.ToString());

        if (double.TryParse(input, out double newWeight) && newWeight >= 30 && newWeight <= 300)
        {
            try
            {
                CurrDayRes = await _dayService.UpdateWeightAsync(newWeight);
            }
            catch (Exception ex)
            {
                await _dialog.ShowAlertAsync("Ошибка", ex.Message);
            }
        }
        else if (!string.IsNullOrEmpty(input))
        {
            await _dialog.ShowAlertAsync("Ошибка", "Недопустимый вес", "OK");
        }
    }
    private async Task GoToAddProductAsync(MealType mealType)
    {
        var parameters = new Dictionary<string, object>
        {
            { "Date", CurrDayRes.Date },
            { "MealType", mealType }
        };
        await _nav.GoToPageAsync(AppRoutes.SearchProduct, parameters);
    }
    private async Task AddSportAsync()
    {
        var newSport = await _dialog.ShowPopupAsync(_popup.CreateAddSportPopup());
        if (newSport != null)
        {
            try
            {
                CurrDayRes = await _dayService.AddSportAsync(CurrDayRes.Date, newSport.Name, newSport.Calories);
            }
            catch (Exception ex)
            {
                await _dialog.ShowAlertAsync("Ошибка", ex.Message);
            }
        }
    }
}
