using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.Random;

public class RandomGenerator(uint? seed = null) : IRandomGenerator
{
    private int _counter;

    public uint Seed { get; } = seed ?? StableHashCode.Hash(0, System.Random.Shared.Next());

    public uint Random(uint max)
    {
        var product = (ulong)max * NextState();
        var low = (uint)product;

        if (low < max)
            unchecked
            {
                var remainder = (0u - max) % max;
                while (low < remainder)
                {
                    product = (ulong)max * NextState();
                    low = (uint)product;
                }
            }

        return (uint)(product >> 32);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Random(int min, int max) =>
        (int)Random((uint)(max - min)) + min;

    public long Random(long min, long max)
    {
        var range = (ulong)(max - min);

        if (range < uint.MaxValue)
            return Random((uint)range) + min;

        var bits = Log2Ceiling(range);
        while (true)
        {
            var result = NextState64() >> (sizeof(ulong) * 8 - bits);
            if (result < range)
                return (long)result + min;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Random(float min, float max) => NextFloat() * (max - min) + min;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double Random(double min, double max) => NextDouble() * (max - min) + min;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 Random(Vector2 min, Vector2 max) =>
        new(Random(min.X, max.X), Random(min.Y, max.Y));

    public T Random<T>(T min, T max)
    {
        switch (min, max)
        {
            case (int mn, int mx):
            {
                var random = Random(mn, mx);
                return Unsafe.As<int, T>(ref random);
            }
            case (long mn, long mx):
            {
                var random = Random(mn, mx);
                return Unsafe.As<long, T>(ref random);
            }
            case (float mn, float mx):
            {
                var random = Random(mn, mx);
                return Unsafe.As<float, T>(ref random);
            }
            case (double mn, double mx):
            {
                var random = Random(mn, mx);
                return Unsafe.As<double, T>(ref random);
            }
            case (Vector2 mn, Vector2 mx):
            {
                var random = Random(mn, mx);
                return Unsafe.As<Vector2, T>(ref random);
            }
            default:
                throw new NotSupportedException();
        }
    }

    private double NextDouble() =>
        (NextState64() >> 11) * (1.0 / (1ul << 53));

    private float NextFloat() =>
        (NextState() >> 8) * (1.0f / (1u << 24));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint NextState() =>
        StableHashCode.Hash(Seed, _counter++);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ulong NextState64() => ((ulong)NextState() << 32) | NextState();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Log2Ceiling(ulong value)
    {
        var result = BitOperations.Log2(value);
        return BitOperations.PopCount(value) != 1 ? result + 1 : result;
    }
}
