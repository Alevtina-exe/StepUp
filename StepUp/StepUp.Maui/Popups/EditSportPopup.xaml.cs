using CommunityToolkit.Maui.Views;
using StepUp.Presentation.Interfaces;
using StepUp.Presentation.Models;

namespace StepUp.Maui.Popups;

public partial class EditSportPopup : Popup<SportResult?>, IPopup<SportResult?>
{
    public EditSportPopup(string originalName, int kcal)
    {
        InitializeComponent();
        FormView.Configure(
            title: "Изменить физ. нагрузку",
            showDelete: true,
            name: originalName,
            kcal: kcal);

        FormView.SaveRequested += async (_, data)
            => await CloseAsync(new SportResult(data.Name, data.Kcal));
        FormView.DeleteRequested += async (_, _)
            => await CloseAsync(new SportResult("", 0, true));
        FormView.CancelRequested += async (_, _) => await CloseAsync(null);
    }
}