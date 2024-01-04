using System.Collections.Generic;
using System.ComponentModel;
using RL.Core;

namespace RL.Generators;

public readonly struct ReadOnlyAdapter<T>(IReadOnlyList<T> list) :
    IGenerator<ReadOnlyAdapter<T>, T>
{
    public T this[int index] => list[int.CreateChecked(index)];
    public int Count => list.Count;
    public GeneratorEnumerator<ReadOnlyAdapter<T>, T> GetEnumerator() => new(this);

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

    public static implicit operator ReadOnlyAdapter<T>(T[] list) => new(list);
}