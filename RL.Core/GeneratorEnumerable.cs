using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RL.Core;

public readonly ref struct GeneratorEnumerable<TG, T>(TG generator)
    where TG : IGenerator<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GeneratorEnumerator<TG, T> GetEnumerator() => new(generator);
}

public readonly struct DisposableGeneratorEnumerable<TG, T>(TG generator) : IEnumerable<T>
    where TG : IGenerator<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GeneratorEnumerator<TG, T> GetEnumerator() => new(generator);

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator().AsEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator().AsEnumerator();
}