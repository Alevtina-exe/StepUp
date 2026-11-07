namespace StepUp.Presentation.Interfaces
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string title, string message, string buttonText = "OK");
        Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Да", string cancel = "Нет");
        Task<string?> ShowPromptAsync(string title, string message, string placeholder = "", string initialValue = "");
    }
}
