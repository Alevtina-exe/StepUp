using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using Microsoft.Maui.Storage;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WeightTracker.Models;
using WeightTracker.ModelViews;
using WeightTracker.Services;

namespace WeightTracker.Views;

public partial class LoginPage : ContentPage
{
    private readonly DatabaseModelView mv;
    public LoginPage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        var firestoreService = new FirestoreService();
        mv = new DatabaseModelView(firestoreService);
        BindingContext = mv;
    }
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        bool userVal = false;
        try { 
            userVal = await mv.IsInUserDatabase(UsernameEntry.Text.Trim());
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            ErrorLabel.Text = "Что-то пошло не так. Проверьте интернет-соединение";
            ErrorLabel.IsVisible = true;
            return;
        }

        if (userVal)
        {
            UserModel? user = null;
            try { 
                user = await mv.ReturnUser(UsernameEntry.Text.Trim());
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
            {
                ErrorLabel.Text = "Что-то пошло не так. Проверьте интернет-соединение";
                ErrorLabel.IsVisible = true;
                return;
            }
            if (VerifyPassword(PasswordEntry.Text, user.PasswordHash))
            {
                UserModel.MainUser = user;
                int age = DateTime.Now.Year - user.Year;
                if (DateTime.Now.Month * 30 + DateTime.Now.Day < user.Month * 30 + user.Day) age--;
                UserModel.MainUser.Age = age;
                await mv.StartWorkWithDay(DateTime.Now);
                await SecureStorage.SetAsync("username", UsernameEntry.Text.Trim());
                await Navigation.PushAsync(new MainPage());
            }
            else
            {
                ErrorLabel.Text = "Неверный пароль.";
                ErrorLabel.IsVisible = true;
            }
        }
        else
        {
            ErrorLabel.Text = "Данного пользователя нет в системе.";
            ErrorLabel.IsVisible = true;
        }
    }

    private bool VerifyPassword(string enteredPassword, string storedPasswordHash)
    {
        return enteredPassword == storedPasswordHash;
    }
    private void OnTogglePassword(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        ShowPasswordButton.Text = PasswordEntry.IsPassword ? "👁" : "❌"; // Меняем иконку
    }

    private async void OnLabelTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new RegistrationPage());
    }
}
