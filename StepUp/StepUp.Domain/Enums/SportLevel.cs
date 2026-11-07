using System.ComponentModel;

namespace StepUp.Domain.Enums
{
    public enum SportLevel
    {
        [Description("Нет физ. нагрузки")]
        No,

        [Description("Лёгкая физ. нагрузка")]
        Low,

        [Description("Средняя физ. нагрузка")]
        Mid,

        [Description("Ежедневная физ. нагрузка")]
        High,

        [Description("Профессиональный спорт")]
        Extreme
    }
}