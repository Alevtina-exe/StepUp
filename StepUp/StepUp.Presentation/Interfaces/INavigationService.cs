namespace StepUp.Presentation.Interfaces;

public interface INavigationService
{
    Task GoBackAsync();

    Task GoToPageAsync(string route);

    Task GoToPageAsync(string route, IDictionary<string, object> parameters);
}