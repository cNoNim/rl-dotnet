using RL.Algorithms;
using RL.Environments;
using RL.Environments.Spaces;
using RL.Plot;
using RL.Toy;

const int episodeCount = 500;
const int stepCount = 1000;
const double gamma = 0.999;

var monteCarlo = new MonteCarlo(episodeCount, stepCount, gamma);
var qLearning = new QLearning(episodeCount, stepCount, gamma);
var sarsa = new Sarsa(episodeCount, stepCount, gamma);

IAlgorithm<Discrete, Discrete, int, int>[] algorithms =
[
    monteCarlo,
    sarsa,
    qLearning
];

TrainEnvironment(new CliffWalkingEnvironment(), algorithms);
TrainEnvironment(new TaxiEnvironment(), algorithms);

return;

static void TrainEnvironment<TEnvironment>(TEnvironment environment,
    params IAlgorithm<Discrete, Discrete, int, int>[] algorithms)
    where TEnvironment : IEnvironment<Discrete, Discrete, int, int>
{
    var builder = Plot.Create(environment.Name)
        .ConfigureXAxis(c => c.SetTitle("Reward"))
        .ConfigureYAxis(c => c.SetTitle("Episodes"));

    foreach (var algorithm in algorithms)
    {
        var (rewards, _) = algorithm.Train(environment);
        builder.Signal(rewards, c => c.SetTitle(algorithm.Name));
    }

    builder.ToPng($"Images/{environment.Name}_{episodeCount}_{stepCount}.png");
}
