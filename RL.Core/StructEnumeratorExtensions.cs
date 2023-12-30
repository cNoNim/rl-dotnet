using System.Runtime.CompilerServices;

namespace RL.Core;

public static class StructEnumeratorExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyListStructEnumerable<IReadOnlyList<T>, T> AsStructEnumerable<T>(
        this IReadOnlyList<T> list
    ) => list.AsStructEnumerable(0, list.Count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyListStructEnumerable<IReadOnlyList<T>, T> AsStructEnumerable<T>(
        this IReadOnlyList<T> list,
        int index
    ) => list.AsStructEnumerable(index, list.Count - index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyListStructEnumerable<IReadOnlyList<T>, T> AsStructEnumerable<T>(
        this IReadOnlyList<T> list,
        int index,
        int count
    ) => new(list, index, count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyListStructEnumerable<TList, T> AsStructEnumerable<TList, T>(
        this TList list
    ) where TList : IReadOnlyList<T> => list.AsStructEnumerable<TList, T>(0, list.Count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyListStructEnumerable<TList, T> AsStructEnumerable<TList, T>(
        this TList list,
        int index
    ) where TList : IReadOnlyList<T> => list.AsStructEnumerable<TList, T>(index, list.Count - index);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyListStructEnumerable<TList, T> AsStructEnumerable<TList, T>(
        this TList list,
        int index,
        int count
    ) where TList : IReadOnlyList<T> => new(list, index, count);
}
