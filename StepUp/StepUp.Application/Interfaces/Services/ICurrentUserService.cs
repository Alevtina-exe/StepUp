namespace StepUp.Application.Interfaces.Services;

public interface ICurrentUserService
{
    User? Current { get; }

    Task<User> GetOrLoadAsync(CancellationToken ct = default);

    void Clear();
}