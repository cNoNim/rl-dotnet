namespace RL.Algorithms;

public interface IPolicy<in TO, out TA>
{
    public TA Predict(TO state);
}