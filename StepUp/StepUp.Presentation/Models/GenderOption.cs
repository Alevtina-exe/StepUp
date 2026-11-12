using StepUp.Domain.Extensions;

namespace StepUp.Presentation.Models
{
    public record GenderOption(Gender Value, string DisplayName)
    {
        public static readonly GenderOption Male = new(Gender.Male, Gender.Male.GetDescription());
        public static readonly GenderOption Female = new(Gender.Female, Gender.Female.GetDescription());

        public static IReadOnlyList<GenderOption> All => [Male, Female];

        public override string ToString() => $"{DisplayName}";
        
    }
}