using System.Runtime.CompilerServices;
using RL.Core;
using RL.MDArrays;

namespace RL.Generators;

public static partial class Generator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectGenerator<
        ZipGenerator<TG, SequenceGenerator<int>, T, int>,
        (T value, int index),
        TTo,
        Func<T, int, TTo>
    > Select<TG, T, TTo>(this TG generator, Func<T, int, TTo> selector)
        where TG : IGenerator<T> =>
        Select<ZipGenerator<TG, SequenceGenerator<int>, T, int>, (T value, int index), TTo, Func<T, int, TTo>>(
            Index<TG, T>(generator),
            selector,
            static (s, tuple) => s(tuple.value, tuple.index)
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectGenerator<ReadOnlyAdapter<T>, T, TTo> Select<T, TTo>(
        this ReadOnlyAdapter<T> generator,
        Func<T, TTo> selector
    ) => Select<ReadOnlyAdapter<T>, T, TTo>(generator, selector);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectGenerator<IGenerator<T>, T, TTo> Select<T, TTo>(
        this IGenerator<T> generator,
        Func<T, TTo> selector
    ) => Select<IGenerator<T>, T, TTo>(generator, selector);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectGenerator<TG, T, TTo> Select<TG, T, TTo>(
        this TG generator,
        Func<T, TTo> selector
    ) where TG : IGenerator<T> =>
        new(generator, selector);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectGenerator<TakeGenerator<TG, T>, T, TTo, TContext> Select<TG, T, TTo, TContext>(
        this TakeGenerator<TG, T> generator,
        TContext context,
        Func<TContext, T, TTo> selector
    ) where TG : IGenerator<T> =>
        Select<TakeGenerator<TG, T>, T, TTo, TContext>(generator, context, selector);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectGenerator<Array1D<T>, T, TTo, TContext> Select<T, TTo, TContext>(
        this Array1D<T> generator,
        TContext context,
        Func<TContext, T, TTo> selector
    ) => Select<Array1D<T>, T, TTo, TContext>(generator, context, selector);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectGenerator<IGenerator<T>, T, TTo, TContext> Select<T, TTo, TContext>(
        this IGenerator<T> generator,
        TContext context,
        Func<TContext, T, TTo> selector
    ) => Select<IGenerator<T>, T, TTo, TContext>(generator, context, selector);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectGenerator<TG, T, TTo, TContext> Select<TG, T, TTo, TContext>(
        this TG generator,
        TContext context,
        Func<TContext, T, TTo> selector
    ) where TG : IGenerator<T> =>
        new(generator, context, selector);
}