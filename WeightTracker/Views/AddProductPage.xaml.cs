using AndroidX.Core.App;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using WeightTracker.Models;
using WeightTracker.ModelViews;
namespace WeightTracker.Views;

public partial class AddProductPage : ContentPage
{
	private ProductBaseModelView ProductBaseModelView { get; set; }
    private Meal _meal;
	public AddProductPage(Meal meal)
	{
		InitializeComponent();
        _meal = meal;
		ProductBaseModelView = new ProductBaseModelView(Navigation);
		BindingContext = ProductBaseModelView;
        NavigationPage.SetHasNavigationBar(this, false);
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        SearchEntry.HideSoftInputAsync(CancellationToken.None);
    }

    private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if(ProductBaseModelView.SelectedItem.Name != null)
            await Navigation.PushAsync(new ProductPage(ProductBaseModelView.SelectedItem, _meal));
    }


}