using System;
using System.ComponentModel;
using System.Diagnostics;
using RL.Core;
using static System.Math;

namespace RL.Generators;

public readonly struct TakeGenerator<TG, T>(TG generator, int count) :
    IGenerator<TakeGenerator<TG, T>, T>
    where TG : IGenerator<T>
{
    public T this[int index]
    {
        get
        {
            Check(index, Count);
            return generator[index];

            [Conditional("DEBUG")]
            static void Check(int index, int count) =>
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, count);
        }
    }

    public GeneratorEnumerator<TakeGenerator<TG, T>, T> GetEnumerator() => new(this);
    public int Count { get; } = generator.IsFinite ? Min(generator.Count, count) : count;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsFinite => true;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool TryGetNext(int current, out int next)
    {
        if (current >= Count)
        {
            next = Count + 1;
            return false;
        }

        next = current + 1;
        return true;
    }
}