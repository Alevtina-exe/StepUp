using StepUp.Domain.Enums;

namespace StepUp.Domain.Entities;

public class Meal
{
    private Meal() { }
    public Meal(Guid dayResultId, MealType mealType)
    {
        Id = Guid.NewGuid();
        DayResultId = dayResultId;
        MealType = mealType; 
    }
    public Guid Id { get; }
    public Guid DayResultId { get; }

    private readonly List<FoodItem> _items = [];
    public IReadOnlyCollection<FoodItem> Items => _items;

    public MealType MealType { get; }

    public int TotalKcal => (int)Math.Round(Items.Sum(i => i.TotalNutrients.Calories), 0);
    public double TotalProtein => Math.Round(Items.Sum(i => i.TotalNutrients.Proteins), 1);
    public double TotalFat => Math.Round(Items.Sum(i => i.TotalNutrients.Fat), 1);
    public double TotalCarbon => Math.Round(Items.Sum(i => i.TotalNutrients.Carbohydrates), 1);

    public void AddItem(FoodItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        int index = _items.FindIndex(i => i.Id == item.Id);
        if (index >= 0)
        {
            _items[index].AddToAmount(item.Amount);
        }
        else _items.Add(item);
    }

    public void RemoveItem(Guid itemId)
    {
        _items.RemoveAll(i => i.Id == itemId);
    }

    public void Clear()
    {
        _items.Clear();
    }
}