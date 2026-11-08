namespace StepUp.Application.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() : base("Пользователь не найден.") { }
        public UserNotFoundException(string email) : base($"Пользователь с email '{email}' не найден.") { }
    }
}
