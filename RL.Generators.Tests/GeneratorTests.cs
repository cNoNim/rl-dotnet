using RL.Core.Tests;
using Xunit.Abstractions;
using static RL.Generators.Generator;

namespace RL.Generators.Tests;

public class GeneratorTests(ITestOutputHelper output) : TestsBase(output)
{
    [Fact]
    public void ByIndex() =>
        Assert.Equal(5, Sequence<int>()[5]);

    [Fact]
    public void Skip()
    {
        var skip = Sequence<int>().Skip(5);
        Assert.Equal(5, skip[0]);
    }

    [Fact]
    public void Take()
    {
        var take = Sequence<int>().Take(5);
        Assert.Equal(5, take.Count);
        Assert.Equal(5, take.Count());
        Assert.Equal([0, 1, 2, 3, 4], take.AsEnumerable().ToArray());
        var count = 0;
        foreach (var _ in take)
            count++;
        Assert.Equal(5, count);
    }

    [Fact]
    public void Count() =>
        Assert.Throws<OverflowException>(() => { Sequence<int>().Count(); });

    [Fact]
    public void Enumerable() =>
        Assert.Throws<OverflowException>(() =>
        {
            foreach (var _ in Sequence<int>())
            {
            }
        });
}