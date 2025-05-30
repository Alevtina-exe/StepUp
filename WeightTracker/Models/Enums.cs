using System.ComponentModel;
using System.Reflection;

namespace WeightTracker.Models;
public enum Sport { No, Low, Mid, High, Extreme};

public enum Meal {
    [Description("Завтрак")]
    Breakfast,
    [Description("Обед")]
    Lunch,
    [Description("Ужин")]
    Dinner,
    [Description("Перекус")]
    Snack};
public enum Portion { g100, g1, serving}

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());
        DescriptionAttribute attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

        return attribute?.Description ?? value.ToString();
    }
}