using StepUp.Maui.Pages;

namespace StepUp.Maui;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
        Routing.RegisterRoute(nameof(BodyDataPage), typeof(BodyDataPage));

        Routing.RegisterRoute(nameof(SearchFoodItemPage), typeof(SearchFoodItemPage));
        Routing.RegisterRoute(nameof(ProductPage), typeof(ProductPage));
        Routing.RegisterRoute(nameof(MealListPage), typeof(MealListPage));
        Routing.RegisterRoute(nameof(SportListPage), typeof(SportListPage));

        Routing.RegisterRoute(nameof(GraphPage), typeof(GraphPage));
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        Routing.RegisterRoute(nameof(EditAccountPage), typeof(EditAccountPage));

        Routing.RegisterRoute(nameof(BarcodeScannerPage), typeof(BarcodeScannerPage));
    }
}
