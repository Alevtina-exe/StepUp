namespace StepUp.Domain.Entities;
public class FoodItem
{
    public Guid Id { get; }
    public Guid MealId { get; }
    public string ProductId { get; } = null!;
    public string Name { get; } = null!;
    public string? Barcode { get; }
    public double Amount { get; private set; }
    public Serving Serving { get; private set; } = null!;
    public Nutrients NutrientsPer100g { get; } = null!;

    private FoodItem() { }
    public FoodItem(
        string productId,
        Guid mealId,
        string name,
        string? barcode,
        Nutrients nutrientsPer100g,
        Serving? serving,
        double amount = 1)
    {
        Id = Guid.NewGuid();
        if (string.IsNullOrWhiteSpace(productId))
            throw new ArgumentException("Id продукта обязателен.", nameof(productId));
        if (amount < 0)
            throw new ArgumentException("Количество не может быть отрицательным.", nameof(amount));

        ProductId = productId;
        MealId = mealId;
        Name = name;
        Barcode = barcode;
        Amount = amount;
        NutrientsPer100g = nutrientsPer100g
            ?? throw new ArgumentNullException(nameof(nutrientsPer100g));
        if(serving != null)
        {
            Serving = serving;
            Amount = amount;
        }
        else
        {
            Serving = new Serving("г", 1);
            Amount = 100;
        }
    }

    public FoodItem(FoodProduct product, Guid mealId, double amount = 1, Serving? serving = null)
        : this(product.Id, mealId, product.Name, product.Barcode, 
               product.NutrientsPer100g,serving ?? product.MainServing, amount)
    {  }
    public Nutrients TotalNutrients => NutrientsPer100g.CalculatePerAmount(Serving*Amount);

    public void ChangeAmount(double newAmount)
    {
        if (newAmount < 0)
            throw new ArgumentException("Количество не может быть отрицательным.", nameof(newAmount));
        Amount = newAmount;
    }
    public void AddToAmount(double amount)
    {
        if(amount < 0)
            throw new ArgumentException("Количество не может быть отрицательным.", nameof(amount));
        Amount += amount;
    }
    public void ChangeServing(Serving serving) => Serving = serving;
}