using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StepUp.Domain.Entities;

namespace StepUp.Data.Configurations
{
    public class DayResultConfiguration : IEntityTypeConfiguration<DayResult>
    {
        public void Configure(EntityTypeBuilder<DayResult> b)
        {
            b.ToTable("day_results", t =>
            {
                t.HasCheckConstraint("ck_day_results_calorie_plan", "calorie_plan >= 1300");
                t.HasCheckConstraint("ck_day_results_weight", "weight BETWEEN 30 AND 300");
            });

            b.HasKey(d => d.Id);
            b.Property(d => d.Id).ValueGeneratedNever();

            b.Property(d => d.UserId).HasColumnName("user_id").IsRequired();
            b.Property(d => d.Date).HasColumnName("date").IsRequired();
            b.Property(d => d.CaloriePlan).HasColumnName("calorie_plan").IsRequired();
            b.Property(d => d.Weight).HasColumnName("weight");

            b.HasIndex(d => new { d.UserId, d.Date })
             .IsUnique()
             .HasDatabaseName("ak_day_results_user_date");

            b.HasIndex(d => d.UserId).HasDatabaseName("ix_day_results_user_id");
            b.HasIndex(d => d.Date).HasDatabaseName("ix_day_results_date");

            b.HasOne<User>()
             .WithMany()
             .HasForeignKey(d => d.UserId)
             .OnDelete(DeleteBehavior.Cascade)
             .HasConstraintName("fk_day_results_users");

            b.HasMany(d => d.Meals)
             .WithOne()
             .HasForeignKey(m => m.DayResultId)
             .OnDelete(DeleteBehavior.Cascade)
             .HasConstraintName("fk_meals_day_results");

            b.Navigation(d => d.Meals).UsePropertyAccessMode(PropertyAccessMode.Field);

            b.HasMany(d => d.Sports)
             .WithOne()
             .HasForeignKey(s => s.DayResultId)
             .OnDelete(DeleteBehavior.Cascade)
             .HasConstraintName("fk_sports_day_results");

            b.Navigation(d => d.Sports).UsePropertyAccessMode(PropertyAccessMode.Field);

            b.Ignore(d => d.Breakfast);
            b.Ignore(d => d.Lunch);
            b.Ignore(d => d.Dinner);
            b.Ignore(d => d.Snack);
            b.Ignore(d => d.KcalBurnt);
            b.Ignore(d => d.KcalEaten);
        }
    }
}