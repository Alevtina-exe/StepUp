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

        builder.Services.AddSingleton<ICurrentUserProvider, CurrentUserProvider>();

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
