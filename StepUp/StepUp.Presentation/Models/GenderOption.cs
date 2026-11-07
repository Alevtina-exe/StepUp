using StepUp.Domain.Enums;

namespace StepUp.Presentation.Models
{
    public record GenderOption(Gender Value, string DisplayName)
    {
        public static readonly GenderOption Male = new(Gender.Male, "Мужской");
        public static readonly GenderOption Female = new(Gender.Female, "Женский");

        public static IReadOnlyList<GenderOption> All { get; } = [Male, Female];
    }
}