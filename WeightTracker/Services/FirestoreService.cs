using WeightTracker.Models;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeightTracker.Services
{
    public class FirestoreService
    {
        private FirestoreDb db;
        private async Task SetupFirestore()
        {
            if (db == null)
            {
                var stream = await FileSystem.OpenAppPackageFileAsync("admin-sdk.json");
                var reader = new StreamReader(stream);
                var contents = reader.ReadToEnd();

                db = new FirestoreDbBuilder
                {
                    ProjectId = "weighttracker-c5ac3",

                    ConverterRegistry = new ConverterRegistry
                    {
                        new DateTimeToTimestampConverter(),
                    },
                    JsonCredentials = contents
                }.Build();
            }
        }
        // Созждание пользователя
        public async Task InsertUserModel(UserModel user)
        {
            await SetupFirestore();
            await db.Collection("Users").Document(user.Username).SetAsync(user);
            await UpdateUserField("Users", user.Username, "RegistrationDate", DTWork.DateId(DateTime.Now));
        }
        // Удаление пользователя
        public async Task DeleteUser(string login)
        {
            await SetupFirestore();
            await db.Collection("Users").Document(login).DeleteAsync();
        }
        // Обновление поля пользователя
        public async Task UpdateUserField(string collection, string document, string field, object newValue)
        {
            await SetupFirestore();
            var userRef = db.Collection(collection).Document(document);
            await userRef.UpdateAsync(new Dictionary<string, object> { { field, newValue } });
        }
        // Проверка существует ли пользователь
        public async Task<bool> IsUserExists(string login)
        {
            await SetupFirestore();
            var query = await db.Collection("Users").Document(login).GetSnapshotAsync();
            return query.Exists;
        }
        // Возвращает пользователя по логину
        public async Task<UserModel?> GetUserByLogin(string login)
        {
            await SetupFirestore();
            var query = db.Collection("Users").Document(login);
            var snapshot = await query.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<UserModel>();
            }
            return null;
        }
        public async Task CreateDayResult(string login, DayResult dayResult)
        {
            await SetupFirestore();
            await db.Collection(login).Document(DTWork.DateId(dayResult)).SetAsync(dayResult);
        }
        public async Task UpdateDayResult(string login, DayResult dayResult)
        {
            await SetupFirestore();
            await db.Collection(login).Document(DTWork.DateId(dayResult)).DeleteAsync();
            await db.Collection(login).Document(DTWork.DateId(dayResult)).SetAsync(dayResult);

        }
        public async Task<bool> DayResultIsCreated(string login, DateTime date)
        {
            await SetupFirestore();
            var db_ref = await db.Collection(login).Document(DTWork.DateId(date)).GetSnapshotAsync();
            return db_ref.Exists;
        }
        public async Task<DayResult?> ReturnDayResult(string login, DateTime date)
        {
            await SetupFirestore();
            var db_ref = await db.Collection(login).Document(DTWork.DateId(date)).GetSnapshotAsync();
            return db_ref.ConvertTo<DayResult?>();
        }
        public async Task AddSport(string login, DayResult day, string sportName, int kcalAmount)
        {
            await SetupFirestore();
            var userRef = db.Collection(login).Document(DTWork.DateId(day));
            if ((await userRef.GetSnapshotAsync()).ContainsField("Sports"))
            {
                var data = new Dictionary<string, object> { { $"Sports.{sportName}", kcalAmount } };
                await userRef.UpdateAsync(data);
            }
        }
        public async Task<List<(string Key, object Value)>> ReturnSports(string login, DayResult dayRes)
        {
            await SetupFirestore();
            var userRef = db.Collection(login).Document(DTWork.DateId(dayRes));
            var snapshot = await userRef.GetSnapshotAsync();

            if (snapshot.Exists && snapshot.ContainsField("Sports"))
            {
                var stats = snapshot.GetValue<Dictionary<string, object>>("Sports");
                var tuples = stats.Select(kv => (kv.Key, kv.Value)).ToList();
                return tuples;
            }
            return null;

        }
        public async Task RefreshKcals(string login, DayResult day)
        {
            await SetupFirestore();
            var userRef = db.Collection(login).Document(DTWork.DateId(day));
            await userRef.UpdateAsync(new Dictionary<string, object> {
                {"KcalRes", day.KcalRes },
                {"KcalSpent", day.KcalSpent },
                {"KcalEaten", day.KcalEaten }
            });
        }
        public async Task AddDayWeight(UserModel user, DayResult day, double weight)
        {
            await SetupFirestore();
            await UpdateUserField("Users", user.Username, "Weight", weight);
            await UpdateUserField(user.Username, DTWork.DateId(day), "Weight", weight);
        }
        public async Task DeleteObjectField(string collection, string document, string field)
        {
            await SetupFirestore();
            var userRef = db.Collection(collection).Document(document);
            var update = new Dictionary<string, object>
            {
                {field, FieldValue.Delete }
            };
            await userRef.UpdateAsync(update);
        }
        public async Task<Dictionary<string, float>> ReturnDateWeightInfo(string login)
        {
            await SetupFirestore();
            Dictionary<String, float> dict = new Dictionary<String, float>();
            var userRef = db.Collection(login);
            var snap = await userRef.GetSnapshotAsync();
            foreach (var doc in snap.Documents)
            {
                if (doc.ContainsField("Weight"))
                {
                    dict[doc.Id] = doc.GetValue<float>("Weight");
                }
            }
            return dict;
        }
        public async Task AddMeal(string login, DayResult day, string meal, FoodProduct product, object amount)
        {
            await SetupFirestore();
            var userRef = db.Collection(login).Document(DTWork.DateId(day));
            Dictionary<string, object> data;
            switch (meal) 
            {
                case "Завтрак":
                    data = new Dictionary<string, object>
                    {
                        { $"Breakfast.Dishes.{product.Barcode}", amount },
                        { "Breakfast.Kcal", day.Breakfast["Kcal"] },
                        { "Breakfast.Pr", day.Breakfast["Pr"] },
                        { "Breakfast.Fat", day.Breakfast["Fat"] },
                        { "Breakfast.Cb", day.Breakfast["Cb"] }
                    };
                    break;
                case "Обед":
                    data = new Dictionary<string, object>
                    {
                        { $"Lunch.Dishes.{product.Barcode}", amount },
                        { "Lunch.Kcal", day.Lunch["Kcal"] },
                        { "Lunch.Pr", day.Lunch["Pr"] },
                        { "Lunch.Fat", day.Lunch["Fat"] },
                        { "Lunch.Cb", day.Lunch["Cb"] }
                    };
                    break;
                case "Ужин":
                    data = new Dictionary<string, object>
                    {
                        { $"Dinner.Dishes.{product.Barcode}", amount },
                        { "Dinner.Kcal", day.Dinner["Kcal"] },
                        { "Dinner.Pr", day.Dinner["Pr"] },
                        { "Dinner.Fat", day.Dinner["Fat"] },
                        { "Dinner.Cb", day.Dinner["Cb"] }
                    };
                    break;
                case "Перекус":
                    data = new Dictionary<string, object>
                    {
                        { $"Snack.Dishes.{product.Barcode}", amount },
                        { "Snack.Kcal", day.Snack["Kcal"] },
                        { "Snack.Pr", day.Snack["Pr"] },
                        { "Snack.Fat", day.Snack["Fat"] },
                        { "Snack.Cb", day.Snack["Cb"] }
                    };
                    break;
                default:
                    data = new Dictionary<string, object>();
                    break;
            }
            data["KcalRes"] = day.KcalRes;
            data["KcalEaten"] = day.KcalEaten;
            data["KcalSpent"] = day.KcalSpent;
            await userRef.UpdateAsync(data);
        }
    }
}