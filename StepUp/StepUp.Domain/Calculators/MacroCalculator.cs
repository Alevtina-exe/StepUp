namespace StepUp.Domain.Calculators;

public static class MacroCalculator
{
    public static double CalculateProteinGrams(int caloriePlan, int proteinPercent)
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

    public static double CalculateCarbonGrams(int caloriePlan, int carbonPercent)
    {
        if (caloriePlan <= 0 || carbonPercent <= 0) return 0;

        double caloriesFromCarbon = caloriePlan * (carbonPercent / 100.0);
        return Math.Round(caloriesFromCarbon / 4.0, 1);
    }
}