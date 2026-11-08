using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StepUp.Application.Exceptions;
using StepUp.Application.Services;
using StepUp.Presentation.Constants;
using StepUp.Presentation.Interfaces;
using System.Text.RegularExpressions;

namespace StepUp.Presentation.ViewModels
{
    public partial class LoginViewModel(AuthService auth, INavigationService nav) : ObservableObject
    {

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string _email = string.Empty;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string _password = string.Empty;

        [ObservableProperty]
        private bool _isPasswordReadable = false;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private bool _emailErrorVisible = false;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private bool _notExistingEmailErrorVisible = false;
        [ObservableProperty]
        private bool _errorMessageVisible = false;

        [ObservableProperty]
        private string _errorMessage = string.Empty;
        [ObservableProperty]
        private string _eyeIcon = "eye.png";


        partial void OnEmailChanged(string value)
        {
            NotExistingEmailErrorVisible = false;
            ErrorMessageVisible = false;
            ValidateEmail(value);
        }

        partial void OnPasswordChanged(string value)
        {
            ErrorMessageVisible = false;
        }


        [RelayCommand(CanExecute = nameof(CanLogin), AllowConcurrentExecutions = false)]
        private async Task Login()
        {
            ErrorMessageVisible = false;
            try
            {
                await auth.LoginAsync(Email, Password);
                await nav.GoToPageAsync(AppRoutes.Main);
            }
            catch (UserNotFoundException) 
            {
                NotExistingEmailErrorVisible = true;
            }
            catch (InvalidPasswordException ex)
            {
                ErrorMessage = ex.Message;
                ErrorMessageVisible = true;
            }
            catch
            {
                ErrorMessage = "Что-то пошло не так. Повторите попытку позже";
                ErrorMessageVisible = true;
            }
        }

        private void ValidateEmail(string value) => EmailErrorVisible = !string.IsNullOrWhiteSpace(value) &&
            !Regex.IsMatch(value, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Email) && !EmailErrorVisible &&
                !string.IsNullOrWhiteSpace(Password) && !NotExistingEmailErrorVisible;
        }

        [RelayCommand]
        private void TogglePasswordVisibility()
        {
            IsPasswordReadable = !IsPasswordReadable;
            EyeIcon = IsPasswordReadable ? "eye_off.png" : "eye.png";
        }

        [RelayCommand]
        private async Task GoToRegisterPage()
        {
            await nav.GoToPageAsync(AppRoutes.Registration);
        }
    }
}
