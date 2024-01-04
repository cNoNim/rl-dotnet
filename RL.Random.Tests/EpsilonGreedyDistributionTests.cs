using RL.Core;
using RL.Core.Tests;
using RL.MDArrays;
using Xunit.Abstractions;
using static RL.Generators.Generator;

namespace RL.Random.Tests;

public class EpsilonGreedyDistributionTests(ITestOutputHelper output) : TestsBase(output)
{
    public static IEnumerable<object[]> Data =>
        Range(0, 10).Select(i => new object[] { 5, i / 10.0, 100000 }).AsEnumerable();

    [Theory]
    [MemberData(nameof(Data))]
    public void EpsilonGreedyDistribution(int count, double epsilon, int valuesCount)
    {
        var probabilities = Range<int>(count).EpsilonGreedy(epsilon).ToMDArray();
        var sum = probabilities.Sum();

        var seed = StableHashCode.Hash(0, [count, valuesCount]);
        var generator = new RandomGenerator(seed);
        var values = Range<int>(valuesCount)
            .Select(
                (generator, probabilities),
                static (tuple, _) => tuple.probabilities.ChoiceIndex(tuple.generator)
            )
            .ToMDArray();

        var distribution = new Array1D<double>(count);
        foreach (var i in values)
            distribution[i]++;
        distribution = distribution.Select(valuesCount, (c, v) => v / c).ToMDArray();

        Output.AppendLine($"seed = {seed}");
        Output.AppendLine($"[{string.Join(", ", probabilities)}].Sum() = {sum}");
        Output.AppendLine($"distribution({valuesCount}) = [{string.Join(", ", distribution)}]");
        Assert.Equal(1.0, sum);

        foreach (var (p, d) in probabilities.Zip(distribution))
            Assert.Equal(p, d, 2);
    }
}