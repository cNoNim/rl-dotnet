using System.Collections;
using System.Numerics;

namespace RL.Core;

public readonly struct RangeList<T>(T from, T to, T step) :
    IReadOnlyList<T>
    where T : INumberBase<T>
{
    public int Count => int.CreateChecked((to - from) / step) + 1;
    public T this[int index] => from + step * T.CreateChecked(index);

    public ReadOnlyListStructEnumerator<RangeList<T>, T> GetEnumerator() => new(this);
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator().AsEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator().AsEnumerator();
}
