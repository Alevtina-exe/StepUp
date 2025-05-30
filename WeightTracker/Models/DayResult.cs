using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace WeightTracker.Models
{
    [FirestoreData]
    public class DayResult
    {
        static public DayResult CurrentDay { get; set; }

        public DayResult() { 
            Breakfast = new Dictionary<string, object>();
            Lunch = new Dictionary<string, object>();
            Dinner = new Dictionary<string, object>();
            Snack = new Dictionary<string, object>();
            Sports = new Dictionary<string, object>();
        }
        public DayResult(DateTime date, int kcalRes) 
        { 
            Date = date;
            Weight = UserModel.MainUser.Weight;

            KcalEaten = 0;
            KcalRes = kcalRes;
            KcalSpent = 0;

            Breakfast = new Dictionary<string, object>
            {
                {"Kcal", 0 },
                {"Pr", 0 },
                {"Fat", 0 },
                {"Cb", 0 },
                {"Dishes", new Dictionary<string, object>() }
            };

            Lunch = new Dictionary<string, object>
            {
                {"Kcal", 0 },
                {"Pr", 0 },
                {"Fat", 0 },
                {"Cb", 0 },
                {"Dishes", new Dictionary<string, object>() }
            };

            Dinner = new Dictionary<string, object>
            {
                {"Kcal", 0 },
                {"Pr", 0 },
                {"Fat", 0 },
                {"Cb", 0 },
                {"Dishes", new Dictionary<string, object>() }
            };

            Snack = new Dictionary<string, object>
            {
                {"Kcal", 0 },
                {"Pr", 0 },
                {"Fat", 0 },
                {"Cb", 0 },
                {"Dishes", new Dictionary<string, object>() }
            };

            Sports = new Dictionary<string, object>();
        }
        public DateTime Date { get; set; }
        [FirestoreProperty]
        public int KcalSpent { get; set; }
        [FirestoreProperty]
        public int KcalEaten { get; set; }
        [FirestoreProperty]
        public int KcalRes {  get; set; }

        [FirestoreProperty]
        public Dictionary<string, object> Breakfast { get; set; }
        [FirestoreProperty]
        public Dictionary<string, object> Lunch { get; set; }
        [FirestoreProperty]
        public Dictionary<string, object> Dinner { get; set; }
        [FirestoreProperty]
        public Dictionary<string, object> Snack { get; set; }
        [FirestoreProperty]
        public Dictionary<string, object> Sports { get; set; }
        
        [FirestoreProperty]
        public double Weight { get; set; }

        public int CaloriePlan
        {
            get
            {
                return KcalRes - KcalSpent + KcalEaten;
            }
        }

    }
}
