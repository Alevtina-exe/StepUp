using StepUp.Application.Exceptions;
using StepUp.Application.Interfaces.Repositories;
using StepUp.Application.Interfaces.Services;

namespace StepUp.Application.Services;

public class CurrentUserService(
    IUserRepository users,
    ICurrentUserProvider provider) : ICurrentUserService
{
    private User? _cached;

    public User? Current => _cached;

    public async Task<User> GetOrLoadAsync(CancellationToken ct = default)
    {
        if (_cached is not null)
            return _cached;

        var id = provider.RequireUserId();
        _cached = await users.GetByIdAsync(id, ct)
            ?? throw new UserNotFoundException();

        return _cached;
    }

    public void Clear() => _cached = null;
}