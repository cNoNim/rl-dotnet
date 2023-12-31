using RL.Tests;
using Xunit.Abstractions;
using static RL.Core.List;

namespace RL.Random.Tests;

public class EpsilonGreedyDistributionTests(ITestOutputHelper output) : TestsBase(output)
{
    [Theory]
    [MemberData(nameof(Data))]
    public void EpsilonGreedyDistribution(int count, double epsilon, int valuesCount)
    {
        var probabilities = Range(count).EpsilonGreedy(epsilon).ToArray();
        var sum = probabilities.Sum();

        var seed = XxHash32Algorithm.Hash(0, count, valuesCount);
        var generator = new RandomGenerator(seed);
        var values = Range(valuesCount)
            .Select((generator, probabilities), static (tuple, _) => tuple.probabilities.ChoiceIndex(tuple.generator))
            .ToArray();

        var distribution = new double[count];
        foreach (var i in values)
            distribution[i]++;
        distribution = distribution.Select(valuesCount, (c, v) => v / c).ToArray();

        Output.AppendLine($"seed = {seed}");
        Output.AppendLine($"[{string.Join(", ", probabilities)}].Sum() = {sum}");
        Output.AppendLine($"distribution({valuesCount}) = [{string.Join(", ", distribution)}]");
        Assert.Equal(1.0, sum);

        foreach (var (p, d) in probabilities.Zip(distribution))
            Assert.Equal(p, d, 2);
    }

    public static IEnumerable<object[]> Data =>
        Range(0, 10).Select(i => new object[] { 5, i / 10.0, 100000 });
}
