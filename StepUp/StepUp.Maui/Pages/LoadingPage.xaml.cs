using StepUp.Application.Interfaces.Services;
using StepUp.Presentation.Constants;

namespace StepUp.Maui.Pages;

public partial class LoadingPage : ContentPage
{
    private readonly ICurrentUserProvider _currentUser;

    public LoadingPage(ICurrentUserProvider currentUser)
    {
        InitializeComponent();
        _currentUser = currentUser;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _currentUser.EnsureLoadedAsync();

        if (_currentUser.IsAuthenticated)
            await Shell.Current.GoToAsync(AppRoutes.Main);
        else
            await Shell.Current.GoToAsync(AppRoutes.Login);
    }
}