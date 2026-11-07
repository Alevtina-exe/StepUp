using StepUp.Presentation.Interfaces;

namespace StepUp.Maui.Services;

public class MauiNavigationService : INavigationService
{
    public Task GoBackAsync()
    {
        return Shell.Current.GoToAsync("..");
    }

    public Task GoToPageAsync(string route)
    {
        return Shell.Current.GoToAsync(route);
    }

    public Task GoToPageAsync(string route, IDictionary<string, object> parameters)
    {
        return Shell.Current.GoToAsync(route, parameters);
    }
}