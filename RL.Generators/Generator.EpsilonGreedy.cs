using System.Numerics;
using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.Generators;

public static partial class Generator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectGenerator<
        TakeGenerator<SequenceGenerator<int>, int>,
        int,
        double,
        (int max, double epsilon, double em)
    > EpsilonGreedy<TG, T>(
        this TakeGenerator<TG, T> generator, double epsilon)
        where TG : IGenerator<T>
        where T : IComparisonOperators<T, T, bool>, IMinMaxValue<T> =>
        EpsilonGreedy<TakeGenerator<TG, T>, T>(generator, epsilon);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectGenerator<
        TakeGenerator<SequenceGenerator<int>, int>,
        int,
        double,
        (int max, double epsilon, double em)
    > EpsilonGreedy<T>(
        this IGenerator<T> generator, double epsilon)
        where T : IComparisonOperators<T, T, bool>, IMinMaxValue<T> =>
        EpsilonGreedy<IGenerator<T>, T>(generator, epsilon);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectGenerator<
        TakeGenerator<SequenceGenerator<int>, int>,
        int,
        double,
        (int max, double epsilon, double em)
    > EpsilonGreedy<TG, T>(this TG generator, double epsilon)
        where TG : IGenerator<T>
        where T : IMinMaxValue<T>, IComparisonOperators<T, T, bool> =>
        Select<TakeGenerator<SequenceGenerator<int>, int>, int, double, (int max, double epsilon, double em)>(
            Range<int>(generator.Count),
            (max: generator.MaxIndex<TG, T>(), epsilon, em: epsilon / double.CreateChecked(generator.Count)),
            (tuple, index) => index == tuple.max ? 1.0 - tuple.epsilon + tuple.em : tuple.em
        );
}