using StepUp.Application.Interfaces.Services;

namespace StepUp.Maui;

public partial class App : Microsoft.Maui.Controls.Application
{
    private readonly IServiceProvider _services;

    public App(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var shell = _services.GetRequiredService<AppShell>();
        _ = LoadSessionAsync(shell);
        return new Window(shell);
    }
    private async Task LoadSessionAsync(AppShell shell)
    {
        var currentUser = _services.GetRequiredService<ICurrentUserProvider>();
        await currentUser.EnsureLoadedAsync();

        if (currentUser.IsAuthenticated)
            await shell.GoToAsync("//MainPage");
        else
            await shell.GoToAsync("//LoginPage");
    }
}
