using RL.MDArrays;
using RL.Plot;
using RL.Random;
using static RL.Generators.Generator;

const int count = 100000;

var seed = XxHash32Algorithm.Hash(0, System.Random.Shared.Next());

var generator = new RandomGenerator(seed);
foreach (var epsilon in Range(0.0, 1.0, 0.5))
    EpsilonGreedy(generator, epsilon, count, 6);

return;

static void EpsilonGreedy(RandomGenerator generator, double epsilon, int count, int valuesCount)
{
    var probabilities = Range<int>(valuesCount).EpsilonGreedy(epsilon).ToMDArray();

    var ints = Range<int>(count)
        .Select((generator, probabilities), static (tuple, _) => tuple.probabilities.ChoiceIndex(tuple.generator))
        .ToMDArray();

    var distribution = valuesCount.Zeroes<double>();
    foreach (var i in ints)
        distribution[i]++;
    distribution = distribution.Select(count, (c, v) => v / c).ToMDArray();

    Plot.Create($"RandomGenerator(seed: {generator.Seed}) EpsilonGreedy(epsilon: {epsilon})")
        .ConfigureXAxis(configurator => configurator.SetMin(0.0).SetMax(1.0))
        .Signal(distribution)
        .ToPng($"Distribution_EpsilonGreedy_{epsilon:F}_{count}.png");
}