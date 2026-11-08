using Microsoft.Extensions.DependencyInjection;
using StepUp.Presentation.ViewModels;

namespace StepUp.Presentation
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            services.AddTransient<RegistrationViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<BodyDataViewModel>();
            services.AddTransient<MainViewModel>();

            return services;
        }
    }
}
