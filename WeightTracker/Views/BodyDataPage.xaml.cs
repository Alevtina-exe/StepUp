using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using System;
using System.Collections.ObjectModel;
using WeightTracker.Models;
using WeightTracker.ModelViews;
using WeightTracker.Services;

namespace WeightTracker.Views;

public partial class BodyDataPage : ContentPage
{
    UserModel _user;
    Sport _sport;
    private bool _whChoosed;
    private readonly DatabaseModelView database;
    public BodyDataPage(UserModel user)
    {
        InitializeComponent();
        database = new DatabaseModelView(new FirestoreService());
        NavigationPage.SetHasNavigationBar(this, false);
        _user = user;
        _whChoosed = false;
        SubmitButton.IsEnabled = false;
    }

    private double CalculateIMT(double weight, int height)
    {
        double heightM = height / 100.0;
        return weight / (heightM * heightM);
    }

    private string GetIMTCategory(double imt)
    {
        return imt < 18.9 ? "Дефицит веса" :
               imt < 24.9 ? "Нормальный вес" :
               imt < 29.9 ? "Избыточный вес" :
               imt < 34.9 ? "Ожирение 1 степени" :
               imt < 39.9 ? "Ожирение 2 степени" :
               "Ожирение 3 степени";
    }
    private Color GetIMTColor(double imt)
    {
        return imt < 18.9 ? Colors.RoyalBlue :
               imt < 24.9 ? Colors.Green :
               imt < 29.9 ? Colors.Yellow :
               imt < 34.9 ? Colors.Orange :
               imt < 39.9 ? Colors.Red : Colors.DarkRed;
    }
    private int GetIMTColumn(double imt)
    {
        return imt < 18.9 ? 0 :
               imt < 24.9 ? 1 :
               imt < 29.9 ? 2 :
               imt < 34.9 ? 3 :
               imt < 39.9 ? 4 : 5;                  
    }

    private void UpdateIMTIndicator(double imt)
    {
        int columnIndex = GetIMTColumn(imt);
        double relativeOffset;
        IMTIndicator.SetValue(Grid.ColumnProperty, 0);
        if (imt <= 8.9) relativeOffset = 0;
        else if (imt >= 43.9) relativeOffset = MyGrid.Width;
        else relativeOffset = (imt - 8.9) / (43.9 - 8.9) * MyGrid.Width; 
        IMTIndicator.Margin = new Thickness(relativeOffset, 0, 0, 0);
        IMTIndicator.IsVisible = true;
    }
    private void OnWeightChanged(object sender, TextChangedEventArgs e)
    {
        ValidateData();
    }

    private void OnHeightChanged(object sender, TextChangedEventArgs e)
    {
        ValidateData();
    }
    private void ValidateData()
    {
        ErrorLabel.IsVisible = false;
        double weight = double.TryParse(WeightEntry.Text, out double w) ? w : 0;
        int height = int.TryParse(HeightEntry.Text, out int h) ? h : 0;
        bool validH = height >= 100 && height <= 250;
        bool validW = weight >= 30 && weight <= 300;
        bool valid = validH && validW;
        SubmitButton.IsEnabled = valid;
        WarningLabel.IsVisible = !valid && !string.IsNullOrEmpty(HeightEntry.Text) && !string.IsNullOrEmpty(WeightEntry.Text)
            || (string.IsNullOrEmpty(HeightEntry.Text) && !string.IsNullOrEmpty(WeightEntry.Text)  && !validW) 
            || (!validH && string.IsNullOrEmpty(WeightEntry.Text) && !string.IsNullOrEmpty(HeightEntry.Text));
        WarningLabel.Text = "Недопустимые значения! Вес: 30-300 кг, Рост: 100-250 см.";
        IMTIndicator.IsVisible = valid;
        if (valid)
        {
            double imt = CalculateIMT(weight, height);
            IMTLabel.Text = $"Ваш ИМТ: {imt:F1}";
            IMTCategoryLabel.Text = $"{GetIMTCategory(imt)}";
            IMTCategoryLabel.TextColor = GetIMTColor(imt);
            UpdateIMTIndicator(imt);
        }
        else
        {
            IMTLabel.Text = "Ваш ИМТ: ";
            IMTCategoryLabel.Text = "";
            IMTCategoryLabel.TextColor = Colors.Black;
            IMTIndicator.IsVisible = false;
        }
        SubmitButton.IsEnabled = SubmitButton.IsEnabled && SportPicker.SelectedItem != null;
    }
    private void OnSportClicked(object sender, EventArgs e)
    {
        string res = SportPicker.SelectedItem.ToString();
        switch (res)
        {
            case "Нет физ. нагрузки":
                _sport = Sport.No;
                break;
            case "Лёгкая физ. нагрузка":
                _sport = Sport.Low;
                break;
            case "Средняя физ. нагрузка":
                _sport = Sport.Mid;
                break;
            case "Ежедневная физ. нагрузка":
                _sport = Sport.High;
                break;
            case "Профессиональный спорт":
                _sport = Sport.Extreme;
                break;
            default:
                _sport = Sport.No;
                break;
        }
        ValidateData();
    }
    private int findCalorieAmount(Sport _sport)
    {
        double ko, res;
        switch (_sport)
        {
            case Sport.No:
                ko = 1.2;
                break;
            case Sport.Low:
                ko = 1.375;
                break;
            case Sport.Mid:
                ko = 1.55;
                break;
            case Sport.High:
                ko = 1.7;
                break;
            case Sport.Extreme:
                ko = 1.9;
                break;
            default:
                ko = 1;
                break;
        }
        if (_user.Gender == "Мужской")
        {
            res = 66.5 + (13.75 * _user.Weight) + (5.003 * _user.Height);
            res -= (6.775 * _user.Age);
        }
        else
        {
            res = 655.1 + (9.563 * _user.Weight) + (1.85 * _user.Height);
            res -= (4.676 * _user.Age);
        }
        res *= ko;
        return (int)res;
    }
    private void OnPFCChanged(object sender, TextChangedEventArgs e)
    {
        ErrorLabel.IsVisible = false;
        int protein = int.TryParse(ProteinEntry.Text, out int p) ? p : 0;
        int fat = int.TryParse(FatEntry.Text, out int f) ? f : 0;
        int carbon = int.TryParse(CarbonEntry.Text, out int c) ? c : 0;
        if(carbon + fat + protein != 100 || carbon == 0 || fat == 0 || protein == 0)
        {
            ProteinEntry.TextColor = Colors.Red;
            CarbonEntry.TextColor = Colors.Red;
            FatEntry.TextColor = Colors.Red;
            SubmitButton.IsEnabled = false;
        }
        else
        {
            ProteinEntry.TextColor = Colors.Green;
            CarbonEntry.TextColor = Colors.Green;
            FatEntry.TextColor = Colors.Green;
            SubmitButton.IsEnabled = true;
        }
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        if (!_whChoosed)
        {
            _user.Weight = double.Parse(WeightEntry.Text);
            _user.Height = int.Parse(HeightEntry.Text);
            HeightEntry.IsEnabled = false;
            WeightEntry.IsEnabled = false;
            SportPicker.IsEnabled = false;
            ProteinEntry.Text = "30";
            FatEntry.Text = "30";
            CarbonEntry.Text = "40";
            HideableStack.IsVisible = true;
            KcalLabel.Text = findCalorieAmount(_sport).ToString();
            _whChoosed = true;
        }
        else
        {
            _user.ProteinPercent = int.Parse(ProteinEntry.Text);
            _user.FatPercent = int.Parse(FatEntry.Text);
            _user.CarbonPercent = int.Parse(CarbonEntry.Text);
            _user.CaloriePlan = int.Parse(KcalLabel.Text);
            try { 
                await database.SaveUserData(_user);
                UserModel.MainUser = _user;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
            {
                ErrorLabel.IsVisible = true;
                return;
            }
            Navigation.InsertPageBefore(new MainPage(), this);
            await Navigation.PopAsync();
        }
    }
    private async void OnKcalTapped(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        bool tr = false;
        int newKcal = 0;
        while (!tr)
        {
            string input = await DisplayPromptAsync("Введите желаемый калораж", "Значение не должжно быть меньше 1300 ккал", "OK", "Отмена", $"{findCalorieAmount(_sport)}");
            if (String.IsNullOrEmpty(input)) return; 
            tr = int.TryParse(input, out newKcal);
            if (!tr || newKcal < 1300)
            {
                await DisplayAlert("Ошибка", "Калораж введён неверно! Попробуйте снова.", "OK");
            }
        }
        KcalLabel.Text = newKcal.ToString();
    }
    private void OnBackClicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        if (!_whChoosed)
        {
            Navigation.RemovePage(this);
        }
        else
        {
            HeightEntry.IsEnabled = true;
            WeightEntry.IsEnabled = true;
            SportPicker.IsEnabled = true;
            HideableStack.IsVisible = false;
            _whChoosed = false;
            SubmitButton.IsEnabled = true;
        }
    }
}
