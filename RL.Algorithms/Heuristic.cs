using System;
using RL.Algorithms.Callbacks;
using RL.Environments;

namespace RL.Algorithms;

public class Heuristic<TO, TA> :
    IAlgorithm<TO, TA>
{
    private readonly CallbackCollection<TO, TA> _collection = new();
    private readonly IEnvironment<TO, TA> _environment;
    private readonly ICompletion _completion;

    public Heuristic(
        IEnvironment<TO, TA> environment,
        ICompletion completion
    )
    {
        _environment = environment;
        _completion = completion;
        _collection.Register(completion);
    }

    public Transition<TO, TA> Transition { get; private set; }
    public double Loss => 0.0;

    public bool Register<T>(T callback) where T : ICallbackNode =>
        _collection.Register(callback);

    public void Run(Func<TO, TA> heuristic)
    {
        _environment.Reset();
        try
        {
            _collection.Before(this);

            while (true)
            {
                var action = heuristic(_environment.Observation);
                _collection.BeforeStep(this);
                Transition = _environment.Step(action);
                _collection.Step(this);
                if (_completion.Complete)
                    return;
            }
        }
        finally
        {
            _collection.After(this);
        }
    }
}
