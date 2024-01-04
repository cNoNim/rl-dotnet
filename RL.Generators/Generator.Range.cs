using System.Numerics;
using System.Runtime.CompilerServices;

namespace RL.Generators;

public static partial class Generator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TakeGenerator<SequenceGenerator<int>, int> Range(int count) =>
        Sequence<int>().Take<SequenceGenerator<int>, int>(count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TakeGenerator<SequenceGenerator<T>, T> Range<T>(int count)
        where T : INumberBase<T> =>
        Sequence<T>().Take<SequenceGenerator<T>, T>(count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectGenerator<TakeGenerator<SequenceGenerator<int>, int>, int, T, (T from, T step)>
        Range<T>(T from, T to)
        where T : INumberBase<T> =>
        Range(from, to, T.One);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectGenerator<TakeGenerator<SequenceGenerator<int>, int>, int, T, (T from, T step)>
        Range<T>(T from, T to, T step)
        where T : INumberBase<T> =>
        Select<TakeGenerator<SequenceGenerator<int>, int>, int, T, (T from, T step)>(
            Range<int>(int.CreateChecked((to - from) / step) + 1),
            (from, step),
            (ctx, index) => ctx.from + ctx.step * T.CreateChecked(index)
        );
}