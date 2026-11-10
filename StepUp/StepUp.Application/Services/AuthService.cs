using StepUp.Application.Exceptions;
using StepUp.Application.Interfaces.Repositories;
using StepUp.Application.Interfaces.Services;
using StepUp.Application.Models;

namespace StepUp.Application.Services;

public class AuthService(IUserRepository userRep,
                         IPasswordHasher passwordHasher,
                         ICurrentUserProvider userProvider,
                         ICurrentUserService userService,
                         IClock clock)
{
    private readonly IUserRepository _userRep = userRep;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ICurrentUserProvider _userProvider = userProvider;
    private readonly ICurrentUserService _userService = userService;
    private readonly IClock _clock = clock;

    public async Task<User> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var passwordErrors = PasswordPolicy.Validate(request.Password);
        if (passwordErrors.Any())
            throw new WeakPasswordException(passwordErrors); 

        if (await _userRep.ExistsByEmailAsync(request.Email, ct))
            throw new EmailAlreadyExistsException(request.Email); 

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
        var user = await _userRep.GetByEmailAsync(email, ct)
            ?? throw new UserNotFoundException(email); 

        if (!_passwordHasher.Verify(password, user.Password))
            throw new InvalidPasswordException();

        _userProvider.SetUser(user.Id);
        return user;
    }

    public async Task<bool> IsUserExistsAsync(string email, CancellationToken ct = default) =>
        await _userRep.ExistsByEmailAsync(email, ct);

    public void Logout()
    {
        _userProvider.Clear();
        _userService.Clear();
    }

    public async Task ChangePasswordAsync(string newPassword, CancellationToken ct = default)
    {
        var passwordErrors = PasswordPolicy.Validate(newPassword);

        if (passwordErrors.Any())
            throw new WeakPasswordException(passwordErrors); 

        var user = await _userRep.GetByIdAsync(_userProvider.RequireUserId(), ct)
            ?? throw new UserNotFoundException(); 

        user.ChangePassword(_passwordHasher.Hash(newPassword));
        await _userRep.UpdateAsync(user, ct);
    }

    public async Task UpdateProfileAsync(User user, CancellationToken ct = default)
    {
        await _userRep.UpdateAsync(user, ct);
        _userService.Clear();
    }

    public async Task DeleteProfileAsync(CancellationToken ct = default)
    {
        await _userRep.DeleteAsync(_userProvider.RequireUserId(), ct);
        Logout();
    }
}