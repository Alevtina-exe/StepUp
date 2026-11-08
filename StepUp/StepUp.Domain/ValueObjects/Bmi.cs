using StepUp.Domain.Enums;

namespace StepUp.Domain.ValueObjects;

public record Bmi
{
    public double Value { get; }
    public BmiCategory Category { get; }

    private Bmi(double value, BmiCategory category)
    {
        Value = value;
        Category = category;
    }

    public static Bmi Calculate(double weightKg, double heightCm)
    {
        if (heightCm <= 0)
            throw new ArgumentException("Рост должен быть больше нуля.", nameof(heightCm));

        double heightM = heightCm / 100.0;
        double value = Math.Round(weightKg / (heightM * heightM), 1);

        var category = value switch
        {
            < 18.9 => BmiCategory.Deficit,
            < 24.9 => BmiCategory.Normal,
            < 29.9 => BmiCategory.Overweight,
            < 34.9 => BmiCategory.Obese1,
            < 39.9 => BmiCategory.Obese2,
            _ => BmiCategory.Obese3
        };

        return new Bmi(value, category);
    }
}