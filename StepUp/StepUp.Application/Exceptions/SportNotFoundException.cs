namespace StepUp.Application.Exceptions
{
    public class SportNotFoundException : Exception
    {
        public SportNotFoundException() : base("Запись физической активности не найдена.") { }
    }
}