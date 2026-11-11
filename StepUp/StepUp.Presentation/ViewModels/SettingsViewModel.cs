using StepUp.Application.Interfaces.Services;
using StepUp.Domain.Extensions;
using StepUp.Presentation.Constants;
using StepUp.Presentation.Interfaces;

namespace StepUp.Presentation.ViewModels 
{
    public partial class SettingsViewModel(AuthService auth, 
        ICurrentUserService userService, ICurrentUserProvider userProvider,
        IDialogService dialog, INavigationService nav, IPopupFactory popups) : ObservableObject
    {
        [ObservableProperty]
        private string _name = string.Empty;
        [ObservableProperty]
        private string _gender = string.Empty;
        [ObservableProperty]
        private string _email = string.Empty;
        [ObservableProperty]
        private int _age = 0;
        [ObservableProperty]
        private int _height = 0;
        [ObservableProperty]
        private double _weight = 0;
        [ObservableProperty]
        private int _caloriePlan = 0;
        
        public async Task InitializeAsync()
        {
            if(!userProvider.IsAuthenticated)
            {
                await dialog.ShowAlertAsync("Ошибка", "Не удалось получить данные пользователя", "Выйти");
                auth.Logout();
                await nav.GoToPageAsync(AppRoutes.Login);
                return;
            }
            var user = await userService.GetOrLoadAsync();
            Name = user.FullName;
            Email = user.Email;
            Gender = user.Gender.GetDescription();
            Age = user.Age;
            Weight = user.Weight;
            Height = user.Height;
            CaloriePlan = user.CaloriePlan;
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            var ans = await dialog.ShowConfirmationAsync("Подтверждение", "Выйти из аккаунта?");
            if(ans)
            {
                auth.Logout();
                await nav.GoToPageAsync(AppRoutes.Login);
            }
        }
        [RelayCommand]
        private async Task ChangePasswordAsync()
        {
            var newPassword = await dialog.ShowPopupAsync(popups.CreateChangePasswordPopup());
            if (newPassword is null) return;
            
            try
            {
                await auth.ChangePasswordAsync(newPassword);
                await dialog.ShowAlertAsync("Успех", "Пароль успешно изменен", "OK");
            }
            catch (Exception ex)
            {
                await dialog.ShowAlertAsync("Ошибка", ex.Message, "OK");
            }
        }
        [RelayCommand]
        private async Task ChangeAllAsync()
        {
            await nav.GoToPageAsync(AppRoutes.EditAccount);
        }
        [RelayCommand]
        private async Task DeleteAccountAsync()
        {
            var ans = await dialog.ShowConfirmationAsync("Вы уверены?", "При удалении пользователя все данные будут утеряны");
            if (ans)
            {
                try
                {
                    await auth.DeleteProfileAsync();
                }
                catch (Exception ex)
                {
                    await dialog.ShowAlertAsync("Ошибка", ex.Message, "Выйти");
                    auth.Logout();
                }
                finally
                {
                    await nav.GoToPageAsync(AppRoutes.Login);
                }
            }
        }

    }
}
