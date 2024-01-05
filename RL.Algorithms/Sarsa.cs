using RL.Environments;
using RL.Environments.Spaces;
using RL.Random;
using RL.Tensors;
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

    
    public Tensor2D<double> CreateQ(IEnvironment<Discrete, Discrete, int, int> environment) => 
        (environment.ObservationSpace.Size, environment.ActionSpace.Size).Zeroes<double>();

    public (Tensor1D<double> rewards, Tensor2D<double> qTable) Train(
        IEnvironment<Discrete, Discrete, int, int> environment,
        Tensor2D<double>? qTable = null
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
                var (nextState, reward, done) = environment.Step(action);
                var nextAction = q[nextState].EpsilonGreedy(epsilon).ChoiceIndex(environment.Random);

                q[state][action] += alpha * (reward + gamma * q[nextState][nextAction] - q[state][action]);

                (state, action) = (nextState, nextAction);

                totalReward += reward;
                if (done)
                    break;
            }

            totalRewards[episode] = totalReward;
        }

        return (totalRewards, q);
    }
}
