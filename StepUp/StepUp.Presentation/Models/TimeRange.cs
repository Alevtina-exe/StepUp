namespace StepUp.Presentation.Models
{
    public record TimeRange(string Name, int DurationInDays)
    {
        public static readonly TimeRange Week = new("неделя", 7);
        public static readonly TimeRange Month = new("месяц", 30);
        public static readonly TimeRange Year = new("год", 365);

        public static readonly List<TimeRange> All = [Week, Month, Year];
        public override string ToString() => Name;
        
    }
}
