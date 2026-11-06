namespace StepUp.Application.Interfaces.Services;

public interface IClock
{
    DateTime UtcNow { get; }

    DateTime Today { get; }
}