using System;
using RL.Algorithms;
using RL.Algorithms.Callbacks;

namespace RL.LunarLander;

public class EpisodeLengthConsumer<TO, TA> :
    IBeforeCallback<TO, TA>,
    IStepCallback<TO, TA>,
    IAfterCallback<TO, TA>
{
    private int _episode;
    private int _counter;
    private Action<int, int>? _callback;

    public void On(Action<int, int> callback) =>
        _callback = callback;

    void IBeforeCallback<TO, TA>.OnBefore(IAlgorithm<TO, TA> algorithm)
    {
        _episode = 0;
        _counter = 0;
    }

    void IStepCallback<TO, TA>.OnStep(IAlgorithm<TO, TA> algorithm)
    {
        var transition = algorithm.Transition;
        _counter++;
        if (transition is { Terminated: false, Truncated: false })
            return;
        _callback?.Invoke(_episode, _counter);
        _counter = 0;
        _episode++;
    }

    void IAfterCallback<TO, TA>.OnAfter(IAlgorithm<TO, TA> algorithm) =>
        _callback = null;
}
