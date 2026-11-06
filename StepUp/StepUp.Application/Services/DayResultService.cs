using StepUp.Application.Interfaces.Repositories;
using StepUp.Application.Interfaces.Services;
using StepUp.Domain.Enums;

namespace StepUp.Application.Services
{
    public class DayResultService(IDayResultRepository dayResults,
                            ICurrentUserProvider currentUser,
                            IUserRepository users,
                            IClock clock)
    {
        private readonly IDayResultRepository _dayResults = dayResults;
        private readonly IUserRepository _users = users;
        private readonly ICurrentUserProvider _currentUser = currentUser;
        private readonly IClock _clock = clock;

        public async Task<DayResult> GetOrCreateTodayAsync(CancellationToken ct = default)
        {
            var id = _currentUser.RequireUserId();
            var user = await _users.GetByIdAsync(id, ct) ?? 
                throw new InvalidOperationException("Пользователя не существует.");
            var todayResult = await _dayResults.GetAsync(id, _clock.Today, ct);
            if(todayResult != null) return todayResult;

            todayResult = new DayResult(
                userId: id,
                caloriePlan: user.CaloriePlan,
                date: _clock.Today,
                weight: user.Weight
                );
            await _dayResults.AddAsync(todayResult, ct);
            return todayResult;
        }

        public async Task<DayResult?> GetDayAsync(DateTime date, CancellationToken ct = default)
        {
            return await _dayResults.GetAsync(_currentUser.RequireUserId(), date, ct);
        }


        public async Task AddFoodAsync(DateTime date, MealType mealType, FoodProduct product, Serving serving,
                                       double amount, CancellationToken ct = default)
        {
            var day = await GetDayAsync(date, ct) ?? throw new InvalidOperationException("День не найден.");
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
            day.AddToMeal(mealType, product, amount, serving);
            await _dayResults.UpdateAsync(day, ct);
        }

        public async Task RemoveFoodAsync(DateTime date, MealType mealType,
            Guid foodItemId, CancellationToken ct = default)
        {
            var day = await GetDayAsync(date, ct) ?? throw new InvalidOperationException("День не найден.");
            day.RemoveFromMeal(mealType, foodItemId);
            await _dayResults.UpdateAsync(day, ct);
        }

        public async Task ClearMealAsync(DateTime date, MealType mealType, CancellationToken ct = default)
        {
            var day = await GetDayAsync(date, ct) ?? throw new InvalidOperationException("День не найден.");
            day.ClearMeal(mealType);
            await _dayResults.UpdateAsync(day, ct);
        }

        public async Task ChangeFoodAmountAsync(DateTime date, MealType mealType, Guid itemId, 
            double newAmount, Serving? newServing = null, CancellationToken ct = default)
        {
            var day = await GetDayAsync(date, ct) ?? throw new InvalidOperationException("День не найден.");
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(newAmount);
            var item = day.GetItemFromMeal(mealType, itemId) ?? 
                throw new InvalidOperationException("Запись продукта в данном приёме пищи не найдена.");
            item.ChangeAmount(newAmount);
            if(newServing != null) item.ChangeServing(newServing);
            await _dayResults.UpdateAsync(day, ct);
        }


        public async Task AddSportAsync(DateTime date, string name, int kcalBurnt, CancellationToken ct = default)
        {
            var day = await GetDayAsync(date, ct) ?? throw new InvalidOperationException("День не найден.");
            day.AddSport(name, kcalBurnt);
            await _dayResults.UpdateAsync(day, ct);
        }

        public async Task RemoveSportAsync(DateTime date, Guid sportId, CancellationToken ct = default)
        {
            var day = await GetDayAsync(date, ct) ?? throw new InvalidOperationException("День не найден.");
            day.RemoveSport(sportId);
            await _dayResults.UpdateAsync(day, ct);
        }


        public async Task UpdateWeightAsync(double newWeight, CancellationToken ct = default)
        {
            var id = _currentUser.RequireUserId();
            var user = await _users.GetByIdAsync(id, ct) ?? 
                throw new InvalidOperationException("Пользователя не существует.");
            var todayResult = await GetOrCreateTodayAsync(ct);
            user.ChangeWeight(newWeight);
            await _users.UpdateAsync(user, ct);
            todayResult.ChangeWeight(newWeight);
            await _dayResults.UpdateAsync(todayResult, ct);
        }
        public async Task UpdateCaloriePlanAsync(int newCaloriePlan, CancellationToken ct = default)
        {
            var id = _currentUser.RequireUserId();
            var user = await _users.GetByIdAsync(id, ct) ?? 
                throw new InvalidOperationException("Пользователя не существует.");
            var todayResult = await _dayResults.GetAsync(id, _clock.Today, ct) ?? 
                throw new InvalidOperationException("День ещё не записан.");
            user.ChangeCaloriePlan(newCaloriePlan);
            await _users.UpdateAsync(user, ct);
            todayResult.ChangeCaloriePlan(newCaloriePlan);
            await _dayResults.UpdateAsync(todayResult, ct);
        }

    }
}