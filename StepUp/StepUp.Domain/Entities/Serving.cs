namespace StepUp.Domain.Entities;
public sealed class Serving
{
    public string Name { get; } = null!;
    public double Quantity { get; }          
    public bool IsLiquid { get; }             

    private Serving() { }

    public Serving(string name, double quantity, bool isLiquid = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название порции обязательно.", nameof(name));
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Количество должно быть > 0.");

        Name = name;
        Quantity = quantity;
        IsLiquid = isLiquid;
    }

    public static Serving Grams(double qty = 100) => new("г", qty);
    public static Serving Milliliters(double qty = 100) => new("мл", qty, isLiquid: true);
    public static Serving Portion(double grams) => new("порция", grams);
    public static Serving Piece(double grams) => new("шт", grams);
    public static Serving Cup(double ml = 200) => new("стакан", ml, isLiquid: true);
    public static Serving Tablespoon() => new("ст. л.", 15, isLiquid: true);
    public static Serving Teaspoon() => new("ч. л.", 5, isLiquid: true);

    public double ToGrams(double amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Количество не может быть отрицательным.");

        return amount * Quantity;                     
    }

    public static double operator *(Serving serving, double amount) => serving.ToGrams(amount);
    public static double operator *(double amount, Serving serving) => serving.ToGrams(amount);


    public string Unit => IsLiquid ? "мл" : "г";
    public string DisplayName => Quantity == 1 ? Name : $"{Name} ({Quantity:0.##} {Unit})";
    public override string ToString() => DisplayName;
}