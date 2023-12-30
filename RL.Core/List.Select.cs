using System.Runtime.CompilerServices;

namespace RL.Core;

public static partial class List
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectList<IReadOnlyList<T>, T, TTo> Select<T, TTo>(
        this IReadOnlyList<T> list,
        Func<T, TTo> selector
    ) => Select<IReadOnlyList<T>, T, TTo>(list, selector);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectList<TList, T, TTo> Select<TList, T, TTo>(
        this TList list,
        Func<T, TTo> selector
    ) where TList : IReadOnlyList<T> => new(list, selector);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectIndexedList<IReadOnlyList<T>, T, TTo> Select<T, TTo>(
        this IReadOnlyList<T> list,
        Func<T, int, TTo> selector
    ) => Select<IReadOnlyList<T>, T, TTo>(list, selector);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectIndexedList<TList, T, TTo> Select<TList, T, TTo>(
        this TList list,
        Func<T, int, TTo> selector
    ) where TList : IReadOnlyList<T> => new(list, selector);
}
