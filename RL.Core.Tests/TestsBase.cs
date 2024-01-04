using System.Text;
using Xunit.Abstractions;

namespace RL.Core.Tests;

public class TestsBase(ITestOutputHelper output) : IDisposable
{
    protected readonly StringBuilder Output = new();

    void IDisposable.Dispose()
    {
        if (Output.Length != 0)
            output.WriteLine(Output.ToString());
        GC.SuppressFinalize(this);
    }
}