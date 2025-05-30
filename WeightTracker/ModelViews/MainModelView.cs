using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Resources;
using System.Runtime.CompilerServices;
using WeightTracker.Models;
using WeightTracker.ModelViews;

namespace WeightTracker;

public partial class MainModelView : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public StepModelView StepModelView { get; set; }
    public bool StepCounterIsVisible
    {
        get => DayResult.CurrentDay.Date.Date == DateTime.Now.Date;
    }

    public string Date  => DayResult.CurrentDay.Date.ToString("ddd, dd MMMM yyyy", new System.Globalization.CultureInfo("ru-RU"));
    public DateTime MaxDate => DateTime.Now;
    public DateTime SelectedDate => DayResult.CurrentDay.Date;
    public DateTime MinDate => DTWork.FromId(UserModel.MainUser.RegistrationDate);

    public string KcalSpent => DayResult.CurrentDay.KcalSpent.ToString();
    public string KcalRes => DayResult.CurrentDay.KcalRes.ToString();   
    public string KcalEaten => DayResult.CurrentDay.KcalEaten.ToString();

    public string? BrCal => DayResult.CurrentDay.Breakfast["Kcal"].ToString() + " ккал";
    public string? BrPr => "Б: " + DayResult.CurrentDay.Breakfast["Pr"].ToString() + " г";
    public string? BrFat => "Ж: " + DayResult.CurrentDay.Breakfast["Fat"].ToString() + " г";
    public string? BrCb => "У: " + DayResult.CurrentDay.Breakfast["Cb"].ToString() + " Г";

    public string? LnCal => DayResult.CurrentDay.Lunch["Kcal"].ToString() + " ккал";
    public string? LnPr => "Б: " + DayResult.CurrentDay.Lunch["Pr"].ToString() + " г";
    public string? LnFat => "Ж: " + DayResult.CurrentDay.Lunch["Fat"].ToString() + " г";
    public string? LnCb => "У: " + DayResult.CurrentDay.Lunch["Cb"].ToString() + " г";

    public string? DnCal => DayResult.CurrentDay.Dinner["Kcal"].ToString() + " ккал";
    public string? DnPr => "Б: " + DayResult.CurrentDay.Dinner["Pr"].ToString() + " г";
    public string? DnFat => "Ж: " + DayResult.CurrentDay.Dinner["Fat"].ToString() + " г";
    public string? DnCb => "У: " + DayResult.CurrentDay.Dinner["Cb"].ToString() + " г";

    public string? SnCal => DayResult.CurrentDay.Snack["Kcal"].ToString() + " ккал";
    public string? SnPr => "Б: " + DayResult.CurrentDay.Snack["Pr"].ToString() + " г";
    public string? SnFat => "Ж: " + DayResult.CurrentDay.Snack["Fat"].ToString() + " г";
    public string? SnCb => "У: " + DayResult.CurrentDay.Snack["Cb"].ToString() + " г";

    public string PrText => Nutrients.ProteinLabelText;
    public Color PrColor => Nutrients.ProteinLabelTextColor;
    public string FatText => Nutrients.FatLabelText;
    public Color FatColor => Nutrients.FatLabelTextColor;
    public string CbText => Nutrients.CarbonLabelText;
    public Color CbColor => Nutrients.CarbonLabelTextColor;

    public MainModelView() 
    {
        StepModelView = new StepModelView();
    }

    public void RefreshCPFC()
    {
        OnPropertyChanged(nameof(PrText));
        OnPropertyChanged(nameof(FatText));
        OnPropertyChanged(nameof(CbText));
        OnPropertyChanged(nameof(PrColor));
        OnPropertyChanged(nameof(FatColor));
        OnPropertyChanged(nameof(CbColor));
        OnPropertyChanged(nameof(KcalEaten));
        OnPropertyChanged(nameof(KcalRes));
    }
    public void RefreshSport()
    {
        OnPropertyChanged(nameof(KcalSpent));
    }
    public void RefreshBreakfast (bool use)
    {
        OnPropertyChanged(nameof(BrCal));
        OnPropertyChanged(nameof(BrPr));
        OnPropertyChanged(nameof(BrFat));
        OnPropertyChanged(nameof(BrCb));
        if (use) RefreshCPFC();
    }
    public void RefreshLunch(bool use)
    {
        OnPropertyChanged(nameof(LnCal)); 
        OnPropertyChanged(nameof(LnPr));
        OnPropertyChanged(nameof(LnFat));
        OnPropertyChanged(nameof(LnCb));
        if (use) RefreshCPFC();
    }
    public void RefreshDinner(bool use)
    {
        OnPropertyChanged(nameof(DnCal));
        OnPropertyChanged(nameof(DnPr));
        OnPropertyChanged(nameof(DnFat));
        OnPropertyChanged(nameof(DnCb));
        if (use) RefreshCPFC();
    }
    public void RefreshSnack(bool use)
    {
        OnPropertyChanged(nameof(SnCal)); 
        OnPropertyChanged(nameof(SnPr));
        OnPropertyChanged(nameof(SnFat));
        OnPropertyChanged(nameof(SnCb));
        if (use) RefreshCPFC();
    }
    public void RefreshAll()
    {
        RefreshBreakfast(false);
        RefreshLunch(false);
        RefreshDinner(false);
        RefreshSnack(false);
        RefreshCPFC();
    }
}
