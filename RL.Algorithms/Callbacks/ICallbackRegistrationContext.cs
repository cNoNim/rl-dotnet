namespace RL.Algorithms.Callbacks;

public interface ICallbackRegistrationContext
{
    public bool Register<T>(T node);
}