namespace StepUp.Domain.Entities;
public class FoodItem
{
    public Guid Id { get; }
    public Guid MealId { get; }
    public string ProductId { get; }
    public string Name { get; }
    public string? Barcode { get; }
    public double AmountGrams { get; private set; }
    public Nutrients NutrientsPer100g { get; }

    public FoodItem(
        string productId,
        Guid mealId,
        string name,
        string? barcode,
        double amountGrams,
        Nutrients nutrientsPer100g)
    {
        Id = Guid.NewGuid();
        if (string.IsNullOrWhiteSpace(productId))
            throw new ArgumentException("Id продукта обязателен.", nameof(productId));
        if (amountGrams < 0)
            throw new ArgumentException("Количество не может быть отрицательным.", nameof(amountGrams));

        ProductId = productId;
        MealId = mealId;
        Name = name;
        Barcode = barcode;
        AmountGrams = amountGrams;
        NutrientsPer100g = nutrientsPer100g
            ?? throw new ArgumentNullException(nameof(nutrientsPer100g));
    }

    public FoodItem(FoodProduct product, Guid mealId, double amountGrams)
        : this(product.Id, mealId, product.Name, product.Barcode, amountGrams, product.NutrientsPer100g)
    {
    }

    public Nutrients TotalNutrients => NutrientsPer100g.CalculatePerAmount(AmountGrams);

    public void ChangeAmount(double newAmountGrams)
    {
        if (newAmountGrams < 0)
            throw new ArgumentException("Количество не может быть отрицательным.", nameof(newAmountGrams));
        AmountGrams = newAmountGrams;
    }
    public void AddToAmount(double amountGrams)
    {
        if(amountGrams < 0)
            throw new ArgumentException("Количество не может быть отрицательным.", nameof(amountGrams));
        AmountGrams += amountGrams;
    }
}