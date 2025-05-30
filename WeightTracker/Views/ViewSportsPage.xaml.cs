using AiForms.Settings;
using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WeightTracker.Models;
using WeightTracker.ModelViews;
using static Android.Icu.Text.CaseMap;

namespace WeightTracker.Views;

public partial class ViewSportsPage : ContentPage, INotifyPropertyChanged
{
	private StepModelView _stepModelView;
    private ObservableCollection<Sportik> _sports;
    public ObservableCollection<Sportik> Sports
    {
        get => _sports;
        set
        {
            _sports = value;
            OnPropertyChanged();
        }
    }
    public class Sportik { 
		public string SportName { get; set; }
		public int SportCals { get; set; }
		public Sportik(string name, int cals)
		{
			SportName = name;
			SportCals = cals;
		}

	}
	private Sportik _selectedItem;
	public Sportik SelectedItem
	{
		get => _selectedItem;
		set
		{
			_selectedItem = value;
			OnPropertyChanged();
		}
	}
	public ViewSportsPage()
	{
		InitializeComponent();
		_stepModelView = new StepModelView();
		BindingContext = this;
		Sports = new ObservableCollection<Sportik>();
        if (DayResult.CurrentDay != null)
        {
            if (DayResult.CurrentDay.Date == DateTime.Now)
            {
                Sports.Add(new Sportik("ױמהüבא", int.Parse(_stepModelView.Calories.Split(' ')[0])));
            }
            else
            {
                int WalkKcal = DayResult.CurrentDay.KcalSpent;
                foreach (var item in DayResult.CurrentDay.Sports)
                {
                    WalkKcal -= Convert.ToInt32(item.Value);
                }
                Sports.Add(new Sportik("ױמהüבא", WalkKcal));
            }
            foreach (var kvp in DayResult.CurrentDay.Sports)
            {
                Sports.Add(new Sportik(kvp.Key, Convert.ToInt32(kvp.Value)));
            }
        }
    }
    private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
		if(SelectedItem.SportName != "ױמהüבא")
		{
            var res = await this.ShowPopupAsync(new AddSport(SelectedItem.SportName, SelectedItem.SportCals));
			if(res is true)
			{
				Sports.Remove(SelectedItem);
			}
        }
    }
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}