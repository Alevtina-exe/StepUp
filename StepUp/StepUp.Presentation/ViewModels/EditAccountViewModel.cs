using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StepUp.Application.Interfaces.Services;
using StepUp.Application.Services;
using StepUp.Domain.Enums;
using StepUp.Presentation.Interfaces;
using StepUp.Presentation.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace StepUp.Presentation.ViewModels
{
    public partial class EditAccountViewModel(AuthService auth, IClock clock,
        ICurrentUserService userService, INavigationService nav) : ObservableObject
    {
        [ObservableProperty]
        private string _name = string.Empty;
        [ObservableProperty]
        private GenderOption _gender = null!;
        [ObservableProperty]
        private DateTime _birthday = DateTime.Now;
        public DateTime MaxDate => clock.Today.AddYears(-10);
        public DateTime MinDate => clock.Today.AddYears(-120);
        [ObservableProperty]
        private string _height = string.Empty;
        [ObservableProperty]
        private string _weight = string.Empty;
        [ObservableProperty]
        private string _caloriePlan = string.Empty;
        [ObservableProperty]
        private string _proteinPercent = string.Empty;
        [ObservableProperty]
        private string _fatPercent = string.Empty;    
        [ObservableProperty]
        private string _carbonPercent = string.Empty;
        public IReadOnlyList<GenderOption> Genders { get; } = GenderOption.All;

        [ObservableProperty] 
        private bool _isNameValid = true;
        [ObservableProperty] 
        private bool _isHeightValid = true;
        [ObservableProperty] 
        private bool _isWeightValid = true;
        [ObservableProperty] 
        private bool _isCaloriePlanValid = true;
        [ObservableProperty] 
        private bool _isMacrosValid = true;


        [ObservableProperty]
        private string _errorMessage = string.Empty;
        [ObservableProperty]
        private bool _isErrorMessageVisible = false;

        public async Task InitializeAsync()
        {
            var user = await userService.GetOrLoadAsync();
            Name = user.FullName;
            Gender = user.Gender == Domain.Enums.Gender.Male ? 
                GenderOption.Male : GenderOption.Female;
            Birthday = user.BirthDate;
            Height = user.Height.ToString();
            Weight = user.Weight.ToString("0.#");
            CaloriePlan = user.CaloriePlan.ToString();
            ProteinPercent = user.ProteinPercent.ToString();
            FatPercent = user.FatPercent.ToString();
            CarbonPercent = user.CarbonPercent.ToString();
        }

        partial void OnNameChanged(string value)
        {
            IsNameValid = !string.IsNullOrWhiteSpace(value) &&
                          Regex.IsMatch(value, @"^[a-zA-Zа-яА-ЯёЁ\s]+$");
            IsErrorMessageVisible = false;
        }

        partial void OnHeightChanged(string value)
        {
            IsHeightValid = int.TryParse(value, out int h) && h >= 100 && h <= 250;
            IsErrorMessageVisible = false;
        }

        partial void OnWeightChanged(string value)
        {
            var normalized = value.Replace(',', '.');
            IsWeightValid = double.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out double w)
                            && w >= 30 && w <= 300;
            IsErrorMessageVisible = false;
        }

        partial void OnCaloriePlanChanged(string value)
        {
            IsCaloriePlanValid = int.TryParse(value, out int c) && c >= 1300;
            IsErrorMessageVisible = false;
        }

        partial void OnProteinPercentChanged(string value) => ValidateMacros();
        partial void OnFatPercentChanged(string value) => ValidateMacros();
        partial void OnCarbonPercentChanged(string value) => ValidateMacros();

        private void ValidateMacros()
        {
            bool pValid = int.TryParse(ProteinPercent, out int p) && p > 0;
            bool fValid = int.TryParse(FatPercent, out int f) && f > 0;
            bool cbValid = int.TryParse(CarbonPercent, out int cb) && cb > 0;

            IsMacrosValid = pValid && fValid && cbValid && (p + f + cb == 100);
            IsErrorMessageVisible = false;
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await nav.GoBackAsync();
        }

        [RelayCommand]
        private async Task SaveChangesAsync()
        {
            ErrorMessage = string.Empty;
            IsErrorMessageVisible = false;

            if (!IsNameValid)
                ErrorMessage += "Имя пустое или содержит недопустимые символы (только буквы).\n";

            if (!IsHeightValid)
                ErrorMessage += "Рост должен быть целым числом от 100 до 250 см.\n";

            if (!IsWeightValid)
                ErrorMessage += "Вес должен быть числом от 30 до 300 кг.\n";

            if (!IsCaloriePlanValid)
                ErrorMessage += "Дневная норма калорий не может быть меньше 1300.\n";

            if (!IsMacrosValid)
                ErrorMessage += "Сумма БЖУ должна быть ровно 100% (все значения > 0).\n";

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                IsErrorMessageVisible = true;
                return;
            }

            try
            {
                var user = await userService.GetOrLoadAsync();

                int.TryParse(Height, out int h);
                double.TryParse(Weight.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double w);
                int.TryParse(CaloriePlan, out int c);
                int.TryParse(ProteinPercent, out int p);
                int.TryParse(FatPercent, out int f);
                int.TryParse(CarbonPercent, out int cb);

                user.ChangeFullName(Name);
                user.ChangeGender(Gender.Value);
                user.ChangeBirthDate(Birthday);
                user.ChangeHeight(h);
                user.ChangeWeight(w);
                user.ChangeCaloriePlan(c);
                user.ChangeMacros(p, f, cb);

                await auth.UpdateProfileAsync(user);

                await nav.GoBackAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка сохранения: {ex.Message}";
                IsErrorMessageVisible = true;
            }
        }
    }
}
