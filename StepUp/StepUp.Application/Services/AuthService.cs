using StepUp.Application.Interfaces.Repositories;
using StepUp.Application.Interfaces.Services;
using StepUp.Application.Models;

namespace StepUp.Application.Services
{
    public class AuthService(IUserRepository userRep,
                             IPasswordHasher passwordHasher,
                             ICurrentUserProvider userProvider,
                             IClock clock)
    {
        private readonly IUserRepository _userRep = userRep;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly ICurrentUserProvider _userProvider = userProvider;
        private readonly IClock _clock = clock;

        public async Task<User> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            var passwordErrors = PasswordPolicy.Validate(request.Password);
            if (passwordErrors.Any())
                throw new InvalidOperationException(string.Join("\n", passwordErrors));

            if (await _userRep.ExistsByEmailAsync(request.Email, ct))
                throw new InvalidOperationException("Пользователь с таким email уже существует.");

            var user = new User(
                fullName: request.Name,
                email: request.Email,
                password: _passwordHasher.Hash(request.Password), 
                birthDate: request.BirthDate,
                gender: request.Gender,
                weight: request.Weight,
                height: request.Height,
                caloriePlan: request.CaloriePlan,
                carbonPercent: request.CarbonPercent,
                proteinPercent: request.ProteinPercent,
                fatPercent: request.FatPercent,
                sportLevel: request.SportLevel,
                registrationDate: _clock.UtcNow);

            await _userRep.AddAsync(user, ct);
            _userProvider.SetUser(user.Id);
            return user;
        }

        public async Task<User> LoginAsync(string email, string password, CancellationToken ct = default)
        {
            var user = await _userRep.GetByEmailAsync(email, ct) ?? throw new InvalidOperationException("Пользователя с заданным email не существует.");
            if (_passwordHasher.Verify(password, user.Password))
            {
                _userProvider.SetUser(user.Id);
                return user;
            }
            throw new InvalidOperationException("Неверный пароль");
        }

        public async Task<bool> IsUserExistsAsync(string email, CancellationToken ct = default) =>
            await _userRep.ExistsByEmailAsync(email, ct);

        public void Logout()
        {
            _userProvider.Clear();
        }

        public async Task ChangePasswordAsync(string oldPassword, string newPassword, CancellationToken ct = default)
        {
            var passwordErrors = PasswordPolicy.Validate(newPassword);
            if (passwordErrors.Any())
                throw new InvalidOperationException(string.Join("\n", passwordErrors));
            var user = await _userRep.GetByIdAsync(_userProvider.RequireUserId(), ct) ?? throw new InvalidOperationException("Пользователя не существует.");
            if (!_passwordHasher.Verify(oldPassword, user.Password)) throw new InvalidOperationException("Пароли не совпадают.");
            user.ChangePassword(_passwordHasher.Hash(newPassword));
            await _userRep.UpdateAsync(user, ct);
        }
    }
}
