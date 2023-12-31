using System.Runtime.CompilerServices;
using static System.Math;

namespace RL.Core;

public readonly struct ZipList<TList1, TList2, T1, T2>(
    TList1 list1,
    TList2 list2
) : IStructList<ZipList<TList1, TList2, T1, T2>, (T1 first, T2 second)>
    where TList1 : IReadOnlyList<T1>
    where TList2 : IReadOnlyList<T2>
{
    public int Count => Min(list1.Count, list2.Count);
    public (T1 first, T2 second) this[int index] => (list1[index], list2[index]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StructEnumerator<ZipList<TList1, TList2, T1, T2>, (T1 first, T2 second)> GetEnumerator() => new(this);
}
