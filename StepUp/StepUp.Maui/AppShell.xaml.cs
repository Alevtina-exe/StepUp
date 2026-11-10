using StepUp.Maui.Views;

namespace StepUp.Maui;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
        Routing.RegisterRoute(nameof(BodyDataPage), typeof(BodyDataPage));

        Routing.RegisterRoute(nameof(AddProductPage), typeof(AddProductPage));
        Routing.RegisterRoute(nameof(ProductPage), typeof(ProductPage));
        Routing.RegisterRoute(nameof(ViewMealPage), typeof(ViewMealPage));
        Routing.RegisterRoute(nameof(SportListPage), typeof(SportListPage));

        Routing.RegisterRoute(nameof(GraphPage), typeof(GraphPage));
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        Routing.RegisterRoute(nameof(EditAccountPage), typeof(EditAccountPage));

        Routing.RegisterRoute(nameof(BarcodeScannerPage), typeof(BarcodeScannerPage));
    }
}
