using CommunityToolkit.Mvvm.Messaging;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;
using ZXing.Net.Maui.Readers;

namespace WeightTracker.Views;
public partial class BarcodeScannerPage : ContentPage
{
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

    private void OnBarcodeDetected(object sender, BarcodeDetectionEventArgs e)
    {
        var first = e.Results?.FirstOrDefault();
        if (first is null) return;

        Application.Current.Dispatcher.Dispatch(() =>
        {
            // Отправляем сообщение через WeakReferenceMessenger
            WeakReferenceMessenger.Default.Send(first.Value);

            // Закрываем страницу после успешного сканирования
            Navigation.PopAsync();
        });
    }


private void Button_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}