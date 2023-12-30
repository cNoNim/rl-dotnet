using System.Numerics;

namespace RL.Core;

public static partial class List
{
    public static T? Max<T>(this MatrixRowList<T> sequence)
        where T : IComparable<T>, IMinMaxValue<T> =>
        Max<MatrixRowList<T>, T>(sequence);

    public static T? Max<T>(this IReadOnlyList<T> sequence)
        where T : IComparable<T>, IMinMaxValue<T> =>
        Max<IReadOnlyList<T>, T>(sequence);


    public static T? Max<TList, T>(this TList sequence)
        where TList : IReadOnlyList<T>
        where T : IComparable<T>, IMinMaxValue<T>
    {
        var maxValue = default(T);

        foreach (var value in sequence.AsStructEnumerable<TList, T>())
        {
            if (value.CompareTo(maxValue) > 0 || maxValue == null)
            {
                maxValue = value;
            }
        }

        return maxValue;
    }
}
