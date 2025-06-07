using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Handlers;
using System.Threading.Tasks;
using WeightTracker.Models;
using WeightTracker.ModelViews;
using WeightTracker.Services;

namespace WeightTracker.Views;

public partial class MainPage : ContentPage
{
	private DatabaseModelView database;
    public MainModelView MainModelView { get; set; }
    public MainPage()
    {
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        MainModelView = new MainModelView();
        var shadowEffect = new Shadow
        {
            Brush = new SolidColorBrush(Colors.Black),
            Offset = new Point(5, 5),
            Radius = 10,
            Opacity = 0.5f
        };
        MyGrid.Shadow = shadowEffect;
        BindingContext = MainModelView;
		database = new DatabaseModelView(new FirestoreService());
        
    }
	protected async override void OnAppearing() 
	{
		base.OnAppearing();
        MainModelView.RefreshAll();
        double progress = 1 - (double)DayResult.CurrentDay.KcalRes / UserModel.MainUser.CaloriePlan;
        ColorChanger colorChanger = new ColorChanger(progressBar, KcalRes, KcalLeft, progress);
        await colorChanger.AnimateColor();
        
    }
    private async void GraphBtn_Clicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new GraphPage());
	}
	private async void SettingsBtn_Clicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new SettingsPage());
	}

    private async void KcalSpent_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ViewSportsPage());
    }
    private async void NewBtn_Clicked(object sender, EventArgs e)
    {
        var popup = new MenuBar();
        await this.ShowPopupAsync(popup);
        switch (MenuBar.type)
        {
            case 1:
                await this.ShowPopupAsync(new AddSport(UserModel.MainUser.Weight));
                break;
            case 2:
                await Navigation.PushAsync(new AddProductPage(Meal.Breakfast));
                break;
            case 3:
                await Navigation.PushAsync(new AddProductPage(Meal.Lunch));
                break;
            case 4:
                await Navigation.PushAsync(new AddProductPage(Meal.Dinner));
                break;
            case 5:
                await Navigation.PushAsync(new AddProductPage(Meal.Snack));
                break;
            case 6:
                await this.ShowPopupAsync(new AddSport());
                MainModelView.RefreshSport();
                MainModelView.RefreshCPFC();
                break;
            default:
                break;
        }
        OnAppearing();
    }

    private async void datePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        await database.StartWorkWithDay(datePicker.Date);
        
        datePicker.IsVisible = false;
        MainModelView.RefreshAll();
        double progress = 1 - (double)DayResult.CurrentDay.KcalRes / UserModel.MainUser.CaloriePlan;
        ColorChanger colorChanger = new ColorChanger(progressBar, KcalRes, KcalLeft, progress);
        await colorChanger.AnimateColor();
    }

    private void DateButton_Clicked(object sender, EventArgs e)
    {
        var handler = datePicker.Handler as IDatePickerHandler;
        handler?.PlatformView?.PerformClick();
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        DateButton_Clicked(sender, e);
    }

    private async void Breakfast_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ViewMealPage(Meal.Breakfast));
    }
    private async void Lunch_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ViewMealPage(Meal.Lunch));
    }
    private async void Dinner_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ViewMealPage(Meal.Dinner));
    }
    private async void Snack_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ViewMealPage(Meal.Snack));
    }
}