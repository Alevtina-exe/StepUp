namespace StepUp.Presentation.Interfaces
{
    public interface ISessionStorage
    {
        Task<Guid?> GetUserIdAsync();
        Task SaveUserIdAsync(Guid userId);
        Task ClearAsync();
    }
}
