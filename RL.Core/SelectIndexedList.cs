using System.Collections;
using System.Runtime.CompilerServices;

namespace RL.Core;

public readonly struct SelectIndexedList<TList, T, TTo>(
    TList list,
    Func<T, int, TTo> selector
) : IReadOnlyList<TTo>
    where TList : IReadOnlyList<T>
{
    public int Count => list.Count;
    public TTo this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => selector(list[index], index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyListStructEnumerator<SelectIndexedList<TList, T, TTo>, TTo> GetEnumerator() => new(this);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator<TTo> IEnumerable<TTo>.GetEnumerator() => GetEnumerator().AsEnumerator();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator().AsEnumerator();
}
