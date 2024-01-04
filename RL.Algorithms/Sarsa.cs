using RL.Environments;
using RL.Environments.Spaces;
using RL.MDArrays;
using RL.Random;
using static RL.Generators.Generator;

namespace RL.Algorithms;

public readonly struct Sarsa(
    int episodeCount,
    int stepCount,
    double gamma = 0.99,
    double alpha = 0.5
) : IDiscreteAlgorithm
{
    public string Name => nameof(Sarsa);


    public Array2D<double> CreateQ(IEnvironment<Discrete, Discrete, int, int> environment) =>
        (environment.ObservationSpace.Size, environment.ActionSpace.Size).Zeroes<double>();

    public (Array1D<double> rewards, Array2D<double> qTable) Train(
        IEnvironment<Discrete, Discrete, int, int> environment,
        Array2D<double>? qTable = null
    )
    {
        var totalRewards = episodeCount.Zeroes<double>();
        var q = qTable ?? CreateQ(environment);

        foreach (var episode in Range<int>(episodeCount))
        {
            var totalReward = 0.0;
            var epsilon = 1.0 / (episode + 1);

            var state = environment.Reset();
            var action = q[state].EpsilonGreedy(epsilon).ChoiceIndex(environment.Random);

            foreach (var _ in Range<int>(stepCount))
            {
                var t = environment.Step(action);
                var nextAction = q[t.NextState].EpsilonGreedy(epsilon).ChoiceIndex(environment.Random);

                q[t.State][t.Action] += alpha * (t.Reward + gamma * q[t.NextState][nextAction] - q[t.State][t.Action]);

                action = nextAction;

                totalReward += t.Reward;
                if (t.Terminated || t.Truncated)
                    break;
            }

            totalRewards[episode] = totalReward;
        }

        return (totalRewards, q);
    }
}