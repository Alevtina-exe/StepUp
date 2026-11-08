using StepUp.Application.Interfaces.Services;

namespace StepUp.Infrastructure.Time;

public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;

    public DateTime Today => DateTime.Now.Date;
}