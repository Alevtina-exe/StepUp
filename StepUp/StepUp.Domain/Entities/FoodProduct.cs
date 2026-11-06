namespace StepUp.Domain.Entities
{
    public class FoodProduct
    {
        public string Id { get; }
        public string Name { get; }
        public string Barcode { get; }
        public string Brands { get; }
        public string? NutriscoreGrade { get; }
        public string? NovaGroup { get; }
        public string? ImageUrl { get; }
        public double Sugars100g { get; }
        public Serving? MainServing { get; }
        public Nutrients NutrientsPer100g { get;}
        public FoodProduct(string id,
                           string? name,
                           string? barcode,
                           Nutrients nutrientsPer100g,
                           Serving? mainServing,
                           string? brands,
                           string? nutriscoreGrade,
                           string? novaGroup,
                           string? imageUrl,
                           double sugars100g = 0)
        {
            Id = id;
            Name = name ?? "Без названия";
            Barcode = barcode ?? "Не указан";
            NutrientsPer100g = nutrientsPer100g ?? throw new ArgumentNullException(nameof(nutrientsPer100g));
            MainServing = mainServing;
            Brands = brands ?? "Не указан";
            NutriscoreGrade = nutriscoreGrade;
            NovaGroup = novaGroup;
            ImageUrl = imageUrl;
            Sugars100g = sugars100g;
        }
    }
}