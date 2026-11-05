using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StepUp.Domain.Entities;

namespace StepUp.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("users", t =>
        {
            t.HasCheckConstraint("ck_users_full_name", "length(trim(full_name)) > 0");
            t.HasCheckConstraint("ck_users_email", "length(trim(email)) > 0");
            t.HasCheckConstraint("ck_users_password", "length(trim(password)) > 0");
            t.HasCheckConstraint("ck_users_gender", "gender IN ('Male', 'Female')");
            t.HasCheckConstraint("ck_users_sport_level", "sport_level IN ('No', 'Low', 'Mid', 'High', 'Extreme')");
            t.HasCheckConstraint("ck_users_weight", "weight BETWEEN 30 AND 300");
            t.HasCheckConstraint("ck_users_height", "height BETWEEN 100 AND 250");
            t.HasCheckConstraint("ck_users_calorie_plan", "calorie_plan >= 1300");
            t.HasCheckConstraint("ck_users_protein_pct", "protein_percent BETWEEN 1 AND 100");
            t.HasCheckConstraint("ck_users_fat_pct", "fat_percent BETWEEN 1 AND 100");
            t.HasCheckConstraint("ck_users_carbon_pct", "carbon_percent BETWEEN 1 AND 100");
            t.HasCheckConstraint("ck_users_macros_sum", "protein_percent + fat_percent + carbon_percent = 100");
        });

        b.HasKey(u => u.Id);
        b.Property(u => u.Id).ValueGeneratedNever();

        b.Property(u => u.FullName).HasColumnName("full_name").IsRequired().HasMaxLength(200);
        b.Property(u => u.Email).HasColumnName("email").IsRequired().HasMaxLength(256);
        b.Property(u => u.Password).HasColumnName("password").IsRequired().HasMaxLength(256);

        b.HasIndex(u => u.Email).IsUnique().HasDatabaseName("ak_users_email");

        b.Property(u => u.BirthDate).HasColumnName("birth_date").IsRequired();
        b.Property(u => u.Gender)
         .HasColumnName("gender")
         .HasConversion<string>()
         .HasMaxLength(20)
         .IsRequired();
        b.Property(u => u.Weight).HasColumnName("weight").IsRequired();
        b.Property(u => u.Height).HasColumnName("height").IsRequired();
        b.Property(u => u.CaloriePlan).HasColumnName("calorie_plan").IsRequired();
        b.Property(u => u.CarbonPercent).HasColumnName("carbon_percent").IsRequired();
        b.Property(u => u.ProteinPercent).HasColumnName("protein_percent").IsRequired();
        b.Property(u => u.FatPercent).HasColumnName("fat_percent").IsRequired();
        b.Property(u => u.SportLevel)
         .HasColumnName("sport_level")
         .HasConversion<string>()
         .HasMaxLength(20)
         .IsRequired();
        b.Property(u => u.RegistrationDate).HasColumnName("registration_date").IsRequired();

        b.Ignore(u => u.Age);
    }
}