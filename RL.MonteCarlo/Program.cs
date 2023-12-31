using RL.Environments;
using RL.Generators;
using RL.MDArrays;
using RL.Plot;
using RL.Random;
using RL.Toy;
using static RL.Generators.Generator;

const int episodeCount = 20000;
const int stepCount = 1000;

var environment = new TaxiEnvironment();

var rewards = MonteCarlo(environment, episodeCount, stepCount, gamma: 0.99);

Plot.Create("Monte-Carlo")
    .ConfigureXAxis(c => c.SetTitle("Reward"))
    .ConfigureYAxis(c => c.SetTitle("Episodes"))
    .Signal(rewards)
    .ToPng($"Images/MonteCarlo_{episodeCount}_{stepCount}.png");

return;

static Array1D<double> MonteCarlo(
    IEnvironment<int, int> environment,
    int episodeCount,
    int stepCount = 500,
    double gamma = 0.99
)
{
    var totalRewards = episodeCount.Zeroes<double>();
    var q = (environment.ObservationSpace.Size, environment.ActionSpace.Size).Zeroes<double>();
    var counter = (environment.ObservationSpace.Size, environment.ActionSpace.Size).Zeroes<int>();

    var steps = new List<(int state, int action, double reward)>(stepCount);
    var returns = new double[stepCount + 1];

    foreach (var episode in Range<int>(episodeCount))
    {
        var epsilon = 1.0 - episode / (double)episodeCount;

        var state = environment.Reset();
        foreach (var _ in Range<int>(stepCount))
        {
            var s = state;
            var action = q[state].EpsilonGreedy(epsilon).ChoiceIndex(environment.Random);
            (state, var reward, var done) = environment.Step(action);
            steps.Add((s, action, reward));

            if (done)
                break;
        }

        totalRewards[episode] = steps.Select(tuple => tuple.reward).Sum();

        var count = steps.Count;

        foreach (var i in Range(count - 1, 0, -1))
            returns[i] = steps[i].reward + gamma * returns[i + 1];

        foreach (var i in Range<int>(count))
        {
            var (s, a, _) = steps[i];
            q[s][a] += (returns[i] - q[s][a]) / (1 + counter[s][a]);
            counter[s][a] += 1;
        }

        Array.Clear(returns);
        steps.Clear();
    }

    return totalRewards;
}
