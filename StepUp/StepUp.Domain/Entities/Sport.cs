namespace StepUp.Domain.Entities
{
    public class Sport
    {
        public Guid Id { get; private set; }
        public Guid DayResultId { get; }
        public string Name { get; private set; } = null!;
        public int BurntKcals { get; private set; }
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

        public void ChangeName(string? newName)
        {
            Name = string.IsNullOrWhiteSpace(newName) ? "Без названия" : newName;
        }

        public void ChangeKcal(int newKcal)
        {
            if (newKcal < 0)
                throw new ArgumentOutOfRangeException(nameof(newKcal), "Калории не могут быть отрицательными.");
            BurntKcals = newKcal;
        }

    }
}
