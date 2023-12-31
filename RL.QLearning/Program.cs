using System.Globalization;
using RL.Core;
using RL.Environments;
using RL.Plot;
using RL.Random;
using RL.Toy;
using static System.Math;
using static RL.Core.List;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

const int episodeCount = 500;
const int noisyEpisodeCount = 400;
const int stepCount = 1000;

var environment = new TaxiEnvironment();

var rewards = QLearning(environment, episodeCount, noisyEpisodeCount, stepCount, gamma: 0.999);

Plot.Create("Q-Learning")
    .ConfigureXAxis(c => c.SetTitle("Reward"))
    .ConfigureYAxis(c => c.SetTitle("Episodes"))
    .Signal(rewards)
    .ToPng($"QLearning_{episodeCount}_{stepCount}.png");

return;

static IReadOnlyList<double> QLearning(
    IEnvironment<int, int> environment,
    int episodeCount,
    int noisyEpisodeCount,
    int stepCount = 500,
    double gamma = 0.99,
    double alpha = 0.5
)
{
    var totalRewards = new double[episodeCount];

    var observationSpaceCount = environment.ObservationSpace.Size;
    var actionSpaceCount = environment.ActionSpace.Size;
    var q = new double[observationSpaceCount, actionSpaceCount].AsMatrix();
    var epsilon = 1.0;

    foreach (var episode in Range(episodeCount))
    {
        var totalReward = 0.0;
        var state = environment.Reset();

        foreach (var _ in Range(stepCount))
        {
            var action = q[state].EpsilonGreedy(epsilon).ChoiceIndex(environment.Generator);
            var (nextState, reward, done) = environment.Step(action);

            q[state][action] += alpha * (reward + gamma * q[nextState].Max() - q[state][action]);

            totalReward += reward;

            if (done)
                break;

            state = nextState;
        }

        totalRewards[episode] = totalReward;
        epsilon = Max(0.0, epsilon - 1.0 / noisyEpisodeCount);
    }

    return totalRewards;
}
