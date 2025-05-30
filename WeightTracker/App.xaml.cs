using WeightTracker.Services;
using WeightTracker.Views;

namespace WeightTracker;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
        MainPage = new NavigationPage(new LoginPage());
		
    }
}
