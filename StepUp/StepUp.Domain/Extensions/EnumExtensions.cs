using System.ComponentModel;
using System.Reflection;

namespace StepUp.Domain.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType()?.GetField(value.ToString());
            if (field == null) return value.ToString();
            DescriptionAttribute? attribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

            return attribute?.Description ?? value.ToString();
        }
    }
}
