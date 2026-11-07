using Microsoft.Extensions.DependencyInjection;
using StepUp.Application.Services;

namespace StepUp.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<AuthService>();
            services.AddScoped<DayResultService>();
            services.AddScoped<ProductSearchService>();
            return services;
        }
    }
}
