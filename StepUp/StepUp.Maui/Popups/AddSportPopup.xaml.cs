using CommunityToolkit.Maui.Views;
using StepUp.Presentation.Interfaces;
using StepUp.Presentation.Models;

namespace StepUp.Maui.Popups;

public partial class AddSportPopup : Popup<SportResult?>, IPopup<SportResult?>
{
    public AddSportPopup()
    {
        InitializeComponent();
        FormView.Configure(title: "Добавить физ. нагрузку", showDelete: false);

        FormView.SaveRequested += async (_, data)
            => await CloseAsync(new SportResult(data.Name, data.Kcal));
        FormView.CancelRequested += async (_, _) => await CloseAsync(null);
    }
}