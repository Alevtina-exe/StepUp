namespace StepUp.Application.Interfaces.Repositories
{
    public interface IDayResultRepository
    {
        Task<DayResult?> GetAsync(Guid userId, DateTime date, CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid userId, DateTime date, CancellationToken ct = default);

        Task AddAsync(DayResult dayRes, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
        Task UpdateAsync(DayResult dayRes, CancellationToken ct = default);
    }
}
