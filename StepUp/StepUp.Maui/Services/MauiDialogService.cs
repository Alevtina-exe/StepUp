using StepUp.Presentation.Interfaces;

namespace StepUp.Maui.Services;

public class MauiDialogService : IDialogService
{
    public Task ShowAlertAsync(string title, string message, string buttonText = "OK")
    {
        return MainThread.InvokeOnMainThreadAsync(() =>
            Shell.Current.DisplayAlertAsync(title, message, buttonText));
    }

    public Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Да", string cancel = "Нет")
    {
        return MainThread.InvokeOnMainThreadAsync(() =>
            Shell.Current.DisplayAlertAsync(title, message, accept, cancel));
    }

    public Task<string?> ShowPromptAsync(string title, string message, string placeholder = "", string initialValue = "")
    {
        return MainThread.InvokeOnMainThreadAsync(() =>
            Shell.Current.DisplayPromptAsync(
                title: title,
                message: message,
                accept: "OK",
                cancel: "Отмена",
                placeholder: placeholder,
                initialValue: initialValue));
    }
}