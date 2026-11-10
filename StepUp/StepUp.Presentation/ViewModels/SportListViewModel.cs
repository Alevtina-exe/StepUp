using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StepUp.Application.Services;
using StepUp.Domain.Entities;
using StepUp.Presentation.Interfaces;
using System.Collections.ObjectModel;

namespace StepUp.Presentation.ViewModels
{
    public partial class SportListViewModel(DayResultService dayService,
        INavigationService nav, IDialogService dialog,
        IPopupFactory popups) : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Sport> _sports = [];
        [ObservableProperty]
        private DayResult? _dayRes;

        public async Task InitializeAsync(DateTime? date)
        {
            if (date == null)
            {
                await nav.GoBackAsync();
                return;
            }
            DayRes = await dayService.GetDayAsync(date.Value);
            if (DayRes == null)
            {
                await nav.GoBackAsync();
            }
        }

        partial void OnDayResChanged(DayResult? value)
        {
            if (value == null) return;
            Sports = new ObservableCollection<Sport>(value.Sports);
        }

        [RelayCommand]
        private async Task EditSportAsync(Sport item)
        {
            if (item == null) return;
            var editedSport = await dialog.ShowPopupAsync(
                popups.CreateEditSportPopup(item.Name, item.BurntKcals));
            if (editedSport != null && DayRes != null)
            {
                try
                {
                    if (editedSport.IsDeleted)
                    {
                        DayRes = await dayService.RemoveSportAsync(DayRes.Date, item.Id);
                    }
                    else
                    {
                        DayRes = await dayService.EditSportAsync(DayRes.Date, item.Id,
                            editedSport.Name, editedSport.Calories);
                    }
                }
                catch (Exception ex)
                {
                    await dialog.ShowAlertAsync("Ошибка", ex.Message);
                }
            }
        }
    }
}
