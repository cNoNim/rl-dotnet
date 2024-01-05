using System;
using System.Linq;
using RL.Algorithms;
using RL.Classic;
using RL.Core;
using RL.Environments;
using RL.Environments.Spaces;
using RL.Generators;
using RL.Plot;
using RL.Progress;
using RL.Tensors;
using static RL.Generators.Generator;

const int episodeCount = 500;
const int stepCount = 1000;
const double gamma = 0.999;

var monteCarlo = new MonteCarlo(episodeCount, stepCount, gamma);
var qLearning = new QLearning(Range<int>(episodeCount), stepCount, gamma);
var sarsa = new Sarsa(episodeCount, stepCount, gamma);

IDiscreteAlgorithm[] algorithms =
[
    monteCarlo,
    sarsa,
    qLearning
];

// TrainEnvironment(new CliffWalkingEnvironment(), algorithms)
//     .ToPng($"Images/Cliff-Walking_{episodeCount}_{stepCount}.png");
// TrainEnvironment(new TaxiEnvironment(), algorithms).ToPng($"Images/Taxi_{episodeCount}_{stepCount}.png");
TrainEnvironmentN(
    new CartPoleEnvironment().AdaptObservationSpace(
        20,
        static (n, s) =>
        {
            var t = Range<int>(s.High.Shape)
                .AsEnumerable()
                .Aggregate((1, n), static (tuple, _) => (tuple.Item1 * tuple.n, tuple.n));
            return new Discrete(t.Item1);
        },
        static (n, space, state) =>
        {
            var result = 0;
            foreach (var (d, (l, h)) in state.Zip(space.Low.Zip(space.High)))
            {
                var dl = (double)l;
                var interval = (h - dl) / n;
                result *= n;
                result += (int)((d - dl) / interval);
            }

            return result;
        }
    ),
    4,
    new QLearning(Range<int>(250000).Progress(), 500),
    100
).ToPng($"Images/CartPoleDiscrete.png");

return;

static PlotBuilder TrainEnvironment<TEnvironment>(TEnvironment environment,
    params IDiscreteAlgorithm[] algorithms)
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

    return builder;
}

static PlotBuilder TrainEnvironmentN<TEnvironment>(
    TEnvironment environment,
    int count,
    IDiscreteAlgorithm algorithm,
    int chunkSize = 0
)
    where TEnvironment : IEnvironment<Discrete, Discrete, int, int>
{
    var builder = Plot.Create(environment.Name)
        .ConfigureXAxis(c => c.SetTitle("Reward"))
        .ConfigureYAxis(c => c.SetTitle("Episodes"));

    var q = algorithm.CreateQ(environment);
    foreach (var i in Range<int>(count))
    {
        Console.Write($"Step {i + 1,2}/{count} - ");
        (var rewards, q) = algorithm.Train(environment, q);
        if (chunkSize <= 0)
        {
            builder.Signal(rewards, Configure);
            continue;
        }

        var maxRewards = rewards
            .Chunks(chunkSize)
            .Select(c => c.Max());
        builder.Signal(maxRewards, Configure);
        continue;

        SeriesConfigurator Configure(SeriesConfigurator c) => c.SetTitle($"{algorithm.Name} Step {i + 1}");
    }

    return builder;
}