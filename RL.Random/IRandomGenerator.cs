using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.Random;

public interface IRandomGenerator
{
    public double Random(double min, double max);
}

public class RandomGenerator(uint? seed = null) : IRandomGenerator
{
    private int _counter;

    public uint Seed { get; } = seed ?? StableHashCode.Hash(0, System.Random.Shared.Next());

    public double Random(double min, double max) => NextDouble() * (max - min) + min;

    public double NextDouble()
    {
        var sx = ((ulong)NextState() << 20) ^ NextState();
        var d = Unsafe.BitCast<ulong, double>(0x3ff0000000000000 | sx);
        return d - 1.0;
    }

    private uint NextState() =>
        StableHashCode.Hash(Seed, _counter++);
}