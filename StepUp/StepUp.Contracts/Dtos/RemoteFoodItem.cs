using Google.Cloud.Firestore;
using StepUp.Domain.Entities;
namespace StepUp.Contracts.Dtos;
[FirestoreData]
public class RemoteFoodItem
{
    [FirestoreProperty("product_id")]
    public string ProductId { get; set; } = null!;

    [FirestoreProperty("name")]
    public string Name { get; set; } = null!;

    [FirestoreProperty("barcode")]
    public string? Barcode { get; set; }

    [FirestoreProperty("amount_g")]
    public double AmountGrams { get; set; }

    [FirestoreProperty("kcal_100g")]
    public double KcalPer100g { get; set; }

    [FirestoreProperty("protein_100g")]
    public double ProteinPer100g { get; set; }

    [FirestoreProperty("fat_100g")]
    public double FatPer100g { get; set; }

    [FirestoreProperty("carbon_100g")]
    public double CarbonPer100g { get; set; }

    public RemoteFoodItem() { } 

    public RemoteFoodItem(FoodItem item)
    {
        ProductId = item.ProductId;
        Name = item.Name;
        Barcode = item.Barcode;
        AmountGrams = item.AmountGrams;
        KcalPer100g = item.NutrientsPer100g.Calories;
        ProteinPer100g = item.NutrientsPer100g.Proteins;
        FatPer100g = item.NutrientsPer100g.Fats;
        CarbonPer100g = item.NutrientsPer100g.Carbohydrates;
    }

    public FoodItem ToDomain() => new(
        ProductId,
        Name,
        Barcode,
        AmountGrams,
        new Nutrients(KcalPer100g, ProteinPer100g, FatPer100g, CarbonPer100g)
    );
}