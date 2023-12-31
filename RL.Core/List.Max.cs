using System.Numerics;

namespace RL.Core;

public static partial class List
{
    public static T Max<T>(this Matrix<T>.Row sequence)
        where T : IComparisonOperators<T, T, bool>, IMinMaxValue<T> =>
        Max<Matrix<T>.Row, T>(sequence);

    public static T Max<T>(this IReadOnlyList<T> sequence)
        where T : IComparisonOperators<T, T, bool>, IMinMaxValue<T> =>
        Max<IReadOnlyList<T>, T>(sequence);

    public static T Max<TList, T>(this TList sequence)
        where TList : IReadOnlyList<T>
        where T : IComparisonOperators<T, T, bool>, IMinMaxValue<T>
    {
        var maxValue = T.MinValue;
        var empty = true;

        foreach (var value in sequence.AsStructEnumerable<TList, T>())
        {
            empty = false;
            if (value > maxValue)
                maxValue = value;
        }

        return !empty ? maxValue : throw new InvalidOperationException("No Elements");
    }
}
