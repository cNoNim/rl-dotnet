using RL.Environments;
using RL.MDArrays;
using RL.Random;
using static System.Math;
using static RL.Generators.Generator;

namespace RL.Algorithms;

public readonly struct QLearning(
    int episodeCount,
    int stepCount,
    int? noisyEpisodeCount = null,
    double gamma = 0.99,
    double alpha = 0.5
) : IAlgorithm<int, int>
{
    private readonly int _noisyEpisodeCount = noisyEpisodeCount ?? episodeCount * 80 / 100;

    public Array1D<double> Train(IEnvironment<int, int> environment)
    {
        var totalRewards = episodeCount.Zeroes<double>();
        var q = (environment.ObservationSpace.Size, environment.ActionSpace.Size).Zeroes<double>();
        var epsilon = 1.0;

        foreach (var episode in Range<int>(episodeCount))
        {
            var totalReward = 0.0;
            var state = environment.Reset();

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
            epsilon = Max(0.0, epsilon - 1.0 / _noisyEpisodeCount);
        }

        return totalRewards;
    }

    static string IAlgorithm<int, int>.Name => nameof(QLearning);
}