using System.Numerics;
using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.Generators;

public static partial class Generator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TTo Average<TG, T, TTo>(this SelectGenerator<TG, T, TTo> generator)
        where TG : IGenerator<T>
        where TTo : INumberBase<TTo> =>
        Average<SelectGenerator<TG, T, TTo>, TTo, TTo>(generator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Average<T>(this ReadOnlyAdapter<T> generator)
        where T : INumberBase<T> =>
        Average<ReadOnlyAdapter<T>, T, T>(generator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Average<T>(this IGenerator<T> generator)
        where T : INumberBase<T> =>
        Average<IGenerator<T>, T, T>(generator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Average<TG, T>(this TG generator)
        where TG : IGenerator<T>
        where T : INumberBase<T> =>
        Average<TG, T, T>(generator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Average<TG, TSource, TResult>(this TG generator)
        where TG : IGenerator<TSource>
        where TSource : INumberBase<TSource>
        where TResult : INumberBase<TResult> =>
        Sum<TG, TSource, TResult>(generator) / TResult.CreateChecked(Count<TG, TSource>(generator));
}