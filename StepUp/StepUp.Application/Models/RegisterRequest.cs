using StepUp.Domain.Enums;

namespace StepUp.Application.Models;

public class RegisterRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; } = DateTime.Today;
    public Gender Gender { get; set; } = Gender.Male;
    public double Weight { get; set; } = 70;
    public int Height { get; set; } = 170;
    public int CaloriePlan { get; set; } = 1800;
    public int ProteinPercent { get; set; } = 30;
    public int FatPercent { get; set; } = 30;
    public int CarbonPercent { get; set; } = 40;
    public SportLevel SportLevel { get; set; } = SportLevel.Low;
}