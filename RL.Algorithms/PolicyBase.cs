namespace RL.Algorithms;

public abstract class PolicyBase<TO, TA> : IPolicy<TO, TA>
{
    public abstract TA Predict(TO state);
}