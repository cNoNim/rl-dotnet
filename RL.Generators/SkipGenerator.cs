using System.ComponentModel;
using RL.Core;

namespace RL.Generators;

public readonly struct SkipGenerator<TG, T>(TG generator, int count) :
    IGenerator<SkipGenerator<TG, T>, T>
    where TG : IGenerator<T>
{
    public T this[int index] => generator[count + index];
    public int Count => generator.Count - count;

    public GeneratorEnumerator<SkipGenerator<TG, T>, T> GetEnumerator() => new(this);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsFinite => generator.IsFinite;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool TryGetNext(int current, out int next) => generator.TryGetNext(current, out next);
}
