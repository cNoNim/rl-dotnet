using System.Runtime.CompilerServices;

namespace RL.Core;

public static partial class List
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ZipList<IReadOnlyList<T1>, IReadOnlyList<T2>, T1, T2>
        Zip<T1, T2>(this IReadOnlyList<T1> list, IReadOnlyList<T2> other) =>
        Zip<IReadOnlyList<T1>, IReadOnlyList<T2>, T1, T2>(list, other);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ZipList<TList1, TList2, T1, T2>
        Zip<TList1, TList2, T1, T2>(this TList1 list, TList2 other)
        where TList1 : IReadOnlyList<T1>
        where TList2 : IReadOnlyList<T2> => new(list, other);
}
