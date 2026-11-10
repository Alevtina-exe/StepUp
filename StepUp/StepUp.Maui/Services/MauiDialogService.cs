using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
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

    public async Task<TResult?> ShowPopupAsync<TResult>(IPopup<TResult> popup, CancellationToken ct = default)
    {
        if (popup is not Popup<TResult> mauiPopup)
            throw new ArgumentException(
                $"Popup must inherit Popup<{typeof(TResult).Name}>. " +
                $"Got {popup.GetType().Name}.",
                nameof(popup));

        var result = await Shell.Current.ShowPopupAsync<TResult>(
            mauiPopup, PopupOptions.Empty, ct);

        if (result.WasDismissedByTappingOutsideOfPopup)
            return default;

        return result.Result;
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