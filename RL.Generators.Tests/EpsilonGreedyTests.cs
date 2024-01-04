using RL.Core.Tests;
using Xunit.Abstractions;
using static RL.Generators.Generator;

namespace RL.Generators.Tests;

public class EpsilonGreedyTests(ITestOutputHelper output) : TestsBase(output)
{
    public static IEnumerable<object[]> Data =>
        Range(0, 10).Select(i => new object[] { i / 10.0 }).AsEnumerable();

    [Theory]
    [MemberData(nameof(Data))]
    public void EpsilonGreedyProbabilitiesSumOne(double epsilon)
    {
        var policy = Range<double>(5).EpsilonGreedy(epsilon);
        var sum = policy.Sum();
        Output.AppendLine($"[{string.Join(", ", policy)}].Sum() = {sum}");
        Assert.Equal(1.0, sum);
    }
}