using System.Numerics;
using System.Runtime.CompilerServices;

namespace RL.Core;

public static partial class List
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectList<RangeList<int>, int, double, (int max, double epsilon, double em)>
        EpsilonGreedy<T>(this Matrix<T>.Row list, double epsilon)
        where T : IMinMaxValue<T>, IComparable<T>, INumberBase<T> =>
        EpsilonGreedy<Matrix<T>.Row, T>(list, epsilon);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectList<RangeList<int>, int, double, (int max, double epsilon, double em)>
        EpsilonGreedy<T>(this RangeList<T> list, double epsilon)
        where T : IMinMaxValue<T>, IComparable<T>, INumberBase<T> =>
        EpsilonGreedy<RangeList<T>, T>(list, epsilon);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectList<RangeList<int>, int, double, (int max, double epsilon, double em)>
        EpsilonGreedy<T>(this IReadOnlyList<T> list, double epsilon)
        where T : IMinMaxValue<T>, IComparable<T> =>
        EpsilonGreedy<IReadOnlyList<T>, T>(list, epsilon);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectList<RangeList<int>, int, double, (int max, double epsilon, double em)>
        EpsilonGreedy<TList, T>(this TList list, double epsilon)
        where TList : IReadOnlyList<T>
        where T : IMinMaxValue<T>, IComparable<T> =>
        Select(
            Range(list.Count),
            (max: list.MaxIndex<TList, T>(), epsilon, em: epsilon / double.CreateChecked(list.Count)),
            (tuple, index) => index == tuple.max ? 1.0 - tuple.epsilon + tuple.em : tuple.em
        );
}
