using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WeightTracker.Models;
using WeightTracker.ModelViews;
using WeightTracker.Services;

namespace WeightTracker.Views;

public partial class ViewMealPage : ContentPage, INotifyPropertyChanged
{
	private bool _deleted;
	private ObservableCollection<FoodProduct> _dishes;
	private OpenFoodFactsService _service;
	private Meal _meal;
	public string MealName => _meal.GetDescription();
	public ObservableCollection<FoodProduct> Dishes
	{
		get => _dishes;
		set
		{
			_dishes = value;
			OnPropertyChanged();
		}
	}
	private FoodProduct _selectedItem;
	public FoodProduct SelectedItem
	{
		get => _selectedItem;
		set
		{
			_selectedItem = value;
			OnPropertyChanged();
		}
	}
	private async Task FromDictToCollection (object dict)
	{
		foreach(var dishes in (Dictionary<string, object>)dict)
		{
			double amount = double.Parse(dishes.Value.ToString().Split('-')[0]);
			string serving = dishes.Value.ToString().Split('-')[1];
			var res = await _service.GetProductByBarcodeAsync(dishes.Key);
			res.Amount = amount;
			res.Serving = serving;
			Dishes.Add(res);
		}
	}
	public ViewMealPage(Meal meal)
	{
		InitializeComponent();
        BindingContext = this;
		_service = new OpenFoodFactsService();
		Dishes = new ObservableCollection<FoodProduct>();
		_meal = meal;
	}
	protected override async void OnAppearing() 
	{
		Dishes.Clear();
		switch (_meal)
		{
			case Meal.Breakfast:
				await FromDictToCollection(DayResult.CurrentDay.Breakfast["Dishes"]);
				break;
			case Meal.Lunch:
				await FromDictToCollection(DayResult.CurrentDay.Lunch["Dishes"]);
				break;
			case Meal.Dinner:
				await FromDictToCollection(DayResult.CurrentDay.Dinner["Dishes"]);
				break;
			case Meal.Snack:
				await FromDictToCollection(DayResult.CurrentDay.Snack["Dishes"]);
				break;
			default:
				break;
        }
			
	}
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
		await Navigation.PushAsync(new ProductPage(SelectedItem, _meal));
    }
}