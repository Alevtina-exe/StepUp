using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeightTracker.Models
{
    [FirestoreData]
    public class UserModel
{
        public static UserModel MainUser {  get; set; }
        public int Age { get; set; }
        [FirestoreProperty]
        public string FullName { get; set; }
        [FirestoreProperty]
        public string Username { get; set; }
        [FirestoreProperty]
        public string PasswordHash { get; set; } 
        [FirestoreProperty]
        public int Day {  get; set; }
        [FirestoreProperty]
        public int Month { get; set; }
        [FirestoreProperty] 
        public int Year { get; set; }
        [FirestoreProperty]
        public string Gender { get; set; } 
        [FirestoreProperty]
        public double Weight { get; set; } 
        [FirestoreProperty]
        public int Height { get; set; } 
        [FirestoreProperty]
        public int CaloriePlan { get; set; }
        [FirestoreProperty]
        public int CarbonPercent { get; set; }
        [FirestoreProperty]
        public int ProteinPercent { get; set; }
        [FirestoreProperty]
        public int FatPercent { get; set; }
        [FirestoreProperty]
        public string RegistrationDate { get; set; }
    }
    public class DateTimeToTimestampConverter : IFirestoreConverter<DateTime>
    {
        public object ToFirestore(DateTime value) => Timestamp.FromDateTime(value.ToUniversalTime());

        public DateTime FromFirestore(object value)
        {
            if (value is Timestamp timestamp)
            {
                return timestamp.ToDateTime();
            }
            throw new ArgumentException("Invalid value");
        }
    }
}
