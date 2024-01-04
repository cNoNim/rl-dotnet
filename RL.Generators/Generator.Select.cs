using System;
using System.Runtime.CompilerServices;
using RL.Core;

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
    public static SelectGenerator<ChunkGenerator<TG, T>, TakeGenerator<SkipGenerator<TG, T>, T>, TTo>
        Select<TG, T, TTo>(
            this ChunkGenerator<TG, T> generator,
            Func<TakeGenerator<SkipGenerator<TG, T>, T>, TTo> selector
        ) where TG : IGenerator<T> =>
        Select<ChunkGenerator<TG, T>, TakeGenerator<SkipGenerator<TG, T>, T>, TTo>(generator, selector);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectGenerator<SelectGenerator<TG, T, TTo>, TTo, TR> Select<TG, T, TTo, TR>(
        this SelectGenerator<TG, T, TTo> generator,
        Func<TTo, TR> selector
    ) where TG : IGenerator<T> =>
        Select<SelectGenerator<TG, T, TTo>, TTo, TR>(generator, selector);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectGenerator<SelectGenerator<TG, T, TTo, TContext>, TTo, TR> Select<TG, T, TTo, TR, TContext>(
        this SelectGenerator<TG, T, TTo, TContext> generator,
        Func<TTo, TR> selector
    ) where TG : IGenerator<T> =>
        Select<SelectGenerator<TG, T, TTo, TContext>, TTo, TR>(generator, selector);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectGenerator<ArrayCacheGenerator<TG, T>, T, TTo> Select<TG, T, TTo>(
        this ArrayCacheGenerator<TG, T> generator,
        Func<T, TTo> selector
    ) where TG : IGenerator<T> =>
        Select<ArrayCacheGenerator<TG, T>, T, TTo>(generator, selector);

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