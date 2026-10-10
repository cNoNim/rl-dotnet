using System.Collections.Generic;
using RL.Algorithms;
using RL.Algorithms.Callbacks;

namespace RL.LunarLander;

public class EpisodeRewardsCollector<TO, TA> :
    IBeforeCallback<TO, TA>,
    IStepCallback<TO, TA>
{
    private readonly List<float> _rewards = [];
    private float _episodeReward;

    public IReadOnlyList<float> Rewards => _rewards;

    public void OnBefore(IAlgorithm<TO, TA> algorithm)
    {
        _rewards.Clear();
        _episodeReward = 0;
    }

    public void OnStep(IAlgorithm<TO, TA> algorithm)
    {
        var transition = algorithm.Transition;
        _episodeReward += transition.Reward;
        if (transition.Terminated || transition.Truncated)
        {
            _rewards.Add(_episodeReward);
            _episodeReward = 0;
        }
    }
}
