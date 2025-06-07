using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace WeightTracker.Models
{
    internal class UserCache : ObservableObject
    {
        private static readonly Lazy<UserCache> _instance = new Lazy<UserCache>(() => new UserCache());

        public static UserCache Instance => _instance.Value;

        private UserCache()
        {
        }
        public List<ObservableCollection<FoodProduct>> MealProducts { get; set; } = new List<ObservableCollection<FoodProduct>>(5);

    }
}
