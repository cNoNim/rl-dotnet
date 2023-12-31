using System.Numerics;
using System.Runtime.CompilerServices;

namespace RL.Core;

public static partial class List
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectList<RangeList<T>, T, TTo> Select<T, TTo>(
        this RangeList<T> list,
        Func<T, TTo> selector
    ) where T : INumberBase<T> =>
        Select<RangeList<T>, T, TTo>(list, selector);

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
    public static SelectList<RangeList<T>, T, TTo, TContext> Select<T, TTo, TContext>(
        this RangeList<T> list,
        TContext context,
        Func<TContext, T, TTo> selector
    ) where T : INumberBase<T> =>
        Select<RangeList<T>, T, TTo, TContext>(list, context, selector);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectList<IReadOnlyList<T>, T, TTo, TContext> Select<T, TTo, TContext>(
        this IReadOnlyList<T> list,
        TContext context,
        Func<TContext, T, TTo> selector
    ) => Select<IReadOnlyList<T>, T, TTo, TContext>(list, context, selector);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectList<TList, T, TTo, TContext> Select<TList, T, TTo, TContext>(
        this TList list,
        TContext context,
        Func<TContext, T, TTo> selector
    ) where TList : IReadOnlyList<T> => new(list, context, selector);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectList<IndexedList<IReadOnlyList<T>, T>, (T value, int index), TTo, Func<T, int, TTo>>
        Select<T, TTo>(
            this IReadOnlyList<T> list,
            Func<T, int, TTo> selector
        ) => Select<IReadOnlyList<T>, T, TTo>(list, selector);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectList<IndexedList<TList, T>, (T, int), TTo, Func<T, int, TTo>>
        Select<TList, T, TTo>(this TList list, Func<T, int, TTo> selector)
        where TList : IReadOnlyList<T> =>
        Select<IndexedList<TList, T>, (T value, int index), TTo, Func<T, int, TTo>>(
            Index<TList, T>(list),
            selector,
            static (s, tuple) => s(tuple.value, tuple.index)
        );
}
