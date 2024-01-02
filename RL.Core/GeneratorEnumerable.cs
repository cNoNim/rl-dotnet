using System.Runtime.CompilerServices;

namespace RL.Core;

public readonly ref struct GeneratorEnumerable<TG, T>(TG generator)
    where TG : IGenerator<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GeneratorEnumerator<TG, T> GetEnumerator() => new(generator);
}