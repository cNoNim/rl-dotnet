using System.Diagnostics;
using RL.Core;

namespace RL.Generators;

public readonly struct TakeGenerator<TG, T>(TG generator, int count) :
    IFiniteGenerator<TakeGenerator<TG, T>, T>
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

    public int Count { get; } = generator.IsFinite ? Math.Min(generator.Count, count) : count;

    public GeneratorEnumerator<TakeGenerator<TG, T>, T> GetEnumerator() => new(this);
}
