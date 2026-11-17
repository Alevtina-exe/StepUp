using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
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
            })
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
                options.Retry.BackoffType = DelayBackoffType.Exponential;
                options.Retry.UseJitter = true;
                options.Retry.ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .HandleResult(r => (int)r.StatusCode >= 500);   
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);
            });
            services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
            services.AddSingleton<IClock, SystemClock>();
            return services;
        }
    }
}
