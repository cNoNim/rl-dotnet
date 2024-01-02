using System.Numerics;
using RL.Core;

namespace RL.Generators;

public static partial class Generator
{
    public static TakeGenerator<SequenceGenerator<T>, T> Take<T>(this SequenceGenerator<T> generator, int count)
        where T : INumberBase<T> =>
        Take<SequenceGenerator<T>, T>(generator, count);

    public static TakeGenerator<IGenerator<T>, T> Take<T>(this IGenerator<T> generator, int count) =>
        Take<IGenerator<T>, T>(generator, count);

    public static TakeGenerator<TG, T> Take<TG, T>(this TG generator, int count)
        where TG : IGenerator<T> =>
        new(generator, count);
}