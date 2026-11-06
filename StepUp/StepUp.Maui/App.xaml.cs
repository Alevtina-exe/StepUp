using StepUp.Maui.Services;
using StepUp.Maui.Views;

namespace StepUp.Maui;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
        MainPage = new NavigationPage(new LoginPage());
		
    }
}
