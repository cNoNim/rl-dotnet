using System.Numerics;

namespace RL.Core;

public static partial class List
{
    public static int MaxIndex<TList, T>(this TList sequence)
        where TList : IReadOnlyList<T>
        where T : IComparable<T>, IMinMaxValue<T>
    {
        var maxIndex = -1;
        var maxValue = T.MinValue;

        var index = 0;
        foreach (var value in sequence.AsStructEnumerable<TList, T>())
        {
            if (value.CompareTo(maxValue) > 0 || maxIndex == -1)
            {
                maxIndex = index;
                maxValue = value;
            }

            index++;
        }

        return maxIndex;
    }
}
