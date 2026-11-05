namespace StepUp.Domain.Entities
{
    public class Nutrients(double calories, double proteins, double fats, double carbohydrates)
    {
        public double Calories { get; } = calories >= 0 ? calories : 0;
        public double Proteins { get; } = proteins >= 0 ? proteins : 0;
        public double Fats { get; } = fats >= 0 ? fats : 0;
        public double Carbohydrates { get; } = carbohydrates >= 0 ? carbohydrates : 0;
        public static Nutrients operator *(Nutrients a, double b)
        {
            return new Nutrients(a.Calories * b, a.Proteins * b, a.Fats * b, a.Carbohydrates * b);
        }
        public Nutrients CalculatePerAmount(double weightInGrams)
        {
            if (weightInGrams < 0)
                throw new ArgumentException("Вес порции не может быть отрицательным.", nameof(weightInGrams));

            double factor = weightInGrams / 100.0;

            return this * factor;
        }
    }   
}
