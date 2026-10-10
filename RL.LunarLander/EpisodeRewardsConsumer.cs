using System;
using RL.Algorithms;
using RL.Algorithms.Callbacks;

namespace RL.LunarLander;

public class EpisodeRewardsConsumer<TO, TA> :
    IBeforeCallback<TO, TA>,
    IStepCallback<TO, TA>,
    IAfterCallback<TO, TA>
{
    private int _episode;
    private float _episodeReward;
    private Action<int, float>? _callback;

    public void On(Action<int, float> callback) =>
        _callback = callback;

    void IBeforeCallback<TO, TA>.OnBefore(IAlgorithm<TO, TA> algorithm)
    {
        _episode = 0;
        _episodeReward = 0;
    }

    void IStepCallback<TO, TA>.OnStep(IAlgorithm<TO, TA> algorithm)
    {
        var transition = algorithm.Transition;
        _episodeReward += transition.Reward;
        if (transition is { Terminated: false, Truncated: false })
            return;
        _callback?.Invoke(_episode, _episodeReward);
        _episodeReward = 0;
        _episode++;
    }

    void IAfterCallback<TO, TA>.OnAfter(IAlgorithm<TO, TA> algorithm) =>
        _callback = null;
}
