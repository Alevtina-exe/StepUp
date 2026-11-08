namespace StepUp.Application.Exceptions
{
    public class FoodItemNotFoundException : Exception
    {
        public FoodItemNotFoundException() : base("Продукт в приёме пищи не найден.") { }
    }
}
