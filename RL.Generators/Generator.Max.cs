using System.Numerics;
using RL.Core;
using RL.MDArrays;

namespace RL.Generators;

public static partial class Generator
{
    public static T Max<T>(this Array2D<T>.Row generator)
        where T : IComparisonOperators<T, T, bool>, IMinMaxValue<T> =>
        Max<Array2D<T>.Row, T>(generator);

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