namespace StepUp.Domain.Entities
{
    public class FoodProduct
    {
        public string Id { get; }
        public string Name { get; private set; }
        public string Barcode { get; private set; }
        public Nutrients NutrientsPer100g { get; private set; }
        public FoodProduct(string id, string name, string barcode, Nutrients nutrientsPer100g)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название продукта не может быть пустым.", nameof(name));

            Id = id;
            Name = name;
            Barcode = barcode;
            NutrientsPer100g = nutrientsPer100g ?? throw new ArgumentNullException(nameof(nutrientsPer100g));
        }
    }
}