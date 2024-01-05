using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.Generators;

public readonly struct ChunkGenerator<TG, T>(TG generator, int chunkSize) :
    IGenerator<ChunkGenerator<TG, T>, TakeGenerator<SkipGenerator<TG, T>, T>>
    where TG : IGenerator<T>
{
    public TakeGenerator<SkipGenerator<TG, T>, T> this[int index] => generator
        .Skip<TG, T>(index * chunkSize)
        .Take(chunkSize);

    public int Count => 
        generator.IsFinite ? InternalCount() : throw new OverflowException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int InternalCount() => (generator.Count + chunkSize - 1) / chunkSize;

    public GeneratorEnumerator<ChunkGenerator<TG, T>, TakeGenerator<SkipGenerator<TG, T>, T>> GetEnumerator() =>
        new(this);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsFinite => generator.IsFinite;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool TryGetNext(int current, out int next)
    {
        if (!generator.IsFinite)
        {
            next = current + 1;
            return true;
        }

        var count = InternalCount();
        if (current >= count)
        {
            next = count + 1;
            return false;
        }

        next = current + 1;
        return true;
    }
}