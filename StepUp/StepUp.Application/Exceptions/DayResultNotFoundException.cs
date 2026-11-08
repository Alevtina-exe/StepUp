namespace StepUp.Application.Exceptions
{
    public class DayResultNotFoundException : Exception
    {
        public DayResultNotFoundException() : base("Запись дня не найдена.") { }
    }
}