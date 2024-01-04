using RL.Core.Tests;
using RL.Generators;
using Xunit.Abstractions;

namespace RL.Toy.Tests;

public class TaxiEnvironmentTests(ITestOutputHelper output) : TestsBase(output)
{
    [Fact]
    public void InitialDistributionSumOne()
    {
        var environment = new TaxiEnvironment();
        var distribution = environment.InitialDistribution;
        Assert.Equal(1.0, distribution.Sum(), 6);
    }
}