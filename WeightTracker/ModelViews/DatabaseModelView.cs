using CommunityToolkit.Mvvm.Input;
using Google.Cloud.Firestore;
using Google.Type;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeightTracker.Models;
using WeightTracker.Services;
using DateTime = System.DateTime;

namespace WeightTracker.ModelViews
{
    internal partial class DatabaseModelView
    {
        static FirestoreService fS;
        public DatabaseModelView(FirestoreService ffirestoreService)
        {
            fS = ffirestoreService;
        }
        [RelayCommand]
        public async Task SaveUserData(UserModel user)
        {
            await fS.InsertUserModel(user);
        }
        public async Task<bool> IsInUserDatabase(string username)
        {
            return await fS.IsUserExists(username);
        }
        public async Task<UserModel?> ReturnUser(string username)
        {
            return await fS.GetUserByLogin(username);
        }
        public async Task DeleteUser(string username)
        {
            await fS.DeleteUser(username);
        }
        public async Task UpdateField(string login, string field, object newValue)
        {
            await fS.UpdateUserField("Users", login, field, newValue);
        }
        public async Task StartWorkWithDay(DateTime now)
        {
            string username = UserModel.MainUser.Username;
            if (!await fS.DayResultIsCreated(username, now))
            {
                DayResult.CurrentDay = new DayResult(now, UserModel.MainUser.CaloriePlan); 
                await fS.CreateDayResult(username, DayResult.CurrentDay);
            }
            else
            {
                DayResult.CurrentDay = await fS.ReturnDayResult(username, now);
                if (DayResult.CurrentDay != null)
                {
                    DayResult.CurrentDay.Date = now;
                }
            }
        }
        public async Task AddSport(string sportName, int kcalSpent, string? oldSportName = null, int? oldKcalSpent = null)
        {
            if (DayResult.CurrentDay != null)
            {
                if (oldSportName != null)
                {
                    await fS.DeleteObjectField(UserModel.MainUser.Username,
                        DTWork.DateId(DayResult.CurrentDay), $"Sports.{oldSportName}");
                }
                await fS.AddSport(UserModel.MainUser.Username, DayResult.CurrentDay, sportName, kcalSpent);
                
            }
        }
        public async Task DeleteSport(string name)
        {
            await fS.DeleteObjectField(UserModel.MainUser.Username, DTWork.DateId(DayResult.CurrentDay), $"Sports.{name}");
        }
        public async Task<List<(string Key, object Value)>?> ReturnSports()
        {
            if (DayResult.CurrentDay != null)
                return await fS.ReturnSports(UserModel.MainUser.Username, DayResult.CurrentDay);
            else
                return null;
        }
        public async Task RefreshKcals()
        {
            if(DayResult.CurrentDay != null) 
                await fS.RefreshKcals(UserModel.MainUser.Username, DayResult.CurrentDay);
        }
        public async Task AddDayWeight(double weight)
        {
            if(DayResult.CurrentDay != null)
            {
                await fS.AddDayWeight(UserModel.MainUser, DayResult.CurrentDay, weight);
            }
        }
        public async Task<Dictionary<string, float>> ReturnDayWeightInfo(bool isYear)
        {
            var dict = await fS.ReturnDateWeightInfo(UserModel.MainUser.Username);
            DateTime date = DTWork.FromId(UserModel.MainUser.RegistrationDate);
            while(date.Date <= DateTime.Now.Date)
            {
                if(!dict.ContainsKey(DTWork.DateId(date)))
                {
                    dict.Add(DTWork.DateId(date), dict[DTWork.DateId(date.AddDays(-1))]);
                }
                date = date.AddDays(1);
            }
            if(!isYear) return dict;
            Dictionary<string, float> yearDict = new Dictionary<string, float>();
            date = DTWork.FromId(UserModel.MainUser.RegistrationDate);
            int month = date.Month;
            while (true)
            {
                float averageWeight = 0;
                int count = 0;
                while (month == date.Month && date.Date <= DateTime.Now.Date)
                {
                    count++;
                    averageWeight += dict[DTWork.DateId(date)];
                    date = date.AddDays(1);
                }
                yearDict.Add(date.ToString("MMM, yyyy"), MathF.Round(averageWeight / count, 1));
                if (date.Date.AddDays(-1) == DateTime.Now.Date)
                {
                    return yearDict;
                }
            }
            
        }
        public async Task AddMeal(string meal, FoodProduct product, double amount = 0, string serving = "")
        {
            if(amount == 0)
            {
                await fS.AddMeal(UserModel.MainUser.Username, DayResult.CurrentDay, meal, product, FieldValue.Delete);
            }
            else {
                await fS.AddMeal(UserModel.MainUser.Username, DayResult.CurrentDay, meal, product, $"{amount}-{serving}");
            }
        }

    }
}

