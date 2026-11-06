namespace StepUp.Presentation.Interfaces
{
    public interface INavigationService
    {
        Task GoBackAsync();
        Task NavigateToAsync<TViewModel>() where TViewModel : class;
        Task NavigateToAsync<TViewModel, TParameter>(TParameter parameter) where TViewModel : class;
    }
}