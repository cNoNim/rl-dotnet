using System.Runtime.CompilerServices;

namespace RL.Core;

public readonly struct IndexedList<TList, T>(
    TList list
) : IStructList<IndexedList<TList, T>, (T value, int index)>
    where TList : IReadOnlyList<T>
{
    public int Count => list.Count;
    public (T, int) this[int index] => (list[index], index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StructEnumerator<IndexedList<TList, T>, (T value, int index)> GetEnumerator() => new(this);
}
