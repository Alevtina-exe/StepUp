using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using WeightTracker.Models;
using WeightTracker.ModelViews;
using WeightTracker.Services;

namespace WeightTracker.Views;
public partial class RegistrationPage : ContentPage
{
    bool _errorOccured = false;
    private readonly DatabaseModelView mv;
    public RegistrationPage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        ContinueButton.IsEnabled = false;
        var firestoreService = new FirestoreService();
        mv = new DatabaseModelView(firestoreService);
        BindingContext = mv;
        BirthDatePicker.MaximumDate = DateTime.Now;
        BirthDatePicker.MinimumDate = new DateTime(1900, 1, 1);
    }

    private void OnNameChanged(object sender, TextChangedEventArgs e)
    {
        NameErrorLabel.IsVisible = !Regex.IsMatch(NameEntry.Text.Trim(), @"^[а-яА-Яa-zA-Z]+$");
        ValidateForm();
    }
    private void OnTogglePassword(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        ShowPasswordButton.Text = PasswordEntry.IsPassword ? "👁" : "🔒";
    }


    private void OnPasswordChanged(object sender, TextChangedEventArgs e)
    {
        bool validLength = PasswordEntry.Text.Trim().Length >= 8;
        bool containsLatinLetter = Regex.IsMatch(PasswordEntry.Text.Trim(), @"[a-zA-Z]");
        bool containsDigit = Regex.IsMatch(PasswordEntry.Text.Trim(), @"\d");
        bool noCyrillic = !Regex.IsMatch(PasswordEntry.Text.Trim(), @"[а-яА-Я]");

        PasswordErrorLabel.IsVisible = !(validLength && containsLatinLetter && containsDigit && noCyrillic);
        ValidateForm();
    }

    public async Task<bool>IsLoginTaken(string username)
    {
        bool res = true;
        try { 
            res = await mv.IsInUserDatabase(username);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            ErrorLabel.IsVisible = true;
            _errorOccured = true;
        }
        return res;
    }

    private async void OnLoginChanged(object sender, TextChangedEventArgs e)
    {
        bool validLogin = Regex.IsMatch(LoginEntry.Text.Trim(), @"^[a-zA-Z._]+$");

        if (!validLogin)
        {
            LoginErrorLabel.Text = "Логин содержит недопустимые символы!";
            LoginErrorLabel.IsVisible = true;
            ValidateForm();
            return;
        }
        else
        {
            LoginErrorLabel.IsVisible = false;
            ValidateForm();

        }
    }
    private void OnAgeSelected(object sender, EventArgs e) => ValidateForm();
    private void OnGenderSelected(object sender, EventArgs e) => ValidateForm();
    private bool DateComp(DateTime one, DateTime two)
    {
        if (one.Year > two.Year) return true;
        if(one.Year == two.Year)
        {
            if (one.Month > two.Month) return true;
            if (one.Month == two.Month)
            {
                if (one.Day > two.Day) return true;
            }
        }
        return false;
    }
    private void ValidateForm()
    {
        ContinueButton.IsEnabled = !NameErrorLabel.IsVisible && !LoginErrorLabel.IsVisible &&
                                   !string.IsNullOrWhiteSpace(NameEntry.Text) &&
                                   !string.IsNullOrWhiteSpace(LoginEntry.Text) &&
                                   DateComp(DateTime.Now, BirthDatePicker.Date) &&
                                   GenderPicker.SelectedItem != null;
        ErrorLabel.IsVisible = false;
    }
    private void OnDateSelected(object sender, DateChangedEventArgs e)
    {
        ValidateForm();
    }
    private async void OnContinueClicked(object sender, EventArgs e)
    {
        if (await IsLoginTaken(LoginEntry.Text) && !_errorOccured)
        {
            LoginErrorLabel.Text = "Этот логин уже занят! Попробуйте другой.";
            LoginErrorLabel.IsVisible = true;
        }
        else if(!await IsLoginTaken(LoginEntry.Text) && !_errorOccured)
        {
            int age = DateTime.Now.Year - BirthDatePicker.Date.Year;
            if (DateTime.Now < BirthDatePicker.Date.AddYears(age)) age--;
            var user = new UserModel
            {
                FullName = NameEntry.Text,
                Username = LoginEntry.Text,
                PasswordHash = PasswordEntry.Text,
                Age = age,
                Day = BirthDatePicker.Date.Day,
                Month = BirthDatePicker.Date.Month,
                Year = BirthDatePicker.Date.Year,
                Gender = GenderPicker.SelectedItem.ToString()
            };
            UserModel.MainUser = user;
            await Navigation.PushAsync(new BodyDataPage(user));
        }
        else
        {
            _errorOccured = false;
        }
    }
}
