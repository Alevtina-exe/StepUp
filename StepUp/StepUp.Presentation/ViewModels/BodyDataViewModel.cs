using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StepUp.Application.Interfaces.Services;
using StepUp.Application.Models;
using StepUp.Application.Services;
using StepUp.Domain.Enums;
using StepUp.Presentation.Interfaces;
using StepUp.Presentation.Models;

namespace StepUp.Presentation.ViewModels;

public partial class BodyDataViewModel : ObservableObject, IParameterReceiver<RegisterRequest>
{
    private readonly AuthService _authService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;
    private readonly IClock _clock;

    private RegisterRequest? _request;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
    private bool _isStepTwo = false; 

    [ObservableProperty]
    private bool _errorMessageVisible;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
    private bool _isFormValid;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BmiValue), nameof(ImtCategory), nameof(ImtCategoryText))]
    private string _weightInput = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BmiValue), nameof(ImtCategory), nameof(ImtCategoryText))]
    private string _heightInput = string.Empty;

    [ObservableProperty]
    private SportLevelOption _selectedSportLevel = SportLevelOption.No;
    public IReadOnlyList<SportLevelOption> SportLevels { get; } = SportLevelOption.All;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
    private string _proteinInput = "30";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
    private string _fatInput = "30";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
    private string _carbonInput = "40";

    [ObservableProperty]
    private int _caloriePlan;


    public double BmiValue
    {
        get
        {
            if (double.TryParse(WeightInput, out var w) && double.TryParse(HeightInput, out var h) && h > 0)
            {
                double heightM = h / 100.0;
                return Math.Round(w / (heightM * heightM), 1);
            }
            return 0;
        }
    }

    public ImtCategory ImtCategory => GetCategory(BmiValue);

    public string ImtCategoryText => ImtCategory switch
    {
        ImtCategory.Deficit => "Дефицит веса",
        ImtCategory.Normal => "Нормальный вес",
        ImtCategory.Overweight => "Избыточный вес",
        ImtCategory.Obese1 => "Ожирение 1 степени",
        ImtCategory.Obese2 => "Ожирение 2 степени",
        _ => "Ожирение 3 степени"
    };

    public BodyDataViewModel(
        AuthService authService,
        INavigationService navigationService,
        IDialogService dialogService,
        IClock clock)
    {
        _authService = authService;
        _navigationService = navigationService;
        _dialogService = dialogService;
        _clock = clock;
    }

    public void ReceiveParameter(RegisterRequest request)
    {
        _request = request;
    }


    partial void OnWeightInputChanged(string value) => ValidateData();
    partial void OnHeightInputChanged(string value) => ValidateData();
    partial void OnSelectedSportLevelChanged(SportLevelOption value) => ValidateData();
    partial void OnProteinInputChanged(string value) => ValidatePfc();
    partial void OnFatInputChanged(string value) => ValidatePfc();
    partial void OnCarbonInputChanged(string value) => ValidatePfc();

    private void ValidateData()
    {
        ErrorMessageVisible = false;

        bool validW = double.TryParse(WeightInput, out var w) && w >= 30 && w <= 300;
        bool validH = double.TryParse(HeightInput, out var h) && h >= 100 && h <= 250;

        IsFormValid = validW && validH;

        if (IsStepTwo)
            ValidatePfc();
        else
            ContinueCommand.NotifyCanExecuteChanged();
    }

    private void ValidatePfc()
    {
        ErrorMessageVisible = false;
        bool validP = int.TryParse(ProteinInput, out var p) && p > 0;
        bool validF = int.TryParse(FatInput, out var f) && f > 0;
        bool validC = int.TryParse(CarbonInput, out var c) && c > 0;

        if (!validP || !validF || !validC || (p + f + c != 100))
        {
            IsFormValid = false;
        }
        else
        {
            IsFormValid = true;
        }
        ContinueCommand.NotifyCanExecuteChanged();
    }

    private ImtCategory GetCategory(double imt) => imt switch
    {
        < 18.9 => ImtCategory.Deficit,
        < 24.9 => ImtCategory.Normal,
        < 29.9 => ImtCategory.Overweight,
        < 34.9 => ImtCategory.Obese1,
        < 39.9 => ImtCategory.Obese2,
        _ => ImtCategory.Obese3
    };


    [RelayCommand(CanExecute = nameof(CanContinue), AllowConcurrentExecutions = false)]
    private async Task ContinueAsync()
    {
        if (!IsStepTwo)
        {
            CaloriePlan = CalculateCalories();
            IsStepTwo = true;
            ValidatePfc(); 
        }
        else
        {
            if (_request is null) return;

            _request.Weight = double.Parse(WeightInput);
            _request.Height = double.Parse(HeightInput);
            _request.SportLevel = SelectedSportLevel.Value;
            _request.CaloriePlan = CaloriePlan;
            _request.ProteinPercent = int.Parse(ProteinInput);
            _request.FatPercent = int.Parse(FatInput);
            _request.CarbonPercent = int.Parse(CarbonInput);

            try
            {
                await _authService.RegisterAsync(_request);
                await _navigationService.GoToPageAsync(AppRoutes.Main);
            }
            catch 
            {
                ErrorMessageVisible = true;
            }
        }
    }

    private bool CanContinue()
    {
        if (!IsFormValid) return false;

        if (IsStepTwo)
        {
            return int.TryParse(ProteinInput, out var p) && p > 0 &&
                   int.TryParse(FatInput, out var f) && f > 0 &&
                   int.TryParse(CarbonInput, out var c) && c > 0 &&
                   (p + f + c == 100);
        }
        return true;
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        ErrorMessageVisible = false;
        if (!IsStepTwo)
        {
            await _navigationService.GoBackAsync();
        }
        else
        {
            IsStepTwo = false;
            ValidateData(); 
        }
    }

    [RelayCommand]
    private async Task EditCaloriesAsync()
    {
        ErrorMessageVisible = false;
        var defaultKcal = CaloriePlan.ToString();

        var input = await _dialogService.ShowPromptAsync("Введите желаемый калораж", "Значение не должно быть меньше 1300 ккал", defaultKcal);

        if (string.IsNullOrEmpty(input)) return;

        if (int.TryParse(input, out int newKcal) && newKcal >= 1300)
        {
            CaloriePlan = newKcal;
        }
        else
        {
            await _dialogService.ShowAlertAsync("Ошибка", "Калораж введён неверно! Попробуйте снова.");
        }
    }

    private int CalculateCalories()
    {
        if (_request is null) return 2000;

        var today = _clock.Today;
        int age = today.Year - _request.BirthDate.Year;
        if (_request.BirthDate.Date > today.AddYears(-age)) age--;

        double ko = SelectedSportLevel.Value switch
        {
            SportLevel.No => 1.2,
            SportLevel.Low => 1.375,
            SportLevel.Mid => 1.55,
            SportLevel.High => 1.7,
            SportLevel.Extreme => 1.9,
            _ => 1.2
        };

        double res;
        if (_request.Gender == Gender.Male)
            res = 66.5 + (13.75 * double.Parse(WeightInput)) + (5.003 * double.Parse(HeightInput)) - (6.775 * age);
        else
            res = 655.1 + (9.563 * double.Parse(WeightInput)) + (1.85 * double.Parse(HeightInput)) - (4.676 * age);

        return (int)(res * ko);
    }
}