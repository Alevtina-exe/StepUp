using StepUp.Application.Interfaces.Services;

namespace StepUp.Maui.Services
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        public Guid? UserId { get; private set; }

        public bool IsAuthenticated => UserId != null;

        public Guid RequireUserId() => UserId ?? throw new InvalidOperationException(
            "Действие невозможно: пользователь не авторизован.");

        public void Clear() => UserId = null;

        public void SetUser(Guid userId) => UserId = userId;
    }
}
