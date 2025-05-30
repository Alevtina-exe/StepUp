using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.Text.RegularExpressions;
using System.Windows.Input;
using WeightTracker.Models;
using WeightTracker.ModelViews;

namespace WeightTracker.Views;

public partial class SettingsPage : ContentPage
{
    public ICommand ChangePassCommand { get; }
    public ICommand ChangeAllCommand { get; }
    public ICommand DeleteAccCommand { get; }
    public ICommand LeaveAccCommand { get; }
    private DatabaseModelView database;

    public SettingsPage()
	{
		InitializeComponent();
        database = new DatabaseModelView(new Services.FirestoreService());
        ChangePassCommand = new Command(ChangePassword);
		ChangeAllCommand = new Command(ChangeAllData);
		DeleteAccCommand = new Command(DeleteAccount);
        LeaveAccCommand = new Command(LeaveAccount);
		BindingContext = this;
	}
	
	protected override void OnAppearing()
	{
		base.OnAppearing();
		NameText.ValueText = UserModel.MainUser.FullName;
        GenderText.ValueText = UserModel.MainUser.Gender;
        AgeText.ValueText = UserModel.MainUser.Age.ToString();
        HeightText.ValueText = UserModel.MainUser.Height.ToString();
        WeightText.ValueText = UserModel.MainUser.Weight.ToString();
        UsernameText.ValueText = UserModel.MainUser.Username;
        KcaltText.ValueText = UserModel.MainUser.CaloriePlan.ToString();

	}
    private async void ChangePassword()
    {
        string input = "";
        bool f = true;
        while (input.Trim() != UserModel.MainUser.PasswordHash)
        {
            if (f)
            {
                f = !f;
            }
            else
            {
                await DisplayAlert("Ошибка", "Неправильный пароль! Попробуйте снова.", "OK");
            }
            input = await DisplayPromptAsync("Введите старый пароль", "", "OK", "Отмена", "Введите пароль...");
            if (input == null) return;
        }
		f = true;
        bool validLength = false, containsLatinLetter = false, containsDigit = false, noCyrillic = false;
        while (!(validLength && containsLatinLetter && containsDigit && noCyrillic))
        {
            if (f)
            {
                f = !f;
                input = await DisplayPromptAsync("Введите новый пароль", "", "OK", "Отмена", "Введите пароль...");
            }
            else
            {
                input = await DisplayPromptAsync("Введите новый пароль", 
                    "Пароль должен содержать минимум 8 символов, хотя бы одну букву и одну цифру, и не содержать кириллицу", 
                    "OK", "Отмена", "Введите пароль...");
            }
            if (input == null) return;
            validLength = input.Trim().Length >= 8;
            containsLatinLetter = Regex.IsMatch(input, @"[a-zA-Z]");
            containsDigit = Regex.IsMatch(input, @"\d");
            noCyrillic = !Regex.IsMatch(input, @"[а-яА-Я]");
        }
		await database.UpdateField(UserModel.MainUser.Username, new string("PasswordHash"), input.Trim());
    }
    private async void LeaveAccount()
    {
        bool answer = await DisplayAlert("Подтверждение", "Выйти из аккаунта?", "Да", "Нет");
        if (answer)
        {
            UserModel.MainUser = null;
            await SecureStorage.SetAsync("username", "");
            await Navigation.PushAsync(new LoginPage());
        }
    }
    private async void DeleteAccount()
	{
        bool answer = await DisplayAlert("Подтверждение", "Вы уверены? При удалении пользователя все данные будут утеряны", "Да", "Нет");
        if (answer)
        {
            await database.DeleteUser(UserModel.MainUser.Username);
            UserModel.MainUser = null;
            await Navigation.PushAsync(new LoginPage());
        }
    }

    private async void ChangeAllData()
	{
        await Navigation.PushAsync(new ChangeDataPage());
	}
}