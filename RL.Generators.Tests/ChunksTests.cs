using RL.Core.Tests;
using Xunit.Abstractions;
using static RL.Generators.Generator;

namespace RL.Generators.Tests;

public class ChunksTests(ITestOutputHelper output) : TestsBase(output)
{
    [Fact]
    public void Test100()
    {
        var chunks = Range<int>(100).Chunks(10);
        Assert.Equal(10, chunks.Count);
    }

    [Fact]
    public void Test101()
    {
        var chunks = Range<int>(101).Chunks(10);
        Assert.Equal(11, chunks.Count);
        Assert.Equal(1, chunks[10].Count);
    }
}