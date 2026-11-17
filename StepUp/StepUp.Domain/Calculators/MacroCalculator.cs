using StepUp.Domain.Entities;

namespace StepUp.Domain.Calculators;

public static class MacroCalculator
{
    public static double CalculateProteinCarbonGrams(int caloriePlan, int proteinPercent)
    {
        if (caloriePlan <= 0 || proteinPercent <= 0) return 0;

        double caloriesFromProtein = caloriePlan * (proteinPercent / 100.0);
        return Math.Round(caloriesFromProtein / 4.0, 1);
    }

    public static double CalculateFatGrams(int caloriePlan, int fatPercent)
    {
        if (caloriePlan <= 0 || fatPercent <= 0) return 0;

        double caloriesFromFat = caloriePlan * (fatPercent / 100.0);
        return Math.Round(caloriesFromFat / 9.0, 1);
    }

    public static (double Protein, double Fat, double Carbon) CalculateMacroPercentages(
        double proteins, double fats, double carbohydrates)
    {
        double proteinKcal = proteins * 4.0;
        double fatKcal = fats * 9.0;
        double carbonKcal = carbohydrates * 4.0;

        double totalKcal = proteinKcal + fatKcal + carbonKcal;

        if (totalKcal <= 0)
            return (0, 0, 0);

        double proteinPct = Math.Round(proteinKcal / totalKcal * 100, 1);
        double fatPct = Math.Round(fatKcal / totalKcal * 100, 1);

        double carbonPct = Math.Round(100.0 - proteinPct - fatPct, 1);

        return (proteinPct, fatPct, carbonPct);
    }

    public static (double Protein, double Fat, double Carbon) CalculateMacroPercentages(Nutrients nutrients)
        => CalculateMacroPercentages(nutrients.Proteins, nutrients.Fat, nutrients.Carbohydrates);

}