using System.Runtime.CompilerServices;

namespace RL.Core;

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public struct GeneratorEnumerator<TG, T>(TG generator) : IGeneratorEnumerator<T>
    where TG : IGenerator<T>
{
    private int _next;

    public readonly T Current => generator[_next - 1];

    public bool MoveNext() => generator.TryGetNext(_next, out _next);

    public DisposableEnumerator<T> AsEnumerator() =>
        new(this);
}