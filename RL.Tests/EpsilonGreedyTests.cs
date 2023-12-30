using RL.Core;
using Xunit.Abstractions;

namespace RL.Tests;

public class EpsilonGreedyTests(ITestOutputHelper output) : TestsBase(output)
{
    [Theory]
    [MemberData(nameof(Data))]
    public void EpsilonGreedyProbabilitiesSumOneTest(double epsilon)
    {
        var policy = List.Range(5).EpsilonGreedyProbabilities(epsilon);
        var sum = policy.Sum();
        Output.AppendLine($"[{string.Join(", ", policy)}].Sum() = {sum}");
        Assert.Equal(1.0, sum);
    }

    public static IEnumerable<object[]> Data =>
        Enumerable.Range(0, 11).Select<int, object[]>(i => [i / 10.0]);
}
