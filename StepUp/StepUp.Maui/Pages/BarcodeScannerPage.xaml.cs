using StepUp.Domain.Enums;
using StepUp.Presentation.Constants;
using ZXing.Net.Maui;

namespace StepUp.Maui.Pages;
public partial class BarcodeScannerPage : ContentPage, IQueryAttributable
{
    private DateTime? _date = null;
    private MealType? _mealType = null;
    
    public BarcodeScannerPage()
    {
        InitializeComponent();
        barcodeView.Options = new BarcodeReaderOptions
        {
            AutoRotate = true,
            TryHarder = true,
            Formats = BarcodeFormat.Ean8 | BarcodeFormat.Ean13 |
                      BarcodeFormat.UpcA | BarcodeFormat.UpcE
        };
        NavigationPage.SetHasNavigationBar(this, false);
        
#if ANDROID
        barcodeView.Handler?.UpdateValue(nameof(barcodeView.CameraLocation));
#endif
    }

    private async Task OnBarcodeDetected(object sender, BarcodeDetectionEventArgs e)
    {
        var first = e.Results?.FirstOrDefault();
        if (first is null) return;

        barcodeView.IsDetecting = false;

        if (_date == null || _mealType == null)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }

        await Shell.Current.GoToAsync(AppRoutes.SearchProduct,
            new Dictionary<string, object>
            {
                {"Date", _date.Value },
                {"MealType", _mealType.Value }
            });
    }


    private void Button_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Date", out var d) && d is DateTime date)
            _date = date;
        if (query.TryGetValue("MealType", out var m) && m is MealType mt)
            _mealType = mt;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if(_date == null || _mealType == null)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }
    }
}