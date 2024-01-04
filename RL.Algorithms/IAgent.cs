namespace RL.Algorithms;

public interface IAgent<in TO, out TA>
{
    public TA Predict(TO state);
}
