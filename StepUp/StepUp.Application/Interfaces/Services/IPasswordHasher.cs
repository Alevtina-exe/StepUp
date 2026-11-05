namespace StepUp.Application.Interfaces.Services
{
    internal interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string hash);
    }
}
