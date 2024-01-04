using RL.Environments;
using RL.Environments.Spaces;
using RL.Random;
using RL.Tensors;
using static RL.Generators.Generator;

namespace RL.Algorithms;

public readonly struct QLearning(
    int episodeCount,
    int stepCount,
    double gamma = 0.99,
    double alpha = 0.5
) : IAlgorithm<Discrete, Discrete, int, int>
{
    public string Name => nameof(QLearning);

    public (Tensor1D<double> rewards, Tensor2D<double> qTable) Train(
        IEnvironment<Discrete, Discrete, int, int> environment,
        Tensor2D<double>? qTable = null
    )
    {
        var totalRewards = episodeCount.Zeroes<double>();
        var q = qTable ?? (environment.ObservationSpace.Size, environment.ActionSpace.Size).Zeroes<double>();

        foreach (var episode in Range<int>(episodeCount))
        {
            var totalReward = 0.0;
            var state = environment.Reset();
            var epsilon = 1.0 / (episode + 1);

            foreach (var _ in Range<int>(stepCount))
            {
                var action = q[state].EpsilonGreedy(epsilon).ChoiceIndex(environment.Random);
                var (nextState, reward, done) = environment.Step(action);

                q[state][action] += alpha * (reward + gamma * q[nextState].Max() - q[state][action]);

                totalReward += reward;

                if (done)
                    break;

                state = nextState;
            }

            totalRewards[episode] = totalReward;
        }

        return (totalRewards, q);
    }
}
