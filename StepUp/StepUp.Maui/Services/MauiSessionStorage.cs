using StepUp.Presentation.Interfaces;

namespace StepUp.Maui.Services;

public class MauiSessionStorage : ISessionStorage
{
    private const string UserIdKey = "session_user_id";

    public async Task<Guid?> GetUserIdAsync()
    {
        try
        {
            var value = await SecureStorage.Default.GetAsync(UserIdKey);
            return Guid.TryParse(value, out var id) ? id : null;
        }
        catch
        {
            return null;
        }
    }

    public Task SaveUserIdAsync(Guid userId)
        => SecureStorage.Default.SetAsync(UserIdKey, userId.ToString());

    public Task ClearAsync()
    {
        SecureStorage.Default.Remove(UserIdKey);
        return Task.CompletedTask;
    }
}