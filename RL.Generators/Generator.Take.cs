using System.Numerics;
using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.Generators;

public static partial class Generator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TakeGenerator<SequenceGenerator<T>, T> Take<T>(this SequenceGenerator<T> generator, int count)
        where T : INumberBase<T> =>
        Take<SequenceGenerator<T>, T>(generator, count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TakeGenerator<SkipGenerator<TG, T>, T> Take<TG, T>(this SkipGenerator<TG, T> generator, int count)
        where TG : IGenerator<T> =>
        Take<SkipGenerator<TG, T>, T>(generator, count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TakeGenerator<SelectGenerator<TG, T, TTo, TContext>, TTo> Take<TG, T, TTo, TContext>(
        this SelectGenerator<TG, T, TTo, TContext> generator,
        int count
    ) where TG : IGenerator<T> =>
        Take<SelectGenerator<TG, T, TTo, TContext>, TTo>(generator, count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TakeGenerator<IGenerator<T>, T> Take<T>(this IGenerator<T> generator, int count) =>
        Take<IGenerator<T>, T>(generator, count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TakeGenerator<TG, T> Take<TG, T>(this TG generator, int count)
        where TG : IGenerator<T> =>
        new(generator, count);
}