
using Microcharts;
using SkiaSharp;
using WeightTracker.Models;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Alerts;
using WeightTracker.ModelViews;

namespace WeightTracker.Views;

public partial class GraphPage : ContentPage
{
	private DatabaseModelView _database;
	private GraphModelView _graph;
    private Dictionary<string, float> weekMonthDict;
    private Dictionary<string, float> yearDict;
	public GraphPage()
	{
		InitializeComponent();
		_database = new DatabaseModelView(new Services.FirestoreService());     
	}

	protected override async void OnAppearing()
	{
        base.OnAppearing();
		var weekMonthDict = await _database.ReturnDayWeightInfo(false);
        var yearDict = await _database.ReturnDayWeightInfo(true);
        _graph = new GraphModelView(weekMonthDict, yearDict);
        BindingContext = _graph;
        UpdateIMTIndicator(CalculateIMT(UserModel.MainUser.Weight, UserModel.MainUser.Height));
    }

    private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
    {
        if (e.Status == GestureStatus.Running)
        {
            var vm = (GraphModelView)BindingContext;
            vm.ZoomLevel *= Convert.ToSingle(e.Scale);
        }
    }

    private void OnDoubleTapped(object sender, EventArgs e)
    {
        // Сброс масштаба по двойному тапу
        var vm = (GraphModelView)BindingContext;
        vm.ZoomLevel = 1.0f;
    }

    private void PickerField_SelectedValueChanged(object sender, object e)
    {
        _graph.CreateGraph(DateTime.Now);
    }

    private double CalculateIMT(double weight, int height)
    {
        double heightM = height / 100.0;
        return weight / (heightM * heightM);
    }

    private string GetIMTCategory(double imt)
    {
        return imt < 18.9 ? "Дефицит веса" :
               imt < 24.9 ? "Нормальный вес" :
               imt < 29.9 ? "Избыточный вес" :
               imt < 34.9 ? "Ожирение 1 степени" :
               imt < 39.9 ? "Ожирение 2 степени" :
               "Ожирение 3 степени";
    }
    private Color GetIMTColor(double imt)
    {
        return imt < 18.9 ? Colors.RoyalBlue :
               imt < 24.9 ? Colors.Green :
               imt < 29.9 ? Colors.Yellow :
               imt < 34.9 ? Colors.Orange :
               imt < 39.9 ? Colors.Red : Colors.DarkRed;
    }
    private int GetIMTColumn(double imt)
    {
        return imt < 18.9 ? 0 :
               imt < 24.9 ? 1 :
               imt < 29.9 ? 2 :
               imt < 34.9 ? 3 :
               imt < 39.9 ? 4 : 5;
    }

    private void UpdateIMTIndicator(double imt)
    {
        int columnIndex = GetIMTColumn(imt);
        double relativeOffset;
        IMTIndicator.SetValue(Grid.ColumnProperty, 0);
        if (imt <= 8.9) relativeOffset = 0;
        else if (imt >= 43.9) relativeOffset = MyGrid.Width;
        else relativeOffset = (imt - 8.9) / (43.9 - 8.9) * MyGrid.Width;
        IMTIndicator.Margin = new Thickness(relativeOffset, 0, 0, 0);
        IMTLabel.Text += " " + Math.Round(imt, 1).ToString();
        IMTCategoryLabel.Text = GetIMTCategory(imt);
        IMTCategoryLabel.TextColor = GetIMTColor(imt);
    }
}