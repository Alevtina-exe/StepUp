using StepUp.Application.Interfaces.Services;
using StepUp.Application.Models;
using StepUp.Presentation.Constants;
using StepUp.Presentation.Interfaces;
using StepUp.Presentation.Models;
using System.Text.RegularExpressions;

namespace StepUp.Presentation.ViewModels
{
    public partial class RegistrationViewModel(IClock clock, 
        AuthService authService, INavigationService navService) : ObservableObject
    {
        private readonly AuthService _auth = authService;
        private readonly INavigationService _navService = navService;
        public IReadOnlyList<GenderOption> Genders { get; } = GenderOption.All;


        public DateTime MaxDate { get; } = clock.Today.AddYears(-10);
        public DateTime MinDate { get; } = clock.Today.AddYears(-120);


        [ObservableProperty]
        private string _eyeIcon = "eye.svg";
        [ObservableProperty]
        private string _checkEyeIcon = "eye.svg";


        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
        private bool _nameErrorVisible = false;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
        private bool _emailErrorVisible = false;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
        private bool _passwordErrorVisible = false;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
        private bool _checkPasswordErrorVisible = false;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
        private bool _existingEmailErrorVisible = false;
        [ObservableProperty]
        private bool _errorMessageVisible = false;

        [ObservableProperty] 
        private bool _isPasswordReadable = false;
        [ObservableProperty] 
        private bool _isCheckPasswordReadable = false;


        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
        private string _name = string.Empty;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
        private string _email = string.Empty;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
        private string _password = string.Empty;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
        private string _checkPassword = string.Empty;
        [ObservableProperty]
        private DateTime _birthDate = clock.Today;
        [ObservableProperty]
        private GenderOption _gender = GenderOption.Male;


        partial void OnNameChanged(string value)
        {
            ValidateUsername(value);
            ErrorMessageVisible = false;
        }
        partial void OnEmailChanged(string value)
        {
            ValidateEmail(value);
            ExistingEmailErrorVisible = false;
            ErrorMessageVisible = false;
        } 
        partial void OnPasswordChanged(string value) 
        {
            ValidatePassword(value);
            ValidateCheckPassword(CheckPassword);
            ErrorMessageVisible = false;
        }
        partial void OnCheckPasswordChanged(string value)
        {
            ValidateCheckPassword(value);
            ErrorMessageVisible = false;
        }


        private void ValidateUsername(string value) => NameErrorVisible = !string.IsNullOrWhiteSpace(value) && 
            !Regex.IsMatch(value.Trim(), @"^[a-zA-Zа-яА-ЯёЁ _]{1,20}$");
        private void ValidateEmail(string value) => EmailErrorVisible = !string.IsNullOrWhiteSpace(value) && 
            !Regex.IsMatch(value.Trim(), @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        private void ValidatePassword(string value) => PasswordErrorVisible = !string.IsNullOrWhiteSpace(value) &&
            !PasswordPolicy.IsValid(value);
        private void ValidateCheckPassword(string value) => CheckPasswordErrorVisible = !string.IsNullOrWhiteSpace(value) &&
            value != Password;

        [RelayCommand(CanExecute = nameof(CanContinue), AllowConcurrentExecutions = false)]
        private async Task ContinueAsync()
        {
            try
            {
                ErrorMessageVisible = false;
                if (await _auth.IsUserExistsAsync(Email))
                {
                    ExistingEmailErrorVisible = true;
                    return;
                }
            }
            catch
            {
                ErrorMessageVisible = true;
                return;
            }
            var request = new RegisterRequest
            {
                Name = Name.Trim(),
                Email = Email.Trim(),
                Password = Password.Trim(),
                BirthDate = BirthDate,
                Gender = Gender.Value
            };

            await _navService.GoToPageAsync(AppRoutes.BodyData, 
                new Dictionary<string, object> { { "Request", request } });
        }
        private bool CanContinue() =>
            !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Email) &&
            !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(CheckPassword) &&
            !NameErrorVisible && !EmailErrorVisible && !PasswordErrorVisible && 
            !CheckPasswordErrorVisible && !ExistingEmailErrorVisible;


        [RelayCommand]
        private void TogglePasswordVisibility()
        {
            IsPasswordReadable = !IsPasswordReadable;
            EyeIcon = IsPasswordReadable ? "eye_off.svg" : "eye.svg";
        }

        [RelayCommand]
        private void ToggleCheckPasswordVisibility()
        {
            IsCheckPasswordReadable = !IsCheckPasswordReadable;
            CheckEyeIcon = IsCheckPasswordReadable ? "eye_off.svg" : "eye.svg";
        }
    }
}
