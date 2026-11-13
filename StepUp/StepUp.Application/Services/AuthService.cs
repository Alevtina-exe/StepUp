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

    public async Task<User> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var passwordErrors = PasswordPolicy.Validate(request.Password);
        if (passwordErrors.Any())
            throw new WeakPasswordException(passwordErrors); 

        if (await userRep.ExistsByEmailAsync(request.Email, ct))
            throw new EmailAlreadyExistsException(request.Email); 

        var user = new User(
            fullName: request.Name,
            email: request.Email,
            password: passwordHasher.Hash(request.Password),
            birthDate: request.BirthDate,
            gender: request.Gender,
            weight: request.Weight,
            height: request.Height,
            caloriePlan: request.CaloriePlan,
            carbonPercent: request.CarbonPercent,
            proteinPercent: request.ProteinPercent,
            fatPercent: request.FatPercent,
            sportLevel: request.SportLevel,
            registrationDate: clock.UtcNow);

        await userRep.AddAsync(user, ct);
        userProvider.SetUser(user.Id);
        return user;
    }

    public async Task<User> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var user = await userRep.GetByEmailAsync(email, ct)
            ?? throw new UserNotFoundException(email); 

        if (!passwordHasher.Verify(password, user.Password))
            throw new InvalidPasswordException();

        userProvider.SetUser(user.Id);
        return user;
    }

    public async Task<bool> IsUserExistsAsync(string email, CancellationToken ct = default) =>
        await userRep.ExistsByEmailAsync(email, ct);

    public void Logout()
    {
        userProvider.Clear();
        userService.Clear();
    }

    public async Task ChangePasswordAsync(string newPassword, CancellationToken ct = default)
    {
        var passwordErrors = PasswordPolicy.Validate(newPassword);

        if (passwordErrors.Any())
            throw new WeakPasswordException(passwordErrors); 

        var user = await userRep.GetByIdAsync(userProvider.RequireUserId(), ct)
            ?? throw new UserNotFoundException(); 

        user.ChangePassword(passwordHasher.Hash(newPassword));
        await userRep.UpdateAsync(user, ct);
    }

    public async Task UpdateProfileAsync(User user, CancellationToken ct = default)
    {
        await userRep.UpdateAsync(user, ct);
        userService.Clear();
    }

    public async Task DeleteProfileAsync(CancellationToken ct = default)
    {
        await   userRep.DeleteAsync(userProvider.RequireUserId(), ct);
        Logout();
    }
}