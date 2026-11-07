using StepUp.Domain.Enums;

namespace StepUp.Presentation.Models;

public record SportLevelOption(SportLevel Value, string DisplayName)
{
    public static readonly SportLevelOption No = new(SportLevel.No, "Нет физ. нагрузки");
    public static readonly SportLevelOption Low = new(SportLevel.Low, "Лёгкая физ. нагрузка");
    public static readonly SportLevelOption Mid = new(SportLevel.Mid, "Средняя физ. нагрузка");
    public static readonly SportLevelOption High = new(SportLevel.High, "Ежедневная физ. нагрузка");
    public static readonly SportLevelOption Extreme = new(SportLevel.Extreme, "Профессиональный спорт");

    public static IReadOnlyList<SportLevelOption> All { get; } = [No, Low, Mid, High, Extreme];

    public static SportLevelOption FromSportLevel(SportLevel level) =>
        All.First(x => x.Value == level);
}