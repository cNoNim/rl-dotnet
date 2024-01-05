using System;
using System.Numerics;
using RL.Core;

namespace RL.Generators;

public static partial class Generator
{
    public static int MaxIndex<TG, T>(this TG generator)
        where TG : IGenerator<T>
        where T : IComparisonOperators<T, T, bool>, IMinMaxValue<T>
    {
        if (!generator.IsFinite)
            throw new OverflowException();

        var maxIndex = -1;
        var maxValue = T.MinValue;

        foreach (var (value, index) in Index<TG, T>(generator))
        {
            if (index != -1L && value <= maxValue)
                continue;

            maxIndex = index;
            maxValue = value;
        }

        return maxIndex;
    }
}
