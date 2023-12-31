using System.Numerics;
using System.Runtime.CompilerServices;

namespace RL.Core;

public static partial class List
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Sum<TList, T>(this SelectList<TList, T, double> source)
        where TList : IReadOnlyList<T> =>
        Sum<SelectList<TList, T, double>, double, double>(source);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TSource Sum<TSource>(this IReadOnlyList<TSource> source)
        where TSource : INumberBase<TSource> =>
        Sum<IReadOnlyList<TSource>, TSource, TSource>(source);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Sum<TSource, TResult>(this IReadOnlyList<TSource> source)
        where TSource : INumberBase<TSource>
        where TResult : INumberBase<TResult> =>
        Sum<IReadOnlyList<TSource>, TSource, TResult>(source);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TSource Sum<TList, TSource>(this TList source)
        where TList : IReadOnlyList<TSource>
        where TSource : INumberBase<TSource> =>
        Sum<TList, TSource, TSource>(source);

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
