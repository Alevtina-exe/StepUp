using Microsoft.EntityFrameworkCore;
using StepUp.Domain.Entities;

namespace StepUp.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<DayResult> DayResults => Set<DayResult>();
    public DbSet<Meal> Meals => Set<Meal>();
    public DbSet<FoodItem> FoodItems => Set<FoodItem>();
    public DbSet<Sport> Sports => Set<Sport>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}