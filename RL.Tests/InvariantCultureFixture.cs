using System.Globalization;

namespace RL.Tests;

public sealed class InvariantCultureFixture : IDisposable
{
    private readonly CultureInfo? _defaultThreadCurrentCulture;

    public InvariantCultureFixture()
    {
        _defaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentCulture;
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
    }

    public void Dispose()
    {
        CultureInfo.DefaultThreadCurrentCulture = _defaultThreadCurrentCulture;
    }
}
