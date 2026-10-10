using System;
using RL.Algorithms;
using RL.Algorithms.Callbacks;

namespace RL.LunarLander;

public class EpisodeMeanLossConsumer<TO, TA> :
    IBeforeCallback<TO, TA>,
    IStepCallback<TO, TA>,
    ITrainStepCallback<TO, TA>,
    IAfterCallback<TO, TA>
{
    private int _episode;
    private double _loss;
    private int _counter;

    private Action<int, double>? _callback;

    public void On(Action<int, double> callback) =>
        _callback = callback;

    void IBeforeCallback<TO, TA>.OnBefore(IAlgorithm<TO, TA> algorithm)
    {
        _episode = 0;
        _loss = 0;
        _counter = 0;
    }

    void IStepCallback<TO, TA>.OnStep(IAlgorithm<TO, TA> algorithm)
    {
        var transition = algorithm.Transition;
        if (transition.Terminated || transition.Truncated)
        {
            _callback?.Invoke(_episode, _loss / _counter);
            _counter = 0;
            _loss = 0;
            _episode++;
        }
    }

    void ITrainStepCallback<TO, TA>.OnTrainStep(IAlgorithm<TO, TA> algorithm)
    {
        _counter++;
        _loss += algorithm.Loss;
    }

    void IAfterCallback<TO, TA>.OnAfter(IAlgorithm<TO, TA> algorithm) =>
        _callback = null;
}
