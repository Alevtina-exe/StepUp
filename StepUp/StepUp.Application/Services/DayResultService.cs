using StepUp.Application.Exceptions;
using StepUp.Application.Interfaces.Repositories;
using StepUp.Application.Interfaces.Services;
using StepUp.Domain.Enums;

namespace StepUp.Application.Services;

public class DayResultService(IDayResultRepository dayResults,
                        ICurrentUserProvider currentUser,
                        IUserRepository users,
                        IClock clock)
{
    private readonly IDayResultRepository _dayResults = dayResults;
    private readonly IUserRepository _users = users;
    private readonly ICurrentUserProvider _currentUser = currentUser;
    private readonly IClock _clock = clock;

    private async Task<DayResult> GetOrCreateAsync(DateTime date, CancellationToken ct = default)
    {
        var id = _currentUser.RequireUserId();
        var user = await _users.GetByIdAsync(id, ct)
            ?? throw new UserNotFoundException(); 

        var todayResult = await _dayResults.GetAsync(id, date, ct);
        if (todayResult != null) return todayResult;

        todayResult = new DayResult(
            userId: id,
            caloriePlan: user.CaloriePlan,
            date: date,
            weight: user.Weight);

        await _dayResults.AddAsync(todayResult, ct);
        return todayResult;
    }

    public async Task<DayResult> GetOrCreateTodayAsync(CancellationToken ct = default)
    {
        return await GetOrCreateAsync(_clock.Today, ct);
    }
    public async Task<DayResult> GetOrCreateDayAsync(DateTime date, CancellationToken ct = default)
    {
        return await GetOrCreateAsync(date, ct);
    }
    public async Task<DayResult?> GetDayAsync(DateTime date, CancellationToken ct = default)
    {
        return await _dayResults.GetAsync(_currentUser.RequireUserId(), date, ct);
    }

    public async Task AddFoodAsync(DateTime date, MealType mealType, FoodProduct product, Serving serving,
                                   double amount, CancellationToken ct = default)
    {
        var day = await GetDayAsync(date, ct)
            ?? throw new DayResultNotFoundException();

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        day.AddToMeal(mealType, product, amount, serving);
        await _dayResults.UpdateAsync(day, ct);
    }

    public async Task RemoveFoodAsync(DateTime date, MealType mealType,
        Guid foodItemId, CancellationToken ct = default)
    {
        var day = await GetDayAsync(date, ct)
            ?? throw new DayResultNotFoundException(); 

        day.RemoveFromMeal(mealType, foodItemId);
        await _dayResults.UpdateAsync(day, ct);
    }

    public async Task ClearMealAsync(DateTime date, MealType mealType, CancellationToken ct = default)
    {
        var day = await GetDayAsync(date, ct)
            ?? throw new DayResultNotFoundException();

        day.ClearMeal(mealType);
        await _dayResults.UpdateAsync(day, ct);
    }

    public async Task ChangeFoodAmountAsync(DateTime date, MealType mealType, Guid itemId,
        double newAmount, Serving? newServing = null, CancellationToken ct = default)
    {
        var day = await GetDayAsync(date, ct)
            ?? throw new DayResultNotFoundException();

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(newAmount);

        var item = day.GetItemFromMeal(mealType, itemId)
            ?? throw new FoodItemNotFoundException();

        if (newServing != null) item.ChangeServing(newServing);
        item.ChangeAmount(newAmount);

        await _dayResults.UpdateAsync(day, ct);
    }

    public async Task AddSportAsync(DateTime date, string name, int kcalBurnt, CancellationToken ct = default)
    {
        var day = await GetDayAsync(date, ct)
            ?? throw new DayResultNotFoundException(); 

        day.AddSport(name, kcalBurnt);
        await _dayResults.UpdateAsync(day, ct);
    }

    public async Task RemoveSportAsync(DateTime date, Guid sportId, CancellationToken ct = default)
    {
        var day = await GetDayAsync(date, ct)
            ?? throw new DayResultNotFoundException(); 

        day.RemoveSport(sportId);
        await _dayResults.UpdateAsync(day, ct);
    }

    public async Task UpdateWeightAsync(double newWeight, CancellationToken ct = default)
    {
        var id = _currentUser.RequireUserId();
        var user = await _users.GetByIdAsync(id, ct)
            ?? throw new UserNotFoundException();

        var todayResult = await GetOrCreateTodayAsync(ct);

        user.ChangeWeight(newWeight);
        await _users.UpdateAsync(user, ct);

        todayResult.ChangeWeight(newWeight);
        await _dayResults.UpdateAsync(todayResult, ct);
    }

    public async Task UpdateCaloriePlanAsync(int newCaloriePlan, CancellationToken ct = default)
    {
        var id = _currentUser.RequireUserId();
        var user = await _users.GetByIdAsync(id, ct)
            ?? throw new UserNotFoundException(); 

        var todayResult = await GetOrCreateTodayAsync(ct);

        user.ChangeCaloriePlan(newCaloriePlan);
        await _users.UpdateAsync(user, ct);

        todayResult.ChangeCaloriePlan(newCaloriePlan);
        await _dayResults.UpdateAsync(todayResult, ct);
    }
}