using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StepUp.Application.Interfaces;
using StepUp.Application.Interfaces.Services;
using StepUp.Infrastructure.OpenFoodFacts;
using StepUp.Infrastructure.Security;
using StepUp.Infrastructure.Time;

namespace StepUp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            var offSection = configuration.GetSection("OpenFoodFacts");
            var baseUrl = offSection["BaseUrl"] ?? "https://ru.openfoodfacts.org/";
            var timeoutSeconds = offSection.GetValue<int?>("TimeoutSeconds") ?? 30;
            var userAgent = offSection["UserAgent"] ?? "StepUp/1.0 (MAUI)";
            services.AddHttpClient<IFoodProductClient, OpenFoodFactsClient>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
            services.AddSingleton<IClock, SystemClock>();
            return services;
        }
    }
}
