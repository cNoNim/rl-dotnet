using RL.Algorithms.Callbacks;
using RL.Environments;

namespace RL.Algorithms;

public interface IAlgorithm<TO, TA>
{
    Transition<TO, TA> Transition { get; }
    double Loss => 0.0;
    public bool Register<T>(T callback) where T : ICallbackNode => false;
}
