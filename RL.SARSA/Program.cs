using RL.Core;
using RL.Environments;
using RL.Plot;
using RL.Random;
using RL.Toy;

var environment = new TaxiEnvironment();
const int episodeCount = 500;
const int stepCount = 1000;
var rewards = Sarsa(environment, episodeCount, stepCount, gamma: 0.999);
rewards.PlotToSvg("SARSA", "Reward", "Episodes", $"SARSA_{episodeCount}_{stepCount}.svg");

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

    foreach (var episode in List.Range(episodeCount))
    {
        var totalReward = 0.0;
        var epsilon = 1.0 - episode / (double)episodeCount;

        var state = environment.Reset();
        var action = q[state].EpsilonGreedyProbabilities(epsilon).ChoiceIndex(environment.Generator);

        foreach (var _ in List.Range(stepCount))
        {
            var (nextState, reward, done, _) = environment.Step(action);
            var nextAction = q[nextState].EpsilonGreedyProbabilities(epsilon).ChoiceIndex(environment.Generator);

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
