using CommunityToolkit.Maui.Views;
using StepUp.Application.Services;
using StepUp.Presentation.Interfaces;

namespace StepUp.Maui.Popups;

public partial class ChangePasswordPopup : Popup<string?>, IPopup<string?>
{
	public ChangePasswordPopup()
	{
		InitializeComponent();
		BindingContext = this;
	}

    public void OnEntryChanged(object sender, TextChangedEventArgs e)
	{
		ErrorLabel.IsVisible = !PasswordPolicy.IsValid(e.NewTextValue);
		SaveButton.IsEnabled = !ErrorLabel.IsVisible;
    }
	public async void OnSaveClicked(object sender, EventArgs e )
	{
		await CloseAsync(Password.Text);
	}
	public async void OnCancelClicked(object sender, EventArgs e)
	{
		await CloseAsync(null);
	}
}