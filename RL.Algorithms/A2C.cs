using RL.Algorithms.Callbacks;
using RL.Environments;
using RL.Random;

namespace RL.Algorithms;

public class A2C<TO, TA> : IAlgorithm<TO, TA>
{
    private readonly CallbackCollection<TO, TA> _collection = new();
    private readonly RandomGenerator _random = new();
    private readonly IPolicyEnvironment<TO, TA> _environment;

    public A2C(IPolicyEnvironment<TO, TA> environment)
    {
        _environment = environment;
    }

    public Transition<TO, TA> Transition { get; private set; }
    public double Loss { get; private set; }

    public bool Register<T>(T callback) where T : ICallbackNode =>
        _collection.Register(callback);
}
