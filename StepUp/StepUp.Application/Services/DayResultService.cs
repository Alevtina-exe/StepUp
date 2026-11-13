using StepUp.Application.Exceptions;
using StepUp.Application.Interfaces.Repositories;
using StepUp.Application.Interfaces.Services;
using StepUp.Domain.Enums;

namespace StepUp.Application.Services;

public class DayResultService(IDayResultRepository dayResults,
                        ICurrentUserProvider userProvider,
                        ICurrentUserService userService,
                        IUserRepository users,
                        IClock clock)
{

    private async Task<DayResult> GetOrCreateAsync(DateTime date, CancellationToken ct = default)
    {
        var user = await userService.GetOrLoadAsync(ct) ??
            throw new UserNotFoundException();

        var todayResult = await dayResults.GetAsync(user.Id, date, ct);
        if (todayResult != null) return todayResult;

        todayResult = new DayResult(
            userId: user.Id,
            caloriePlan: user.CaloriePlan,
            date: date,
            weight: user.Weight);

        await dayResults.AddAsync(todayResult, ct);
        return todayResult;
    }

    public async Task<DayResult> GetOrCreateTodayAsync(CancellationToken ct = default)
    {
        return await GetOrCreateAsync(clock.Today, ct);
    }
    public async Task<DayResult> GetOrCreateDayAsync(DateTime date, CancellationToken ct = default)
    {
        return await GetOrCreateAsync(date, ct);
    }
    public async Task<DayResult?> GetDayAsync(DateTime date, CancellationToken ct = default)
    {
        return await dayResults.GetAsync(userProvider.RequireUserId(), date, ct);
    }

    public async Task<DayResult> AddFoodAsync(DateTime date, MealType mealType, FoodProduct product, Serving serving,
                                   double amount, CancellationToken ct = default)
    {
        var day = await GetDayAsync(date, ct)
            ?? throw new DayResultNotFoundException();

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        day.AddToMeal(mealType, product, amount, serving);
        await dayResults.UpdateAsync(day, ct);

        return day;
    }

    public async Task<DayResult> RemoveFoodAsync(DateTime date, MealType mealType,
        Guid foodItemId, CancellationToken ct = default)
    {
        var day = await GetDayAsync(date, ct)
            ?? throw new DayResultNotFoundException(); 

        day.RemoveFromMeal(mealType, foodItemId);
        await dayResults.UpdateAsync(day, ct);

        return day;
    }

    public async Task<DayResult> ClearMealAsync(DateTime date, MealType mealType, CancellationToken ct = default)
    {
        var day = await GetDayAsync(date, ct)
            ?? throw new DayResultNotFoundException();

        day.ClearMeal(mealType);
        await dayResults.UpdateAsync(day, ct);

        return day;
    }

    public async Task<DayResult> ChangeFoodAmountAsync(DateTime date, MealType mealType, Guid itemId,
        double newAmount, Serving? newServing = null, CancellationToken ct = default)
    {
        var day = await GetDayAsync(date, ct)
            ?? throw new DayResultNotFoundException();

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(newAmount);

        var item = day.GetItemFromMeal(mealType, itemId)
            ?? throw new FoodItemNotFoundException();

        if (newServing != null) item.ChangeServing(newServing);
        item.ChangeAmount(newAmount);

        await dayResults.UpdateAsync(day, ct);

        return day;
    }

    public async Task<DayResult> AddSportAsync(DateTime date, string name, int kcalBurnt, CancellationToken ct = default)
    {
        var day = await GetDayAsync(date, ct)
            ?? throw new DayResultNotFoundException(); 

        day.AddSport(name, kcalBurnt);
        await dayResults.UpdateAsync(day, ct);

        return day;
    }

    public async Task<DayResult> RemoveSportAsync(DateTime date, Guid sportId, CancellationToken ct = default)
    {
        var day = await GetDayAsync(date, ct)
            ?? throw new DayResultNotFoundException(); 

        day.RemoveSport(sportId);
        await dayResults.UpdateAsync(day, ct);

        return day;
    }

    public async Task<DayResult> EditSportAsync(DateTime date, Guid sportId,
        string newName, int kcalBurnt, CancellationToken ct = default)
    {
        var day = await GetDayAsync(date, ct)
           ?? throw new DayResultNotFoundException();

        if (day.GetSport(sportId) is null)
            throw new SportNotFoundException();

        day.EditSport(sportId, newName, kcalBurnt);
        await dayResults.UpdateAsync(day, ct);

        return day;
    }

    public async Task<DayResult> UpdateWeightAsync(double newWeight, CancellationToken ct = default)
    {
        var user = await userService.GetOrLoadAsync(ct) ??
            throw new UserNotFoundException();

        var todayResult = await GetOrCreateTodayAsync(ct);

        user.ChangeWeight(newWeight);
        await users.UpdateAsync(user, ct);
        userService.Clear();

        todayResult.ChangeWeight(newWeight);
        await dayResults.UpdateAsync(todayResult, ct);

        return todayResult;
    }

    public async Task<DayResult> UpdateCaloriePlanAsync(int newCaloriePlan, CancellationToken ct = default)
    {
        var user = await userService.GetOrLoadAsync(ct) ??
            throw new UserNotFoundException();

        var todayResult = await GetOrCreateTodayAsync(ct);

        user.ChangeCaloriePlan(newCaloriePlan);
        await users.UpdateAsync(user, ct);
        userService.Clear();

        todayResult.ChangeCaloriePlan(newCaloriePlan);
        await dayResults.UpdateAsync(todayResult, ct);

        return todayResult ;
    }
}