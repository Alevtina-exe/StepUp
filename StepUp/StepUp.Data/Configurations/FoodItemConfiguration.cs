using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StepUp.Domain.Entities;

namespace StepUp.Data.Configurations
{
    public class FoodItemConfiguration : IEntityTypeConfiguration<FoodItem>
    {
        public void Configure(EntityTypeBuilder<FoodItem> b)
        {
            b.ToTable("food_items", t =>
            {
                t.HasCheckConstraint("ck_food_items_amount_g", "amount_g >= 0");
                t.HasCheckConstraint("ck_food_items_product_id", "length(trim(product_id)) > 0");
                t.HasCheckConstraint("ck_food_items_name", "length(trim(name)) > 0");
                t.HasCheckConstraint("ck_food_items_kcal_100g", "kcal_100g    >= 0");
                t.HasCheckConstraint("ck_food_items_protein_100g", "protein_100g >= 0");
                t.HasCheckConstraint("ck_food_items_fat_100g", "fat_100g     >= 0");
                t.HasCheckConstraint("ck_food_items_carbon_100g", "carbon_100g  >= 0");
            });

            b.HasKey(i => i.Id);
            b.Property(i => i.Id).ValueGeneratedNever();

            b.Property(i => i.MealId).HasColumnName("meal_id").IsRequired();
            b.Property(i => i.ProductId).HasColumnName("product_id").IsRequired().HasMaxLength(100);
            b.Property(i => i.Name).HasColumnName("name").IsRequired().HasMaxLength(300);
            b.Property(i => i.Barcode).HasColumnName("barcode").HasMaxLength(50);
            b.Property(i => i.AmountGrams).HasColumnName("amount_g").IsRequired();

            b.HasIndex(i => i.MealId).HasDatabaseName("ix_food_items_meal_id");
            b.HasIndex(i => i.ProductId).HasDatabaseName("ix_food_items_product_id");
            b.HasIndex(i => i.Barcode)
             .HasDatabaseName("ix_food_items_barcode")
             .HasFilter("barcode IS NOT NULL");

            b.OwnsOne(i => i.NutrientsPer100g, n =>
            {
                n.Property(x => x.Calories).HasColumnName("kcal_100g").IsRequired();
                n.Property(x => x.Proteins).HasColumnName("protein_100g").IsRequired();
                n.Property(x => x.Fats).HasColumnName("fat_100g").IsRequired();
                n.Property(x => x.Carbohydrates).HasColumnName("carbon_100g").IsRequired();
            });
            b.Navigation(i => i.NutrientsPer100g).IsRequired();

            b.Ignore(i => i.TotalNutrients);
        }
    }
}