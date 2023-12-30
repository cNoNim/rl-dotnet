using RL.Core;
using Xunit.Abstractions;

namespace RL.Tests;

public class RangeListTests(ITestOutputHelper output) : TestsBase(output)
{
    [Theory]
    [InlineData(0, 5, 1, 6)]
    [InlineData(5, 0, -1, 6)]
    [InlineData(-2, 2, 1, 5)]
    [InlineData(2, -2, -1, 5)]
    public void Test(int from, int to, int step, int count)
    {
        var rangeList = List.Range(from, to, step);
        Output.Append('[');
        Output.Append(string.Join(", ", rangeList));
        Output.Append("].Count = ");
        Output.AppendLine(rangeList.Count.ToString());
        Assert.Equal(count, rangeList.Count);
    }
}
