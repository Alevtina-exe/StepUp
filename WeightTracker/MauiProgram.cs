using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using UraniumUI;
using Microcharts.Maui;
using AiForms.Settings;
using ZXing.Net.Maui.Controls;
using WeightTracker.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.LifecycleEvents;
namespace WeightTracker;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

        builder
			.UseMauiApp<App>()
		    .UseMauiCommunityToolkit()
			.UseUraniumUI()
			.UseUraniumUIMaterial()
			.UseMicrocharts()
			.UseBarcodeReader()
            .ConfigureMauiHandlers(handlers =>
			{
				handlers.AddSettingsViewHandler();
			})
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("Font-Awesome-6-Brands.otf", "FontAwesomeBrands");
				fonts.AddFont("Font-Awesome-6-Solid.otf", "FontAwesomeSolid");
				fonts.AddFont("Font-Awesome-6-Regular.otf", "FontAwesomeRegular");
				fonts.AddFont("impact.ttf", "impact");
			});

#if DEBUG
        builder.Logging.AddDebug();
#endif
    return builder.Build();
	}
}
