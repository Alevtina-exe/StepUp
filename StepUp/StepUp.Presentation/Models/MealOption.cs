using StepUp.Domain.Extensions;

namespace StepUp.Presentation.Models
{
    public record MealOption(MealType Type, string Name)
    {
        public static readonly MealOption Breakfast =
            new(MealType.Breakfast, MealType.Breakfast.GetDescription());
        public static readonly MealOption Lunch =
            new(MealType.Lunch, MealType.Lunch.GetDescription());
        public static readonly MealOption Dinner =
            new(MealType.Dinner, MealType.Dinner.GetDescription());
        public static readonly MealOption Snack =
            new(MealType.Snack, MealType.Snack.GetDescription());

        public static IReadOnlyList<MealOption> All { get; } =
            [Breakfast, Lunch, Dinner, Snack];

        public override string ToString() => $"{Name}";

    }
}
