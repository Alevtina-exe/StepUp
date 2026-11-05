using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StepUp.Domain.Entities;

namespace StepUp.Data.Configurations
{
    public class SportConfiguration : IEntityTypeConfiguration<Sport>
    {
        public void Configure(EntityTypeBuilder<Sport> b)
        {
            b.ToTable("sports", t =>
            {
                t.HasCheckConstraint("ck_sports_name", "length(trim(name)) > 0");
                t.HasCheckConstraint("ck_sports_burnt_kcals", "burnt_kcals >= 0");
            });

            b.HasKey(s => s.Id);
            b.Property(s => s.Id).ValueGeneratedNever();

            b.Property(s => s.DayResultId).HasColumnName("day_result_id").IsRequired();
            b.Property(s => s.Name).HasColumnName("name").IsRequired().HasMaxLength(200);
            b.Property(s => s.BurntKcals).HasColumnName("burnt_kcals").IsRequired();

            b.HasIndex(s => s.DayResultId).HasDatabaseName("ix_sports_day_result_id");
        }
    }
}