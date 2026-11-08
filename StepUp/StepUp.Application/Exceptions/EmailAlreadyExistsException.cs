namespace StepUp.Application.Exceptions
{
    public class EmailAlreadyExistsException(string email) : Exception($"Email '{email}' уже зарегистрирован.")
    {
    }
}
