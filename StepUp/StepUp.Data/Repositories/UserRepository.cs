using Microsoft.EntityFrameworkCore;
using StepUp.Application.Interfaces.Repositories;

namespace StepUp.Data.Repositories
{
    public class UserRepository(AppDbContext db) : IUserRepository
    {
        private readonly AppDbContext _db = db;
        public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
            _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
        public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default) =>
            _db.Users.AnyAsync(u => u.Email == email, ct);

        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);
        }
           
        public Task DeleteAsync(Guid id, CancellationToken ct = default) =>
            _db.Users.Where(u => u.Id == id).ExecuteDeleteAsync(ct);

        public async Task UpdateAsync(User user, CancellationToken ct = default)
        {
            var existing = await _db.Users.FirstOrDefaultAsync(u => u.Id == user.Id, ct)
                ?? throw new InvalidOperationException($"Пользователь {user.Id} не найден.");

            _db.Entry(existing).CurrentValues.SetValues(user);
            await _db.SaveChangesAsync(ct);
        }
    }
}
