using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StepUp.Domain.Entities;

namespace StepUp.Data.Configurations
{
    public class MealConfiguration : IEntityTypeConfiguration<Meal>
    {
        public void Configure(EntityTypeBuilder<Meal> b)
        {
            b.ToTable("meals", t =>
            {
                t.HasCheckConstraint("ck_meals_meal_type", "meal_type IN ('Breakfast', 'Lunch', 'Dinner', 'Snack')");
            });

            b.HasKey(m => m.Id);
            b.Property(m => m.Id).ValueGeneratedNever();

            b.Property(m => m.DayResultId).HasColumnName("day_result_id").IsRequired();
            b.Property(m => m.MealType)
             .HasColumnName("meal_type")
             .HasConversion<string>()
             .HasMaxLength(20)
             .IsRequired();

            b.HasIndex(m => new { m.DayResultId, m.MealType })
             .IsUnique()
             .HasDatabaseName("ak_meals_day_result_type");

            b.HasIndex(m => m.DayResultId).HasDatabaseName("ix_meals_day_result_id");

            b.HasMany(m => m.Items)
             .WithOne()
             .HasForeignKey(i => i.MealId)
             .OnDelete(DeleteBehavior.Cascade)
             .HasConstraintName("fk_food_items_meals");

            b.Navigation(m => m.Items).UsePropertyAccessMode(PropertyAccessMode.Field);

            b.Ignore(m => m.TotalKcal);
            b.Ignore(m => m.TotalProtein);
            b.Ignore(m => m.TotalFat);
            b.Ignore(m => m.TotalCarbon);
        }
    }
}