using System.Numerics;
using RL.Core;

namespace RL.Generators;

public static partial class Generator
{
    public static SkipGenerator<SequenceGenerator<T>, T> Skip<T>(
        this SequenceGenerator<T> generator,
        int count
    ) where T : INumberBase<T> =>
        Skip<SequenceGenerator<T>, T>(generator, count);

    public static SkipGenerator<IGenerator<T>, T> Skip<T>(this IGenerator<T> generator, int count) =>
        Skip<IGenerator<T>, T>(generator, count);

    public static SkipGenerator<TG, T> Skip<TG, T>(this TG generator, int count)
        where TG : IGenerator<T> =>
        new(generator, count);
}