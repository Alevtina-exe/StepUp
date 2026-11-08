using System.ComponentModel;

namespace StepUp.Domain.Enums;

public enum BmiCategory
{
    [Description("Дефицит веса")]
    Deficit,

    [Description("Нормальный вес")]
    Normal,

    [Description("Избыточный вес")]
    Overweight,

    [Description("Ожирение 1 степени")]
    Obese1,

    [Description("Ожирение 2 степени")]
    Obese2,

    [Description("Ожирение 3 степени")]
    Obese3
}