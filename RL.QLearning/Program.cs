using RL.Core;
using RL.Environments;
using RL.Plot;
using RL.Random;
using RL.Toy;

var environment = new TaxiEnvironment();
const int episodeCount = 500;
const int noisyEpisodeCount = 400;
const int stepCount = 1000;
var rewards = QLearning(environment, episodeCount, noisyEpisodeCount, stepCount, gamma: 0.999);
rewards.PlotToSvg("Q-Learning", "Reward", "Episodes", $"QLearning_{episodeCount}_{stepCount}.svg");

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

    foreach (var episode in List.Range(episodeCount))
    {
        var totalReward = 0.0;
        var state = environment.Reset();

        foreach (var _ in List.Range(stepCount))
        {
            var action = q[state].EpsilonGreedyProbabilities(epsilon).ChoiceIndex(environment.Generator);
            var (nextState, reward, done, _) = environment.Step(action);

            q[state][action] += alpha * (reward + gamma * q[nextState].Max() - q[state][action]);

            state = nextState;

            totalReward += reward;
            if (done)
                break;
        }

        epsilon = double.Max(0.0, epsilon - 1.0 / noisyEpisodeCount);
        totalRewards[episode] = totalReward;
    }

    return totalRewards;
}
