using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WeightTracker.Models;
using WeightTracker.ModelViews;
using static WeightTracker.Views.ViewSportsPage;
namespace WeightTracker.Views;


public partial class AddSport : Popup
{
    private DatabaseModelView _database;
    bool _weight;
    bool _edited;
    private string _sportName;
    private int _kcalAmount;
    public AddSport() 
	{
		InitializeComponent();
        delButton.IsVisible = false;
        _database = new DatabaseModelView(new Services.FirestoreService());
        _weight = false;
        _edited = false;
	}
    public AddSport(string sportName, int kcalSpent)
    {
        InitializeComponent();
        _database = new DatabaseModelView(new Services.FirestoreService());
        _sportName = sportName;
        _kcalAmount = kcalSpent;

        NameEntry.Text = sportName;
        KcalEntry.Text = kcalSpent.ToString();
        text.Text = "Изменить физ. нагрузку";
        _weight = false;
        _edited = true;
    }
    public AddSport(double weight)
    {
        InitializeComponent();
        delButton.IsVisible=false;
        _database = new DatabaseModelView(new Services.FirestoreService());
        KcalEntry.IsVisible = false;
        NameEntry.Text = weight.ToString();
        text.Text = "Изменить массу тела";
        _weight = true;
        _edited = false;
    }
    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (_weight)
        {
            double d = 0;
            if (double.TryParse(NameEntry.Text,out d) && d >= 30 && d <= 300 &&DayResult.CurrentDay != null) {
                DayResult.CurrentDay.Weight = d;
                UserModel.MainUser.Weight = d;
                await _database.AddDayWeight(d);
            }
            else
            {
                ErrorLabel.IsVisible = true;
                ErrorLabel.Text = "Допустимые значения веса: 30-300 кг";
            }
        }
        else
        {
            int k;
            if (int.TryParse(KcalEntry.Text, out k) && DayResult.CurrentDay != null)
            {
                if(_edited)
                {
                    DayResult.CurrentDay.Sports.Remove(_sportName);
                    DayResult.CurrentDay.KcalRes -= _kcalAmount;
                    DayResult.CurrentDay.KcalSpent -= _kcalAmount;
                    await _database.AddSport(NameEntry.Text, k, _sportName, _kcalAmount);
                }
                else
                {
                    await _database.AddSport(NameEntry.Text, k);
                }
                DayResult.CurrentDay.Sports.Add(NameEntry.Text, k);
                DayResult.CurrentDay.KcalSpent += k;
                DayResult.CurrentDay.KcalRes += k;  
                await _database.RefreshKcals();
            }
            else
            {
                ErrorLabel.IsVisible = true;
                ErrorLabel.Text = "Неверный формат ввода";
            }
        }
        Close(false);
    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {
        Close(false);
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        ErrorLabel.IsVisible = false;
    }

    private async void delButton_Clicked(object sender, EventArgs e)
    {
        DayResult.CurrentDay.KcalRes -= _kcalAmount;
        DayResult.CurrentDay.KcalSpent -= _kcalAmount;
        DayResult.CurrentDay.Sports.Remove(_sportName);
        await _database.DeleteSport(_sportName);
        await _database.RefreshKcals();
        Close(true);
    }
}