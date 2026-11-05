using Google.Cloud.Firestore;

namespace StepUp.Contracts.Dtos
{
    [FirestoreData]
    public class RemoteDayResult
    {
        [FirestoreProperty("date")]
        public DateTime Date { get; set; }

        [FirestoreProperty("kcal_spent")]
        public int KcalSpent { get; set; }

        [FirestoreProperty("kcal_eaten")]
        public int KcalEaten { get; set; }

        [FirestoreProperty("kcal_target")]
        public int KcalTarget { get; set; } 

        [FirestoreProperty("weight")]
        public double Weight { get; set; }

        [FirestoreProperty("breakfast")]
        public RemoteMeal Breakfast { get; set; } = new();

        [FirestoreProperty("lunch")]
        public RemoteMeal Lunch { get; set; } = new();

        [FirestoreProperty("dinner")]
        public RemoteMeal Dinner { get; set; } = new();

        [FirestoreProperty("snack")]
        public RemoteMeal Snack { get; set; } = new();

        [FirestoreProperty("sports")]
        public Dictionary<string, object> Sports { get; set; } = new();
    }
}