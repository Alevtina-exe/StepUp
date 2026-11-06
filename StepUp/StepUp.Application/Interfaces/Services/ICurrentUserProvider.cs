namespace StepUp.Application.Interfaces.Services
{
    public interface ICurrentUserProvider
    {
        Guid? UserId { get; }
        bool IsAuthenticated { get; }
        Guid RequireUserId();
        void SetUser(Guid userId);
        void Clear();
    }
}
