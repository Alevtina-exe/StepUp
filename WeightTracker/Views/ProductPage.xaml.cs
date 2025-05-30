using System.Threading.Tasks;
using WeightTracker.Models;
using WeightTracker.ModelViews;

namespace WeightTracker.Views;

public partial class ProductPage : ContentPage
{
	public FoodProduct Product { get; set; }
	public ProductPageModelView ProductPageModelView { get; set; }
    private DatabaseModelView _database;
    private Meal _meal;
    private bool _edited;

	public ProductPage(FoodProduct product, Meal meal)
	{
        InitializeComponent();
        Product = product;
        _database = new DatabaseModelView(new Services.FirestoreService());
        ProductPageModelView = new ProductPageModelView(product, grid, _meal);
        _meal = meal;
        if (product.Serving != null)
        {
            _edited = true;
            DeleteButton.IsVisible = true;
            PickMeal.IsVisible = false;
            AddButton.Text = "Изменить";
            ProductPageModelView.SelectedItem = product.Serving;
            ProductPageModelView.Amount = Math.Round(product.Amount / ProductPageModelView.CurrentPortion_gr, 2).ToString();
        }
        else
        {
            _edited = false;
        }

        NavigationPage.SetHasNavigationBar(this, false);
	}

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        double w;
        if (Double.TryParse(entry.Text, out w) && w != 0)
        {
            entry.TextColor = Colors.Black;
            AddButton.IsEnabled = true;
        }
        else
        {
            entry.TextColor = Colors.Red;
            AddButton.IsEnabled = false;
        }      
    }
    private void RenewMeal(Dictionary<string, object> meal, Dictionary<string, double> add)
    {
        foreach (var el in meal)
        {
            if (el.Key != "Dishes")
            {
                meal[el.Key] = AddToObj(el.Value, add[el.Key], el.Key == "Kcal");
            }
            else
            {
                if(ProductPageModelView.CurrentPortion_gr != 0)
                {
                    ((Dictionary<string, object>)meal[el.Key])[Product.Barcode] =
                            ProductPageModelView.CurrentPortion_gr.ToString() + "-" +
                                ProductPageModelView.SelectedItem;
                }
                else
                {
                    ((Dictionary<string, object>)meal[el.Key]).Remove(Product.Barcode);
                }
            }
        }
    }
    private object AddToObj(object obj, double value, bool ifInt)
    {
        if (ifInt)
        {
            obj = (int)(Math.Round(Convert.ToDouble(obj) + value));
        }
        else
        {
            obj = Math.Round(Convert.ToDouble(obj) + value, 1);
        }
        return obj;
    }

    private async void AddButton_Clicked(object sender, EventArgs e)
    {
        double gr = ProductPageModelView.CurrentPortion_gr, old_gr = 0;
        if (_edited)
        {
            old_gr = Product.Amount;
        }
        Dictionary<string, double> dict = new Dictionary<string, double>
        {
            {"Kcal", (gr - old_gr) / 100 * Product.Cal100g},
            {"Pr", (gr - old_gr)/ 100 * Product.Protein100g},
            {"Fat", (gr - old_gr)/ 100 * Product.Fat100g},
            {"Cb", (gr - old_gr) / 100 *Product.Fat100g}
        };
        switch (ProductPageModelView.MealItem)
        {
            case "Завтрак":
                RenewMeal(DayResult.CurrentDay.Breakfast, dict);
                break;
            case "Обед":
                RenewMeal(DayResult.CurrentDay.Lunch, dict);
                break;
            case "Ужин":
                RenewMeal(DayResult.CurrentDay.Dinner, dict);
                break;
            case "Перекус":
                RenewMeal(DayResult.CurrentDay.Snack, dict);
                break;
            default: 
                break;              
        }
        DayResult.CurrentDay.KcalEaten += (int)Math.Round((gr - old_gr) / 100 * Product.Cal100g);
        DayResult.CurrentDay.KcalRes -= (int)Math.Round((gr - old_gr) / 100 * Product.Cal100g);
        await _database.AddMeal(ProductPageModelView.MealItem, Product, 
                                gr, ProductPageModelView.SelectedItem);
        await Navigation.PopAsync();
    }

    private void DeleteButton_Clicked(object sender, EventArgs e)
    {
        ProductPageModelView.CurrentPortion_gr = 0;
        AddButton_Clicked(sender, e);
    }

    private void favButton_Clicked(object sender, EventArgs e)
    {
        if (Application.Current.Resources.TryGetValue("IconFavourite", out var favIcon) &&
            Application.Current.Resources.TryGetValue("IconNotFavourite", out var notFavIcon))
        {
            if (favButton.Source == (ImageSource)notFavIcon)
            {
                favButton.Source = (ImageSource)favIcon;
            }
            else
            {
                favButton.Source = (ImageSource)notFavIcon;
            }
        }
    }
}