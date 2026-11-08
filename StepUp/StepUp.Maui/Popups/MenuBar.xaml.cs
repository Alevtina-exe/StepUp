using CommunityToolkit.Maui.Views;
using StepUp.Presentation.Enums;

namespace StepUp.Maui.Popups
{
    public partial class MenuBar : Popup<MenuAction>
    {
        public MenuBar() => InitializeComponent();

        private async void AddWeight_Clicked(object sender, EventArgs e) => await CloseAsync(MenuAction.ChangeWeight);
        private async void AddBreakfast_Clicked(object sender, EventArgs e) => await CloseAsync(MenuAction.Breakfast);
        private async void AddLunch_Clicked(object sender, EventArgs e) => await CloseAsync(MenuAction.Lunch);
        private async void AddDinner_Clicked(object sender, EventArgs e) => await CloseAsync(MenuAction.Dinner);
        private async void AddSnack_Clicked(object sender, EventArgs e) => await CloseAsync(MenuAction.Snack);
        private async void AddSport_Clicked(object sender, EventArgs e) => await CloseAsync(MenuAction.Sport);
    }
}