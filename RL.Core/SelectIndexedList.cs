using System.Runtime.CompilerServices;

namespace RL.Core;

public readonly struct SelectIndexedList<TList, T, TTo>(
    TList list,
    Func<T, int, TTo> selector
) : IStructList<SelectIndexedList<TList, T, TTo>, TTo>
    where TList : IReadOnlyList<T>
{
    public int Count => list.Count;
    public TTo this[int index] => selector(list[index], index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StructEnumerator<SelectIndexedList<TList, T, TTo>, TTo> GetEnumerator() => new(this);
}
