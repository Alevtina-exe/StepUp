namespace StepUp.Application.Interfaces.Services
{
    public interface IUserProvider
    {
        Guid UserId { get; }
        bool IsAuthenticated { get; }
        void SetUser(Guid userId);
        void Clear();
    }
}
