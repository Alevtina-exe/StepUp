using StepUp.Presentation.Enums;
using StepUp.Presentation.Models;

namespace StepUp.Presentation.Interfaces;

public interface IPopupFactory
{
    IPopup<SportResult?> CreateAddSportPopup();
    IPopup<SportResult?> CreateEditSportPopup(string originalName, int kcal);
    IPopup<MenuAction> CreateMenuBarPopup();
    IPopup<string?> CreateChangePasswordPopup();
}