using Xunit.Abstractions;
using static RL.Core.List;

namespace RL.Tests;

public class EpsilonGreedyTests(ITestOutputHelper output) : TestsBase(output)
{
    [Theory]
    [MemberData(nameof(Data))]
    public void EpsilonGreedyProbabilitiesSumOne(double epsilon)
    {
        var policy = Range(5).EpsilonGreedy(epsilon);
        var sum = policy.Sum();
        Output.AppendLine($"[{string.Join(", ", policy)}].Sum() = {sum}");
        Assert.Equal(1.0, sum);
    }

    public static IEnumerable<object[]> Data =>
        Range(0, 10).Select(i => new object[] { i / 10.0 });
}
