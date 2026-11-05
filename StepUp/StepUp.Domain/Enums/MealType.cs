using System.ComponentModel;

namespace StepUp.Domain.Enums
{
    public enum MealType
    {
        [Description("Завтрак")]
        Breakfast,

        [Description("Обед")]
        Lunch,

        [Description("Ужин")]
        Dinner,

        [Description("Перекус")]
        Snack
    }
}
