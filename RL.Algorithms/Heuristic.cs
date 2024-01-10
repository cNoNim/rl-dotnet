using RL.Core;
using RL.Environments;
using RL.Generators;
using RL.MDArrays;

namespace RL.Algorithms;

public static class HeuristicExtensions
{
    public static Array1D<float> Heuristic<TO, TA, TG>(this IEnvironment<TO, TA> environment, TG episodeGenerator)
        where TG : IGenerator<int> =>
        Heuristic<IEnvironment<TO, TA>, TO, TA, TG>(environment, episodeGenerator);

    public static Array1D<float> Heuristic<TE, TO, TA, TG>(this TE environment, TG episodeGenerator)
        where TE : IEnvironment<TO, TA>
        where TG : IGenerator<int>
    {
        var rewards = episodeGenerator.Count.Zeroes<float>();

        foreach (var episode in episodeGenerator.AsGeneratorEnumerable())
        {
            var episodeReward = 0.0f;
            environment.Reset();
            while (true)
            {
                var action = environment.Heuristic(environment.State);
                var transition = environment.Step(action);
                episodeReward += transition.Reward;
                if (transition.Terminated || transition.Truncated)
                    break;
            }

            rewards[episode] = episodeReward;
        }

        return rewards;
    }
}