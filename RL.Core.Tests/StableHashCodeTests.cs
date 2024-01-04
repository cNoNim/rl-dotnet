using Xunit.Abstractions;

namespace RL.Core.Tests;

public class StableHashCodeTests(ITestOutputHelper output) : TestsBase(output)
{
    [Fact]
    public void Test()
    {
        var hash = StableHashCode.Hash(0, (ReadOnlySpan<int>) [1, 2, 3]);
        Assert.Equal(StableHashCode.Hash(0, 1, 2, 3), hash);
    }
}