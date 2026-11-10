using AiForms.Settings;
using CommunityToolkit.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StepUp.Application;
using StepUp.Application.Interfaces.Services;
using StepUp.Data;
using StepUp.Infrastructure;
using StepUp.Maui.Services;
using StepUp.Maui.Views;
using StepUp.Presentation;
using StepUp.Presentation.Interfaces;
using System.Reflection;
using UraniumUI;
using ZXing.Net.Maui.Controls;

namespace StepUp.Maui;

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

        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("StepUp.Maui.appsettings.json")
            ?? throw new InvalidOperationException("appsettings.json is not found");

        builder.Configuration.AddJsonStream(stream);

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "stepup.db");
        builder.Services.AddData(dbPath);

        builder.Services.AddInfrastructure(builder.Configuration);
		builder.Services.AddApplication();
		builder.Services.AddViewModels();

        builder.Services.AddSingleton<ICurrentUserProvider, CurrentUserProvider>();
        builder.Services.AddSingleton<INavigationService, MauiNavigationService>();
        builder.Services.AddSingleton<IDialogService, MauiDialogService>();
        builder.Services.AddSingleton<IPopupFactory, MauiPopupFactory>();

        builder.Services.AddTransient<RegistrationPage>();
		builder.Services.AddTransient<BodyDataPage>();
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<SettingsPage>();

		builder.Services.AddTransient<AddProductPage>();
		builder.Services.AddTransient<BarcodeScannerPage>();
		builder.Services.AddTransient<EditAccountPage>();
		builder.Services.AddTransient<GraphPage>();
		builder.Services.AddTransient<ProductPage>();
		builder.Services.AddTransient<ViewMealPage>();
		builder.Services.AddTransient<SportListPage>();
		

#if DEBUG
        builder.Logging.AddDebug();
#endif

		var app = builder.Build();
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }
        return app;
	}
}
