using Google.Cloud.Firestore;
using StepUp.Domain.Entities;

namespace StepUp.Contracts.Dtos;

[FirestoreData]
public class RemoteMeal
{
    [FirestoreProperty("items")]
    public List<RemoteFoodItem> Items { get; set; } = new();

    public RemoteMeal() { }

    public RemoteMeal(Meal meal)
    {
        Items = meal.Items.Select(i => new RemoteFoodItem(i)).ToList();
    }

    public Meal ToDomain()
    {
        var meal = new Meal();
        foreach (var item in Items)
            meal.AddItem(item.ToDomain());
        return meal;
    }
}