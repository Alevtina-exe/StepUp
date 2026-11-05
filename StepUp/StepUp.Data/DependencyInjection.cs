using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StepUp.Application.Interfaces.Repositories;
using StepUp.Data;
using StepUp.Data.Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddData(this IServiceCollection services, string dbPath)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDayResultRepository, DayResultRepository>();

        return services;
    }
}