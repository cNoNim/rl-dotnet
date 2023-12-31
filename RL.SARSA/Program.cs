using System.Globalization;
using RL.Core;
using RL.Environments;
using RL.Plot;
using RL.Random;
using RL.Toy;
using static RL.Core.List;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

const int episodeCount = 500;
const int stepCount = 1000;

var environment = new TaxiEnvironment();

var rewards = Sarsa(environment, episodeCount, stepCount, gamma: 0.999);

Plot.Create("SARSA")
    .ConfigureXAxis(c => c.SetTitle("Reward"))
    .ConfigureYAxis(c => c.SetTitle("Episodes"))
    .Signal(rewards)
    .ToPng($"Images/SARSA_{episodeCount}_{stepCount}.png");

return;

static IReadOnlyList<double> Sarsa(
    IEnvironment<int, int> environment,
    int episodeCount,
    int stepCount = 500,
    double gamma = 0.99,
    double alpha = 0.5
)
{
    var totalRewards = new double[episodeCount];

    var observationSpaceCount = environment.ObservationSpace.Size;
    var actionSpaceCount = environment.ActionSpace.Size;
    var q = new double[observationSpaceCount, actionSpaceCount].AsMatrix();

    foreach (var episode in Range(episodeCount))
    {
        var totalReward = 0.0;
        var epsilon = 1.0 / (episode + 1);

        var state = environment.Reset();
        var action = q[state].EpsilonGreedy(epsilon).ChoiceIndex(environment.Generator);

        foreach (var _ in Range(stepCount))
        {
            var (nextState, reward, done) = environment.Step(action);
            var nextAction = q[nextState].EpsilonGreedy(epsilon).ChoiceIndex(environment.Generator);

            q[state][action] += alpha * (reward + gamma * q[nextState][nextAction] - q[state][action]);

            (state, action) = (nextState, nextAction);

            totalReward += reward;
            if (done)
                break;
        }

        totalRewards[episode] = totalReward;
    }

    return totalRewards;
}
