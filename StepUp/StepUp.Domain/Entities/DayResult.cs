using StepUp.Domain.Enums;

namespace StepUp.Domain.Entities
{
    public class DayResult
    {
        private DayResult() { }

        private readonly List<Sport> _sports = [];
        public IReadOnlyCollection<Sport> Sports => _sports;
        private readonly List<Meal> _meals = [];
        public IReadOnlyCollection<Meal> Meals => _meals;

        public DayResult(Guid userId, DateTime date, int caloriePlan, double weight)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Date = date.Date;
            ChangeCaloriePlan(caloriePlan);
            ChangeWeight(weight);
            _meals.Add(new Meal(Id, Enums.MealType.Breakfast));
            _meals.Add(new Meal(Id, Enums.MealType.Lunch));
            _meals.Add(new Meal(Id, Enums.MealType.Dinner));
            _meals.Add(new Meal(Id, Enums.MealType.Snack));
        }

        public Guid Id { get; }
        public Guid UserId { get;  }
        public DateTime Date { get; }
        public int CaloriePlan { get; private set; }
        public double Weight { get; private set; }

        public int KcalBurnt => Sports.Sum(i => i.BurntKcals);
        public int KcalEaten => Breakfast.TotalKcal + Lunch.TotalKcal + Dinner.TotalKcal + Snack.TotalKcal; 

        public double TotalProtein =>
            _meals.Sum(m => m.TotalProtein);
        public double TotalFat =>
            _meals.Sum(m => m.TotalFat);
        public double TotalCarbon =>
            _meals.Sum(m => m.TotalCarbon);

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

        public Sport? GetSport(Guid id) => _sports.FirstOrDefault(s => s.Id == id);

        public void EditSport(Guid sportId, string newName, int newKcal)
        {
            var sport = GetSport(sportId) ??
                throw new InvalidOperationException("Тренировка не найдена."); 

            sport.ChangeName(newName);
            sport.ChangeKcal(newKcal);
        }

        public void AddToMeal(MealType mealType, FoodItem foodItem)
        {
            _meals.First(m => m.MealType == mealType).AddItem(foodItem);
        }
        public void AddToMeal(MealType mealType, FoodProduct product, 
            double amount = 1, Serving? serving = null)
        {
            var meal = _meals.First(m => m.MealType == mealType);
            meal.AddItem(new FoodItem(product: product, mealId: meal.Id, amount: amount, serving: serving));
        }
        public void RemoveFromMeal(MealType meal, Guid itemId)
        {
            _meals.First(m => m.MealType == meal).RemoveItem(itemId);
        }
        public void ClearMeal(MealType meal)
        {
            _meals.First(m => m.MealType == meal).Clear();
        }
        public FoodItem? GetItemFromMeal(MealType mealType, Guid itemId)
        {
            return _meals.First(m => m.MealType == mealType)
                         .Items.FirstOrDefault(i => i.Id == itemId);
        }


        public void ChangeWeight(double newWeight)
        {
            if (newWeight < 30 || newWeight > 300)
                throw new ArgumentOutOfRangeException(
                    nameof(newWeight),
                    "Вес должен быть в диапазоне 30–300 кг.");

            Weight = newWeight;
        }

        public void ChangeCaloriePlan(int newCaloriePlan)
        {
            if (newCaloriePlan < 1300)
                throw new ArgumentOutOfRangeException(nameof(newCaloriePlan),
                    "Дневная норма калорий не может быть меньше 1300.");
            CaloriePlan = newCaloriePlan;
        }
    }

}
