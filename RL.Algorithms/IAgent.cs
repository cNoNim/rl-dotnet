namespace RL.Algorithms;

public interface IAgent<in TO, out TA>
{
    public string Name { get; }
    public TA Predict(TO state);
}
