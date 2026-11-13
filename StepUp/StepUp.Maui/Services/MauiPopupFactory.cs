using StepUp.Maui.Popups;
using StepUp.Presentation.Enums;
using StepUp.Presentation.Interfaces;
using StepUp.Presentation.Models;

namespace StepUp.Maui.Services
{
    public class MauiPopupFactory : IPopupFactory
    {
        IPopup<SportResult?> IPopupFactory.CreateAddSportPopup() =>
            new AddSportPopup();

        IPopup<string?> IPopupFactory.CreateChangePasswordPopup() =>
            new ChangePasswordPopup();

        IPopup<SportResult?> IPopupFactory.CreateEditSportPopup(string originalName, int kcal) =>
            new EditSportPopup(originalName, kcal);

        IPopup<MenuAction> IPopupFactory.CreateMenuBarPopup() =>
            new Popups.MenuBar();
    }
}
