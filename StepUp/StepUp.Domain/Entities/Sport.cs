namespace StepUp.Domain.Entities
{
    public class Sport
    {
        public Guid Id { get; private set; }
        public Guid DayResultId { get; }
        public string Name { get; set; } = null!;
        public int BurntKcals { get; set; }
        private Sport () { }
        public Sport(Guid dayResultId, string? name, int burntKcals)
        {
            Id = Guid.NewGuid();
            DayResultId = dayResultId;
            if (String.IsNullOrEmpty(name))
            {
                Name = "Без названия";
            }
            else Name = name;
            BurntKcals = burntKcals;
        }
    }
}
