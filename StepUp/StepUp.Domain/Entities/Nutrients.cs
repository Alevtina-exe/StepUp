namespace StepUp.Domain.Entities
{
    public class Nutrients
    {
        private Nutrients() { }
        public Nutrients(double calories, double proteins, double fat, double carbohydrates)
        {
            Calories = calories >= 0 ? calories : 0;
            Proteins = proteins >= 0 ? proteins : 0;
            Fat = fat >= 0 ? fat : 0;
            Carbohydrates = carbohydrates >= 0 ? carbohydrates : 0;
        }
        public double Calories { get; } 
        public double Proteins { get; } 
        public double Fat { get; }
        public double Carbohydrates { get; } 
        public static Nutrients operator *(Nutrients a, double b)
        {
            return new Nutrients(a.Calories * b, a.Proteins * b, a.Fat * b, a.Carbohydrates * b);
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
