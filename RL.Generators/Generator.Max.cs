using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.Generators;

public static partial class Generator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<TG, T>(this TakeGenerator<TG, T> generator)
        where TG : IGenerator<T>
        where T : IComparisonOperators<T, T, bool>, IMinMaxValue<T> =>
        Max<TakeGenerator<TG, T>, T>(generator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T>(this IGenerator<T> generator)
        where T : IComparisonOperators<T, T, bool>, IMinMaxValue<T> =>
        Max<IGenerator<T>, T>(generator);

    public static T Max<TG, T>(this TG generator)
        where TG : IGenerator<T>
        where T : IComparisonOperators<T, T, bool>, IMinMaxValue<T>
    {
        if (!generator.IsFinite)
            throw new OverflowException();

        var maxValue = T.MinValue;
        var empty = true;

        foreach (var value in generator.AsGeneratorEnumerable<TG, T>())
        {
            empty = false;
            if (value > maxValue)
                maxValue = value;
        }

        return !empty ? maxValue : throw new InvalidOperationException("No Elements");
    }
}