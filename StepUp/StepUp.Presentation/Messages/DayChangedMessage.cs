using CommunityToolkit.Mvvm.Messaging.Messages;

namespace StepUp.Presentation.Messages;

public sealed class DayChangedMessage : ValueChangedMessage<DateTime>
{
    public DayChangedMessage(DateTime date) : base(date.Date) { }
}