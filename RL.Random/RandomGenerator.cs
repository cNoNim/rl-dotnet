using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.Random;

public class RandomGenerator(uint? seed = null) : IRandomGenerator
{
    private int _counter;

    public uint Seed { get; } = seed ?? StableHashCode.Hash(0, System.Random.Shared.Next());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double Random(double min, double max) => NextDouble() * (max - min) + min;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Random(float min, float max) => NextFloat() * (max - min) + min;

    public T Random<T>(T min, T max) where T : INumber<T>
    {
        if (typeof(T) == typeof(double))
            return T.CreateChecked(Random(double.CreateChecked(min), double.CreateChecked(max)));
        if (typeof(T) == typeof(float))
            return T.CreateChecked(Random(float.CreateChecked(min), float.CreateChecked(max)));
        if (typeof(T) == typeof(int))
            return T.CreateChecked(Random(int.CreateChecked(min), int.CreateChecked(max)));
        throw new NotSupportedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Random(int min, int max)
    {
        var range = (uint)(max - min);
        return (int)(NextState() * (ulong)range >> 32) + min;
    }

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