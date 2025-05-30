using CommunityToolkit.Maui.Views;
using WeightTracker.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;


namespace WeightTracker.Views;

public partial class DetailedView : CommunityToolkit.Maui.Views.Popup
{


	public DetailedView()
	{
		InitializeComponent();

	}
	
	private async void BackBtn_Clicked(object sender, EventArgs e)
	{
		await CloseAsync(false);
	}
	
}