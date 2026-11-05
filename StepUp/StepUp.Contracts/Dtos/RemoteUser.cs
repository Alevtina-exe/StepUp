using Google.Cloud.Firestore;
using StepUp.Domain.Entities;
using StepUp.Domain.Enums;

namespace StepUp.Contracts.Dtos
{
    [FirestoreData]
    public class RemoteUser
    {
        //public static UserModel MainUser { get; set; }
        [FirestoreProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [FirestoreProperty("full_name")]
        public string FullName { get; set; } = null!;
        [FirestoreProperty("username")]
        public string Username { get; set; } = null!;
        //[FirestoreProperty]
        //public string PasswordHash { get; set; }
        [FirestoreProperty("birth_date")]
        public DateTime BirthDate { get; set; }
        [FirestoreProperty("gender")]
        public string Gender { get; set; } = null!;
        [FirestoreProperty("weight")]
        public double Weight { get; set; }
        [FirestoreProperty("height")]
        public double Height { get; set; }
        [FirestoreProperty("calorie_plan")]
        public int CaloriePlan { get; set; }
        [FirestoreProperty("carbon_percent")]
        public int CarbonPercent { get; set; }
        [FirestoreProperty("protein_percent")]
        public int ProteinPercent { get; set; }
        [FirestoreProperty("fat_percent")]
        public int FatPercent { get; set; }
        [FirestoreProperty]
        public DateTime AddedAt { get; set; }
        //[FirestoreProperty]
        //public Dictionary<string, object> FavDishes { get; set; } = new Dictionary<string, object>();

        public RemoteUser(User user)
        {
            Id = user.Id;
            FullName = user.FullName;
            Username = user.Username;
            BirthDate = user.BirthDate;
            Gender = user.Gender.ToString(); 
            Weight = user.Weight;
            Height = user.Height;       
            CaloriePlan = user.CaloriePlan;
            CarbonPercent = user.CarbonPercent;
            ProteinPercent = user.ProteinPercent;
            FatPercent = user.FatPercent;
            AddedAt = user.RegistrationDate;
        }
    }

}

