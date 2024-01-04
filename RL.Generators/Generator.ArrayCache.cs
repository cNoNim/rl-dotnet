using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.Generators;

public static partial class Generator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayCacheGenerator<SelectGenerator<TG, T, TTo, TContext>, TTo> ArrayCache<TG, T, TTo, TContext>(
        this SelectGenerator<TG, T, TTo, TContext> generator
    ) where TG : IGenerator<T> =>
        ArrayCache<SelectGenerator<TG, T, TTo, TContext>, TTo>(generator);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayCacheGenerator<IGenerator<T>, T> ArrayCache<T>(this IGenerator<T> generator) =>
        ArrayCache<IGenerator<T>, T>(generator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayCacheGenerator<TG, T> ArrayCache<TG, T>(this TG generator)
        where TG : IGenerator<T> =>
        new(generator);
}