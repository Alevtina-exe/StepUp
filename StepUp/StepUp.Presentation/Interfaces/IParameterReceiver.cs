namespace StepUp.Presentation.Interfaces;

public interface IParameterReceiver<in T>
{
    void ReceiveParameter(T parameter);
}