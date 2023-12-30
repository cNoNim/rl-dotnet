using System.Collections;

namespace RL.Core;

public readonly struct SelectList<TList, T, TTo>(
    TList list,
    Func<T, TTo> selector
) : IReadOnlyList<TTo>
    where TList : IReadOnlyList<T>
{
    public int Count => list.Count;
    public TTo this[int index] => selector(list[index]);

    public ReadOnlyListStructEnumerator<SelectList<TList, T, TTo>, TTo> GetEnumerator() => new(this);
    IEnumerator<TTo> IEnumerable<TTo>.GetEnumerator() => GetEnumerator().AsEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator().AsEnumerator();
}
