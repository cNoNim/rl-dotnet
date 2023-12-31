using System.Globalization;
using RL.Core;
using RL.Plot;
using RL.Random;
using static RL.Core.List;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

const int count = 100000;

var seed = XxHash32Algorithm.Hash(0, System.Random.Shared.Next());

var generator = new RandomGenerator(seed);
foreach (var epsilon in Range(0.0, 1.0, 0.5))
    EpsilonGreedy(generator, epsilon, count, 6);

return;

static void EpsilonGreedy(RandomGenerator generator, double epsilon, int count, int valuesCount)
{
    var probabilities = Range(valuesCount).EpsilonGreedy(epsilon).ToArray();

    var ints = Range(count)
        .Select((generator, probabilities), static (tuple, _) => tuple.probabilities.ChoiceIndex(tuple.generator))
        .ToArray();

    var distribution = new double[valuesCount];
    foreach (var i in ints)
        distribution[i]++;
    distribution = distribution.Select(count, (c, v) => v / c).ToArray();

    Plot.Create($"RandomGenerator(seed: {generator.Seed}) EpsilonGreedy(epsilon: {epsilon})")
        .ConfigureXAxis(configurator => configurator.SetMin(0.0).SetMax(1.0))
        .Signal(distribution)
        .ToPng($"Distribution_EpsilonGreedy_{epsilon:F}_{count}.png");
}
