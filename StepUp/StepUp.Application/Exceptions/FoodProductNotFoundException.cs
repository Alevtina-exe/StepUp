namespace StepUp.Application.Exceptions
{
    public class FoodProductNotFoundException : Exception
    {
        public FoodProductNotFoundException() : base("Продукт в базе не найден.") { }
    }
}
