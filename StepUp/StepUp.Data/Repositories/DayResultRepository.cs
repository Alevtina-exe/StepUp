using Microsoft.EntityFrameworkCore;
using StepUp.Application.Dtos;
using StepUp.Application.Interfaces.Repositories;

namespace StepUp.Data.Repositories;

public class DayResultRepository(AppDbContext db) : IDayResultRepository
{
    private readonly AppDbContext _db = db;

    public Task<DayResult?> GetAsync(Guid userId, DateTime date, CancellationToken ct = default) =>
        _db.DayResults
            .Include(d => d.Meals).ThenInclude(m => m.Items)
            .Include(d => d.Sports)
            .AsSplitQuery()
            .FirstOrDefaultAsync(d => d.UserId == userId && d.Date == date.Date, ct);

    public Task<bool> ExistsAsync(Guid userId, DateTime date, CancellationToken ct = default) =>
        _db.DayResults.AnyAsync(d => d.UserId == userId && d.Date == date.Date, ct);

    public async Task AddAsync(DayResult day, CancellationToken ct = default)
    {
        day.RecalculateCache();
        _db.DayResults.Add(day);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(DayResult day, CancellationToken ct = default)
    {
        day.RecalculateCache();
        if (_db.Entry(day).State == EntityState.Detached)
            _db.DayResults.Update(day);

        await _db.SaveChangesAsync(ct);
    }

    public Task DeleteAsync(Guid dayId, CancellationToken ct = default) =>
        _db.DayResults
            .Where(d => d.Id == dayId)
            .ExecuteDeleteAsync(ct);

    public Task<DayResult?> GetLastCreatedInRangeAsync(Guid userId, 
        DateTime from, DateTime to, CancellationToken ct = default) =>
         _db.DayResults
            .Where(d => d.UserId == userId && d.Date >= from.Date && d.Date <= to.Date)
            .OrderByDescending(d => d.Date)
            .FirstOrDefaultAsync(ct);

    public Task<List<DayResultChartDto>> GetRangeForChartAsync(Guid userId, 
        DateTime from, DateTime to, CancellationToken ct = default) =>
        _db.DayResults
            .AsNoTracking()
            .Where(d => d.UserId == userId && d.Date >= from.Date && d.Date <= to.Date)
            .OrderBy(d => d.Date)
            .Select(d => new DayResultChartDto(d.Date, d.Weight, d.KcalEatenCache, d.KcalBurntCache))
            .ToListAsync(ct);
}