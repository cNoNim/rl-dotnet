using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.Random;

public interface IRandomGenerator
{
    public double Random(double min, double max);
    public float Random(float min, float max);
}

public class RandomGenerator(uint? seed = null) : IRandomGenerator
{
    private int _counter;

    public uint Seed { get; } = seed ?? StableHashCode.Hash(0, System.Random.Shared.Next());

    public double Random(double min, double max) => NextDouble() * (max - min) + min;

    public float Random(float min, float max) => NextFloat() * (max - min) + min;

    public double NextDouble()
    {
        var sx = ((ulong)NextState() << 20) ^ NextState();
        var d = Unsafe.BitCast<ulong, double>(0x3ff0000000000000 | sx);
        return d - 1.0;
    }

    public float NextFloat()
    {
        var sx = NextState() >> 9;
        var f = Unsafe.BitCast<uint, float>(0x3f800000 | sx);
        return f - 1.0f;
    }

    private uint NextState() =>
        StableHashCode.Hash(Seed, _counter++);
}