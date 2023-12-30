using System.Numerics;
using System.Runtime.CompilerServices;

namespace RL.Core;

public static partial class List
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Sum(this EpsilonGreedy.EpsilonGreedyProbabilityList source) =>
        source.Sum<EpsilonGreedy.EpsilonGreedyProbabilityList, double, double>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Sum<TList, T>(this SelectList<TList, T, double> source)
        where TList : IReadOnlyList<T> =>
        source.Sum<SelectList<TList, T, double>, double, double>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TSource Sum<TSource>(this IReadOnlyList<TSource> source)
        where TSource : INumberBase<TSource> =>
        source.Sum<IReadOnlyList<TSource>, TSource, TSource>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Sum<TSource, TResult>(this IReadOnlyList<TSource> source)
        where TSource : INumberBase<TSource>
        where TResult : INumberBase<TResult> =>
        source.Sum<IReadOnlyList<TSource>, TSource, TResult>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TSource Sum<TList, TSource>(this TList source)
        where TList : IReadOnlyList<TSource>
        where TSource : INumberBase<TSource> =>
        source.Sum<TList, TSource, TSource>();

    public static TResult Sum<TList, TSource, TResult>(this TList source)
        where TList : IReadOnlyList<TSource>
        where TSource : INumberBase<TSource>
        where TResult : INumberBase<TResult>
    {
        var sum = TResult.Zero;
        foreach (var value in source.AsStructEnumerable<TList, TSource>())
            checked
            {
                sum += TResult.CreateChecked(value);
            }

        return sum;
    }
}
