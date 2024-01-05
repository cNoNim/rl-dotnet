using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RL.Core;

public static class StableHashCode
{
    private const uint P1 = 2654435761U;
    private const uint P2 = 2246822519U;
    private const uint P3 = 3266489917U;
    private const uint P4 = 668265263U;
    private const uint P5 = 374761393U;

    public static uint Hash(uint seed, ReadOnlySpan<int> values) =>
        Hash(seed, MemoryMarshal.CreateReadOnlySpan(
            ref Unsafe.As<int, uint>(ref MemoryMarshal.GetReference(values)),
            values.Length
        ));

    public static uint Hash(uint seed, ReadOnlySpan<uint> buf)
    {
        uint hash;
        var index = 0;
        var length = buf.Length;

        if (length < 4)
            hash = MixEmptyState(seed);
        else
        {
            var (h1, h2, h3, h4) = Initialize(seed);

            var limit = length - 4;
            do
            {
                h1 = Round(h1, buf[index++]);
                h2 = Round(h2, buf[index++]);
                h3 = Round(h3, buf[index++]);
                h4 = Round(h4, buf[index++]);
            } while (index <= limit);

            hash = MixState(h1, h2, h3, h4);
        }

        hash += (uint)length * 4;

        while (index < length)
            hash = QueueRound(hash, buf[index++]);

        return MixFinal(hash);
    }

    public static uint HashGenerator<TG>(uint seed, TG generator)
        where TG : IGenerator<uint>
    {
        var index = 0;
        var length = generator.Count;

        uint hash;
        if (length < 4)
            hash = MixEmptyState(seed);
        else
        {
            var (h1, h2, h3, h4) = Initialize(seed);

            var limit = length - 4;
            do
            {
                h1 = Round(h1, generator[index++]);
                h2 = Round(h2, generator[index++]);
                h3 = Round(h3, generator[index++]);
                h4 = Round(h4, generator[index++]);
            } while (index <= limit);

            hash = MixState(h1, h2, h3, h4);
        }

        hash += (uint)length * 4;

        while (index < length)
            hash = QueueRound(hash, generator[index++]);

        return MixFinal(hash);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Hash(uint seed, int v1) =>
        Hash(seed, (uint)v1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Hash(uint seed, int v1, int v2) =>
        Hash(seed, (uint)v1, (uint)v2);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Hash(uint seed, int v1, int v2, int v3) =>
        Hash(seed, (uint)v1, (uint)v2, (uint)v3);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Hash(uint seed, int v1, int v2, int v3, int v4) =>
        Hash(seed, (uint)v1, (uint)v2, (uint)v3, (uint)v4);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Hash(uint seed, int v1, int v2, int v3, int v4, int v5) =>
        Hash(seed, (uint)v1, (uint)v2, (uint)v3, (uint)v4, (uint)v5);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Hash(uint seed, int v1, int v2, int v3, int v4, int v5, int v6) =>
        Hash(seed, (uint)v1, (uint)v2, (uint)v3, (uint)v4, (uint)v5, (uint)v6);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Hash(uint seed, int v1, int v2, int v3, int v4, int v5, int v6, int v7) =>
        Hash(seed, (uint)v1, (uint)v2, (uint)v3, (uint)v4, (uint)v5, (uint)v6, (uint)v7);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Hash(uint seed, int v1, int v2, int v3, int v4, int v5, int v6, int v7, int v8) =>
        Hash(seed, (uint)v1, (uint)v2, (uint)v3, (uint)v4, (uint)v5, (uint)v6, (uint)v7, (uint)v8);

    public static uint Hash(uint seed, uint v1)
    {
        var hash = MixEmptyState(seed);
        hash += 4;
        hash = QueueRound(hash, v1);
        return MixFinal(hash);
    }

    public static uint Hash(uint seed, uint v1, uint v2)
    {
        var hash = MixEmptyState(seed);
        hash += 8;
        hash = QueueRound(hash, v1);
        hash = QueueRound(hash, v2);
        return MixFinal(hash);
    }

    public static uint Hash(uint seed, uint v1, uint v2, uint v3)
    {
        var hash = MixEmptyState(seed);
        hash += 12;
        hash = QueueRound(hash, v1);
        hash = QueueRound(hash, v2);
        hash = QueueRound(hash, v3);
        return MixFinal(hash);
    }

    public static uint Hash(uint seed, uint v1, uint v2, uint v3, uint v4)
    {
        var (h1, h2, h3, h4) = Initialize(seed);

        h1 = Round(h1, v1);
        h2 = Round(h2, v2);
        h3 = Round(h3, v3);
        h4 = Round(h4, v4);

        var hash = MixState(h1, h2, h3, h4);
        hash += 16;
        return MixFinal(hash);
    }

    public static uint Hash(uint seed, uint v1, uint v2, uint v3, uint v4, uint v5)
    {
        var (h1, h2, h3, h4) = Initialize(seed);

        h1 = Round(h1, v1);
        h2 = Round(h2, v2);
        h3 = Round(h3, v3);
        h4 = Round(h4, v4);

        var hash = MixState(h1, h2, h3, h4);
        hash += 20;
        hash = QueueRound(hash, v5);
        return MixFinal(hash);
    }

    public static uint Hash(uint seed, uint v1, uint v2, uint v3, uint v4, uint v5, uint v6)
    {
        var (h1, h2, h3, h4) = Initialize(seed);

        h1 = Round(h1, v1);
        h2 = Round(h2, v2);
        h3 = Round(h3, v3);
        h4 = Round(h4, v4);

        var hash = MixState(h1, h2, h3, h4);
        hash += 24;
        hash = QueueRound(hash, v5);
        hash = QueueRound(hash, v6);
        return MixFinal(hash);
    }

    public static uint Hash(uint seed, uint v1, uint v2, uint v3, uint v4, uint v5, uint v6, uint v7)
    {
        var (h1, h2, h3, h4) = Initialize(seed);

        h1 = Round(h1, v1);
        h2 = Round(h2, v2);
        h3 = Round(h3, v3);
        h4 = Round(h4, v4);

        var hash = MixState(h1, h2, h3, h4);
        hash += 28;
        hash = QueueRound(hash, v5);
        hash = QueueRound(hash, v6);
        hash = QueueRound(hash, v7);
        return MixFinal(hash);
    }

    public static uint Hash(uint seed, uint v1, uint v2, uint v3, uint v4, uint v5, uint v6, uint v7, uint v8)
    {
        var (h1, h2, h3, h4) = Initialize(seed);

        h1 = Round(h1, v1);
        h2 = Round(h2, v2);
        h3 = Round(h3, v3);
        h4 = Round(h4, v4);

        h1 = Round(h1, v4);
        h2 = Round(h2, v6);
        h3 = Round(h3, v7);
        h4 = Round(h4, v8);

        var hash = MixState(h1, h2, h3, h4);
        hash += 32;
        return MixFinal(hash);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (uint h1, uint h2, uint h3, uint h4) Initialize(uint seed) => (
        h1: seed + P1 + P2,
        h2: seed + P2,
        h3: seed,
        h4: seed - P1
    );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint MixEmptyState(uint seed) =>
        seed + P5;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint MixState(uint v1, uint v2, uint v3, uint v4) =>
        BitOperations.RotateLeft(v1, 1) +
        BitOperations.RotateLeft(v2, 7) +
        BitOperations.RotateLeft(v3, 12) +
        BitOperations.RotateLeft(v4, 18);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint Round(uint hash, uint input) =>
        BitOperations.RotateLeft(hash + input * P2, 13) * P1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint QueueRound(uint hash, uint input) =>
        BitOperations.RotateLeft(hash + input * P3, 17) * P4;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint MixFinal(uint hash)
    {
        hash ^= hash >> 15;
        hash *= P2;
        hash ^= hash >> 13;
        hash *= P3;
        hash ^= hash >> 16;
        return hash;
    }
}