using System.Resources;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using WeightTracker.Models;

namespace WeightTracker;

public partial class WelcomeModelView : ObservableObject
{
    public ICommand TapGestureCommand { get; }
    private async void OnLabelTapped()
    {
        await Shell.Current.GoToAsync("//RegistrationPage"); // Переход на страницу регистрации
    }
    private static string[] Colors = {"ee6002", "6200EE", "75e900", "FFDE03"};

    [ObservableProperty]
    private string title = Titles[0];

    private static string[] Titles = {"Hey There!", "Chose Your Gender", "Choose a Unit", "What's Your Height"};

    [ObservableProperty]
    private FontImageSource image = new FontImageSource() {Glyph= Images[0], FontFamily = "FontAwesomeSolid", Size = 150, Color = Color.FromArgb(Colors[0])};
    
    private static string[] Images = {"\uf0ae", "\ue548", "\uf545", "\uf5ae"};

    [ObservableProperty]
    private string pickerTitle = PickerTitles[0];

    private static string[] PickerTitles = {"", "Gender", "Unit of Measurement", "Height (CM)"};
    
    [ObservableProperty]
    private string description = "We just need a few small btis of information to get going :)";
    
    [ObservableProperty]
    private bool isTitle = true;

    [ObservableProperty]
    private bool isPicker = false;

    [ObservableProperty]
    private List<string> pickerItems = new List<string>();

    public List<string>[] Items = {
        new List<string>(),
        new List<string>() {"Male", "Female"}, 
        new List<string>() {"Imperial", "Metric"}, 
        new List<string>() 
    };

    [ObservableProperty]
    private string selectedItem = "";

    [ObservableProperty]
    private bool isMetricHeight = false;
    
    [ObservableProperty]
    private bool isImperialHeight = false;

    [ObservableProperty]
    private double height = 0;
    
    [ObservableProperty]
    private string heightFt = "";
    
    [ObservableProperty]
    private string heightIn = "";

    private int Index = 0;

    public WelcomeModelView() {
        TapGestureCommand = new Command(OnLabelTapped);
    }

    public bool Increment() {
        return false;
    }

    public bool Save() {
        // mark app as init
        Preferences.Set("isAppInit", DateTime.Now);

        return true;
    }
}
