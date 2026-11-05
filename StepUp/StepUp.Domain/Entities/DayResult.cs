using StepUp.Domain.Enums;

namespace StepUp.Domain.Entities
{
    public class DayResult
    {
        private readonly List<Sport> _sports = [];
        public IReadOnlyCollection<Sport> Sports => _sports;
        private readonly List<Meal> _meals = [];
        public IReadOnlyCollection<Meal> Meals => _meals;

        public DayResult(Guid userId, DateTime date, int caloriePlan, double? weight = null)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Date = date.Date;
            CaloriePlan = caloriePlan;
            Weight = weight;
            _meals.Add(new Meal(Id, Enums.MealType.Breakfast));
            _meals.Add(new Meal(Id, Enums.MealType.Lunch));
            _meals.Add(new Meal(Id, Enums.MealType.Dinner));
            _meals.Add(new Meal(Id, Enums.MealType.Snack));
        }

        public Guid Id { get; }
        public Guid UserId { get;  }
        public DateTime Date { get; set; }
        public int CaloriePlan { get; set; }

        public int KcalBurnt { get => Sports.Sum(i => i.BurntKcals); }
        public int KcalEaten { get => Breakfast.TotalKcal + Lunch.TotalKcal + Dinner.TotalKcal + Snack.TotalKcal; }
        public int KcalRes { get => CaloriePlan - KcalEaten + KcalBurnt; }
        public double? Weight { get; set; }

        public Meal Breakfast => _meals.First(m => m.MealType == MealType.Breakfast);
        public Meal Lunch => _meals.First(m => m.MealType == MealType.Lunch);
        public Meal Dinner => _meals.First(m => m.MealType == MealType.Dinner);
        public Meal Snack => _meals.First(m => m.MealType == MealType.Snack);

        public void AddSport(string name, int kcal)
        {
            _sports.Add(new Sport(Id, name, kcal));
        }

        public void RemoveSport(Guid sportId)
        {
            _sports.RemoveAll(s => s.Id == sportId);
        }

        public void AddToMeal(FoodItem foodItem, MealType mealType)
        {
            _meals.First(m => m.MealType == mealType).AddItem(foodItem);
        }
        public void ClearMeal(MealType meal)
        {
            _meals.First(m => m.MealType == meal).Clear();
        }

    }

}
