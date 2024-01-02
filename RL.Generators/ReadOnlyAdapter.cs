using RL.Core;

namespace RL.Generators;

public readonly struct ReadOnlyAdapter<T>(IReadOnlyList<T> list) :
    IFiniteGenerator<ReadOnlyAdapter<T>, T>
{
    public T this[int index] => list[int.CreateChecked(index)];
    public int Count => list.Count;
    public GeneratorEnumerator<ReadOnlyAdapter<T>, T> GetEnumerator() => new(this);
}