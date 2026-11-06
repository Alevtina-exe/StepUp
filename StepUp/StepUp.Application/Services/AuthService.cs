using StepUp.Application.Interfaces.Repositories;
using StepUp.Application.Interfaces.Services;
using StepUp.Domain.Enums;

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

        public async Task<User> RegisterAsync(
            string fullName, string email, string password,
            DateTime birthDate, Gender gender, double weight, double height,
            int caloriePlan, int proteinPercent, int fatPercent, int carbonPercent,
            SportLevel sportLevel, CancellationToken ct = default)
        {
            var passwordErrors = PasswordPolicy.Validate(password);
            if (passwordErrors.Count != 0)
                throw new InvalidOperationException(string.Join("\n", passwordErrors));
            if (await _userRep.ExistsByEmailAsync(email, ct))
                throw new InvalidOperationException("Пользователь с таким email уже существует.");
            var user = new User(
                fullName: fullName,
                email: email,
                password: _passwordHasher.Hash(password),
                birthDate: birthDate.Date,
                gender: gender,
                weight: weight,
                height: height,
                caloriePlan: caloriePlan,
                carbonPercent: carbonPercent,
                proteinPercent: proteinPercent,
                fatPercent: fatPercent,
                sportLevel: sportLevel,
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

        public void Logout()
        {
            _userProvider.Clear();
        }

        public async Task ChangePasswordAsync(string oldPassword, string newPassword, CancellationToken ct = default)
        {
            var passwordErrors = PasswordPolicy.Validate(newPassword);
            if (passwordErrors.Any())
                throw new InvalidOperationException(string.Join("\n", passwordErrors));
            var user = await _userRep.GetByIdAsync(_userProvider.UserId, ct) ?? throw new InvalidOperationException("Пользователя не существует.");
            if (!_passwordHasher.Verify(oldPassword, user.Password)) throw new InvalidOperationException("Пароли не совпадают.");
            user.Password = _passwordHasher.Hash(newPassword);
            await _userRep.UpdateAsync(user, ct);
        }
    }
}
