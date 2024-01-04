using RL.Core;
using RL.Environments;
using RL.Environments.Spaces;
using RL.MDArrays;
using RL.Random;
using static RL.Generators.Generator;

namespace RL.Algorithms;

public readonly struct QLearning(
    IGenerator<int> episodeGenerator,
    int stepCount,
    double gamma = 0.99,
    double alpha = 0.5
) : IDiscreteAlgorithm
{
    public string Name => nameof(QLearning);


    public Array2D<double> CreateQ(IEnvironment<Discrete, Discrete, int, int> environment) =>
        (environment.ObservationSpace.Size, environment.ActionSpace.Size).Zeroes<double>();

    public (Array1D<double> rewards, Array2D<double> qTable) Train(
        IEnvironment<Discrete, Discrete, int, int> environment,
        Array2D<double>? qTable = null
    )
    {
        var totalRewards = episodeGenerator.Count.Zeroes<double>();
        var q = qTable ?? CreateQ(environment);

        foreach (var episode in episodeGenerator.AsGeneratorEnumerable())
        {
            var totalReward = 0.0;
            var state = environment.Reset();
            var epsilon = 1.0 / (episode + 1);

            foreach (var _ in Range<int>(stepCount))
            {
                var action = q[state].EpsilonGreedy(epsilon).ChoiceIndex(environment.Random);
                var t = environment.Step(action);

                q[t.State][t.Action] += alpha * (t.Reward + gamma * q[t.NextState].Max() - q[t.State][t.Action]);

                totalReward += t.Reward;

                if (t.Terminated || t.Truncated)
                    break;

                state = t.NextState;
            }

            totalRewards[episode] = totalReward;
        }

        return (totalRewards, q);
    }
}