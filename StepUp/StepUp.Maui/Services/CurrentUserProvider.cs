using StepUp.Application.Interfaces.Services;
using StepUp.Presentation.Interfaces;

namespace StepUp.Maui.Services
{
    public class CurrentUserProvider(ISessionStorage storage) : ICurrentUserProvider
    {
        private bool _loaded;

        public Guid? UserId { get; private set; }

        public bool IsAuthenticated => UserId != null;

        public Guid RequireUserId() => UserId ?? throw new InvalidOperationException(
            "Действие невозможно: пользователь не авторизован.");

        public async Task EnsureLoadedAsync()
        {
            if (_loaded) return;
            _loaded = true;

            UserId = await storage.GetUserIdAsync();
        }

        public void Clear()
        {
            UserId = null;
            _ = storage.ClearAsync();
        }

        public void SetUser(Guid userId)
        {
            UserId = userId;
            _ = storage.SaveUserIdAsync(userId);
        }
    }
}
