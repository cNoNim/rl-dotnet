using System.Runtime.CompilerServices;

namespace RL.Core;

public static partial class List
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IndexedList<IReadOnlyList<T>, T> Index<T>(
        this IReadOnlyList<T> list
    ) => Index<IReadOnlyList<T>, T>(list);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IndexedList<TList, T> Index<TList, T>(
        this TList list
    ) where TList : IReadOnlyList<T> => new(list);
}
