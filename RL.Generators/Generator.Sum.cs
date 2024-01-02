using System.Numerics;
using RL.Core;

namespace RL.Generators;

public static partial class Generator
{
    public static T Sum<T>(this IGenerator<T> generator)
        where T : INumberBase<T> =>
        Sum<IGenerator<T>, T, T>(generator);

    public static T Sum<TG, T>(this TG generator)
        where TG : IGenerator<T>
        where T : INumberBase<T> =>
        Sum<TG, T, T>(generator);

    public static TResult Sum<TSource, TResult>(this IGenerator<TSource> generator)
        where TSource : INumberBase<TSource>
        where TResult : INumberBase<TResult> =>
        Sum<IGenerator<TSource>, TSource, TResult>(generator);

    public static TResult Sum<TG, TSource, TResult>(this TG generator)
        where TG : IGenerator<TSource>
        where TSource : INumberBase<TSource>
        where TResult : INumberBase<TResult>
    {
        if (!generator.IsFinite)
            throw new OverflowException();

        var sum = TResult.Zero;
        foreach (var value in generator.AsGeneratorEnumerable<TG, TSource>())
            checked
            {
                sum += TResult.CreateChecked(value);
            }

        return sum;
    }
}
