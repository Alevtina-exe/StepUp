using StepUp.Maui.Pages;

namespace StepUp.Maui;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(AddFoodItemPage), typeof(AddFoodItemPage));
        Routing.RegisterRoute(nameof(BarcodeScannerPage), typeof(BarcodeScannerPage));
        Routing.RegisterRoute(nameof(BodyDataPage), typeof(BodyDataPage));
        Routing.RegisterRoute(nameof(EditAccountPage), typeof(EditAccountPage));
        Routing.RegisterRoute(nameof(EditFoodItemPage), typeof(EditFoodItemPage));
        Routing.RegisterRoute(nameof(GraphPage), typeof(GraphPage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        Routing.RegisterRoute(nameof(MealListPage), typeof(MealListPage));
        Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
        Routing.RegisterRoute(nameof(SearchProductPage), typeof(SearchProductPage));
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        Routing.RegisterRoute(nameof(SportListPage), typeof(SportListPage));
    }
}
